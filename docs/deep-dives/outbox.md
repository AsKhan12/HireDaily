# Transactional Outbox

## Overview

Hiredaily uses the **Transactional Outbox Pattern** to reliably transition from transactional domain operations to asynchronous integration-event processing.

The key requirement is:

> When a business operation succeeds, the event describing that operation must not be lost.

For example, when a job is created, the job itself must be persisted together with the information required to notify downstream components such as the Job Feed processor.

The implementation separates this into two stages:

```text
Business Transaction
        │
        ▼
Domain Event
        │
        ▼
Outbox Message
        │
        ▼
Database Commit
        │
        ▼
Outbox Publisher
        │
        ▼
Message Broker
```

This means the HTTP request that creates a job does not need to directly publish a message to the broker.

---

# 1. The Problem

Consider a straightforward implementation:

```text
Create Job
    │
    ▼
Save Job
    │
    ▼
Publish Event
```

There is a failure window between the database operation and message publication.

For example:

```text
Save Job
    │
    ▼
Database succeeds ✓
    │
    ▼
Publish Event
    │
    ✗ Broker unavailable
```

The job now exists in the database, but downstream consumers have never been told that the job was created.

The opposite problem is also possible if message publication happens before the database transaction completes.

The system therefore needs a durable representation of the event that is committed together with the business transaction.

---

# 2. Hiredaily's Approach

Hiredaily uses an **OutboxMessage** persisted in the same SQL Server database as the transactional domain data.

The high-level flow is:

```text
Create Job
    │
    ▼
Job Entity
    │
    └── Domain Event
            │
            ▼
       Unit of Work
            │
            ▼
      Outbox Message
            │
            ▼
      SQL Transaction
            │
          Commit
            │
            ▼
    Outbox Processor
            │
            ▼
      Message Broker
```

The important distinction is that **the Unit of Work does not publish to the external broker**.

It makes the event durable.

A separate process is responsible for eventually publishing that durable message.

---

# 3. Job Creation

The transactional flow begins in the `CreateJobCommandHandler`.

The handler creates the domain `Job`, adds it through the repository, and delegates transaction coordination to the Unit of Work:

```text
CreateJobCommandHandler
        │
        ▼
Create Job
        │
        ▼
jobRepository.AddAsync(...)
        │
        ▼
unitOfWork.CommitAsync(...)
```

The handler therefore does not need to know how domain events are converted into outbox messages or how messages are published.

This keeps the application feature focused on the business operation.

---

# 4. Unit of Work

The Unit of Work is the central part of the current implementation.

During `CommitAsync`, it first obtains the entities tracked by the `DbContext` that implement `IEntity` and collects their domain events.

It then explicitly begins a database transaction.

Conceptually:

```text
DbContext Change Tracker
        │
        ▼
Tracked Entities
        │
        ▼
Domain Events
        │
        ▼
Begin Transaction
```

The first `SaveChangesAsync` persists the normal entity changes while the transaction is still open.

The important detail is that the transaction has **not yet been committed**.

---

# 5. Domain Event Dispatch

After the initial database save, the Unit of Work dispatches the collected domain events:

```text
SaveChanges
    │
    ▼
Dispatch Domain Events
```

The current implementation calls:

```text
dispatcher.Dispatch(domainEvent)
```

for each collected event.

The `IntegrationEventDispatcher` dynamically resolves the handler corresponding to the concrete event type and invokes it.

This gives the domain event a chance to trigger application-level work such as creating an outbox message.

Importantly, this is still happening **inside the database transaction**.

---

# 6. Creating the Outbox Message

The outbox repository exposes an `AddMessage` operation for adding a message to the outbox:

```text
IOutboxRepository
    │
    └── AddMessage(...)
```

It also exposes operations for retrieving unpublished messages, marking messages as published, and saving changes.

The resulting `OutboxMessage` contains:

* `Id`
* `EventType`
* `Payload`
* `OccurredAt`
* `PublishedAt`
* `RetryCount`
* `Error`

These fields provide both the information required to publish the event and the state required to track delivery attempts.

Conceptually:

```text
Domain Event
     │
     ▼
Event Handler
     │
     ▼
OutboxRepository.AddMessage(...)
     │
     ▼
OutboxMessage
```

---

# 7. The Second Save

After domain-event handlers have executed, the Unit of Work checks whether the `DbContext` contains additional changes.

If it does, it calls `SaveChangesAsync` again.

This is important because the domain-event handler can add the `OutboxMessage` to the same `DbContext`.

The resulting transaction is therefore conceptually:

```text
┌─────────────────────────────────────┐
│         SQL Transaction             │
│                                     │
│  Job                                 │
│    +                                │
│  OutboxMessage                       │
│                                     │
└─────────────────────────────────────┘
                  │
                Commit
```

If anything fails before the commit, the transaction is rolled back. The Unit of Work explicitly rolls back the transaction and rethrows the exception.

This is the core reliability property of the design.

---

# 8. Why the Outbox Is Reliable

Consider the following failure:

```text
Create Job
    │
    ▼
Save Job
    │
    ▼
Create Outbox Message
    │
    ▼
Save Outbox Message
    │
    ▼
Something fails
```

Because both operations occur within the same database transaction, the transaction is rolled back.

The system should therefore not end up with:

```text
Job exists
Outbox message missing
```

Instead, the transaction either commits the complete operation or rolls it back.

---

# 9. Publishing the Outbox

Once the transaction has committed, a separate `JobOutboxMessagePublisher` is responsible for delivering the messages to the message broker.

It first retrieves unpublished messages through the repository.

The publisher then processes each message independently.

```text
Outbox Table
     │
     ▼
Get unpublished messages
     │
     ▼
For each message
     │
     ▼
Create MessageEnvelope
     │
     ▼
Publish
```

The event type is used as the message routing key when creating the envelope.

---

# 10. Successful Publication

When publication succeeds, the message's `PublishedAt` timestamp is set.

```text
Publish succeeds
       │
       ▼
PublishedAt = DateTime.UtcNow
```

The publisher then saves the updated state.

This allows subsequent processing to distinguish between messages that still need to be published and messages that have already been successfully delivered.

---

# 11. Failure and Retry Information

If publication fails, the publisher does not simply discard the message.

Instead, it records:

* An incremented `RetryCount`
* The exception message in `Error`

The message therefore remains available for another processing attempt.

The current model explicitly stores these fields on `OutboxMessage`.

The resulting behavior is:

```text
              ┌───────────────┐
              │ Outbox Message│
              └───────┬───────┘
                      │
                      ▼
                  Publish
                 /       \
                /         \
            Success       Failure
               │             │
               ▼             ▼
         PublishedAt      RetryCount++
                            Error
                               │
                               ▼
                         Try again later
```

---

# 12. Why Publishing Is Separate

The outbox publisher is intentionally separated from the request that created the job.

Without this separation:

```text
POST /jobs
     │
     ▼
Create Job
     │
     ▼
Publish Message
     │
     ▼
Wait for Broker
     │
     ▼
Return HTTP response
```

The job creation request becomes dependent on the availability and performance of the messaging infrastructure.

With the outbox:

```text
POST /jobs
     │
     ▼
Create Job
     │
     ▼
Persist Job + Outbox
     │
     ▼
Commit
     │
     ▼
HTTP response
     
     ...later...

Outbox Publisher
     │
     ▼
Message Broker
```

The transactional operation is therefore decoupled from downstream message delivery.

---

# 13. End-to-End Example

Consider an employer creating a new job.

### Step 1 — API request

The request reaches the `CreateJob` feature.

### Step 2 — Domain model

The handler creates a `Job` using domain concepts such as `Money`, `JobSite`, `GeoLocation`, `Skill`, and `OrganizationId`.

### Step 3 — Persistence

The job is added through `IJobRepository` and the Unit of Work is committed.

### Step 4 — Domain events

The Unit of Work collects domain events from tracked entities.

### Step 5 — Event handling

The appropriate domain-event handler is resolved and invoked.

### Step 6 — Outbox

The handler adds an `OutboxMessage` to the same `DbContext`.

### Step 7 — Transaction

The job and outbox message are persisted within the same transaction.

### Step 8 — Commit

The transaction commits.

### Step 9 — Outbox processing

The background publisher retrieves unpublished messages.

### Step 10 — Broker

The publisher creates a `MessageEnvelope` and sends it through the configured message publisher.

### Step 11 — Completion

On success, `PublishedAt` is recorded.

The complete flow is therefore:

```text
CreateJobCommand
       │
       ▼
    Job Entity
       │
       ▼
    UnitOfWork
       │
       ├───────────────┐
       ▼               ▼
  SQL Changes      Domain Event
                       │
                       ▼
                  Event Handler
                       │
                       ▼
                  OutboxMessage
                       │
                       ▼
                  SQL Transaction
                       │
                     Commit
                       │
                       ▼
               Outbox Publisher
                       │
                       ▼
                MessageEnvelope
                       │
                       ▼
                 Message Broker
```

---

# 14. Design Responsibilities

One of the important characteristics of the implementation is that responsibilities are separated across several components.

| Component                    | Responsibility                                     |
| ---------------------------- | -------------------------------------------------- |
| `CreateJobCommandHandler`    | Execute the job creation use case                  |
| `UnitOfWork`                 | Coordinate transaction and domain-event processing |
| `IntegrationEventDispatcher` | Resolve and invoke the appropriate event handler   |
| `IOutboxRepository`          | Persist and retrieve outbox messages               |
| `OutboxMessage`              | Represent durable pending-event state              |
| `JobOutboxMessagePublisher`  | Deliver pending messages to the broker             |
| `IMessagePublisher`          | Abstract the actual broker implementation          |

This keeps the business feature from needing to understand the mechanics of message delivery.

---

# 15. Important Architectural Detail

The Hiredaily implementation does **not** have the event handler directly publish to the message broker.

Instead:

```text
Domain Event
     │
     ▼
Event Handler
     │
     ▼
Outbox
     │
     ▼
Database
     │
     ▼
Publisher
     │
     ▼
Broker
```

This distinction is important.

The event handler participates in creating durable application state, while the publisher is responsible for external delivery.

The database therefore acts as the reliable hand-off point between the transactional and asynchronous parts of the system.

---

# 16. Current Trade-offs

The current design provides several benefits:

* Job data and the corresponding outbox message share a transaction.
* Broker availability does not determine whether the job transaction can succeed.
* Failed publications can be retried.
* Messaging infrastructure remains outside the job creation feature.
* Outbox state provides visibility into publication failures.
* The message publisher can remain independent of the domain model.

There are also trade-offs.

The system now has:

* Additional database state
* An asynchronous processing pipeline
* Eventual delivery rather than immediate delivery
* Retry and failure-handling requirements
* Additional infrastructure to operate

These are accepted because reliable asynchronous processing is more important for this workflow than keeping the implementation entirely synchronous.

---

# 17. Future Considerations

The current implementation establishes the fundamental Outbox pattern, but there are areas that can evolve as Hiredaily becomes more production-oriented.

Potential improvements include:

* Explicit retry policies and backoff
* Maximum retry limits
* Dead-letter handling
* Better structured error information
* Idempotent message consumers
* Message processing observability
* Distributed tracing
* Batch processing
* Locking/claiming strategies for concurrent publishers
* More explicit message delivery guarantees

These are intentionally separate concerns from the fundamental transactional guarantee provided by the current implementation.

---

# Related Documentation

* [`../architecture.md`](../architecture.md)
* [`messaging.md`](messaging.md)
* [`job-feed.md`](job-feed.md)
* [`background-processing.md`](background-processing.md)
* [`../decisions/004-outbox.md`](../decisions/004-outbox.md)
