# Hiredaily Architecture

## 1. Purpose

Hiredaily is a job marketplace designed to connect daily-wage workers with relevant jobs based primarily on **skills and location**.

The system has two fundamentally different workloads:

1. **Transactional workload** — employers and the system create and manage jobs.
2. **Discovery workload** — workers retrieve a potentially large set of jobs optimized around their skills and location.

The architecture is designed around this distinction.

The primary goals are:

* Maintain clear business boundaries.
* Keep the core domain independent of infrastructure.
* Support asynchronous processing.
* Separate transactional and read-heavy workloads.
* Allow infrastructure components to evolve independently.
* Avoid introducing distributed-system complexity before it is necessary.

---

# 2. High-Level Architecture

Hiredaily is currently implemented as a **modular monolith**.

The application is deployed as a single application boundary, while business capabilities are separated into explicit modules.

```text id="f8p2tx"
┌──────────────────────────────────────────────────────────┐
│                    Hiredaily API                          │
│                                                          │
│  ┌──────────┐  ┌──────────────┐  ┌───────┐  ┌─────────┐ │
│  │ Identity │  │ Organization │  │  Job  │  │ JobFeed │ │
│  └──────────┘  └──────────────┘  └───────┘  └─────────┘ │
│                                                          │
└──────────────────────────┬───────────────────────────────┘
                           │
              ┌────────────┴────────────┐
              │                         │
              ▼                         ▼
       ┌─────────────┐          ┌─────────────┐
       │ SQL Server  │          │  Cosmos DB  │
       │ Transaction │          │ Feed Model  │
       └─────────────┘          └─────────────┘
              │
              │ Outbox
              ▼
       ┌─────────────┐
       │   Message   │
       │    Broker   │
       └──────┬──────┘
              │
              ▼
       ┌─────────────┐
       │   Feed      │
       │  Processor  │
       └─────────────┘
```

The important architectural characteristic is that **not every part of the system needs to share the same persistence model or processing model**.

---

# 3. Why a Modular Monolith?

Hiredaily contains several distinct business capabilities, but independently deploying each capability as a microservice would introduce significant operational complexity.

A modular monolith provides:

* Clear business boundaries
* Simple deployment
* Lower infrastructure overhead
* Easier local development
* Easier debugging
* The ability to enforce module boundaries in code

The intention is not to treat the modular monolith as a permanent limitation.

Instead, modules should be designed so that a future extraction into independently deployed services is possible if there is a genuine operational or scaling reason to do so.

### Trade-off

The main trade-off is that all modules initially share the same process and deployment lifecycle.

The architecture therefore prioritizes **logical separation before physical separation**.

---

# 4. Business Modules

The current system is divided into four primary modules:

```text id="i6o2u7"
Modules
│
├── Identity
├── Organization
├── Job
└── JobFeed
```

## Identity

Responsible for user identity and authentication-related concerns.

The module owns identity-specific behavior rather than allowing other modules to directly manipulate identity state.

---

## Organization

Represents organizations that operate on the platform.

Organizations provide the boundary around employer-side job creation and related business concepts.

---

## Job

Responsible for the transactional lifecycle of jobs.

The Job module is the source of truth for job creation and management.

A job creation operation primarily belongs to the transactional side of the system and is persisted to SQL Server.

---

## JobFeed

Responsible for the worker-facing job discovery experience.

The feed has substantially different access patterns from the transactional Job module.

Instead of treating the feed as another query over the transactional model, Hiredaily maintains a dedicated read model optimized for feed retrieval.

---

# 5. Module Boundaries

A module owns its business rules and should not reach directly into another module's internal implementation.

Conceptually:

```text id="q7v0q3"
┌────────────┐
│   Identity │
└──────┬─────┘
       │
       │ explicit contracts
       ▼
┌────────────┐
│Organization│
└──────┬─────┘
       │
       │ explicit contracts / events
       ▼
┌────────────┐
│    Job     │
└──────┬─────┘
       │
       │ integration event
       ▼
┌────────────┐
│  JobFeed   │
└────────────┘
```

The exact mechanism for communication depends on the nature of the interaction.

Synchronous interactions may use explicit application-level contracts, while asynchronous workflows use integration events.

The objective is to avoid creating hidden dependencies such as one module directly accessing another module's database tables or internal services.

---

# 6. Physical Project Structure

The backend repository follows the same architectural boundaries:

```text id="v4l8qk"
Hiredaily.API/
│
├── src/
│   │
│   ├── BuildingBlocks/
│   │
│   ├── Host/
│   │
│   └── Modules/
│       │
│       ├── Identity/
│       │   └── Features/
│       │
│       ├── Organization/
│       │   └── Features/
│       │
│       ├── Job/
│       │   └── Features/
│       │
│       └── JobFeed/
│           └── Features/
│
└── README.md
```

### BuildingBlocks

Contains reusable technical building blocks shared by multiple modules.

These are intended to contain genuinely cross-cutting capabilities rather than becoming a dumping ground for arbitrary application logic.

Examples include shared abstractions and infrastructure concerns.

### Host

The application composition root.

Responsibilities include concerns such as:

* Application startup
* Dependency injection
* Configuration
* Middleware
* Endpoint composition
* Infrastructure registration

The Host should primarily compose the application rather than contain business logic.

### Modules

Contains the business capabilities of Hiredaily.

Each module owns its functionality and is internally organized around features.

---

# 7. Vertical Slice Architecture

Within modules, functionality is organized around **features** rather than global technical layers.

For example:

```text id="5m1l7d"
Job/
└── Features/
    │
    ├── CreateJob/
    │   ├── Command
    │   ├── Handler
    │   ├── Endpoint
    │   └── ...
    │
    ├── GetJob/
    │   ├── Query
    │   ├── Handler
    │   └── ...
    │
    └── ...
```

The intention is to keep the code required for a business operation close together.

This reduces the amount of unrelated code that needs to be understood when modifying a feature.

Vertical slicing is combined with domain boundaries rather than being treated as an alternative to DDD.

**Deep dive:** [`deep-dives/vertical-slices.md`](deep-dives/vertical-slices.md)

---

# 8. Domain-Driven Design

DDD is primarily used to model the **business domain and its boundaries**, rather than simply organizing folders into entities, repositories, and services.

The domain contains concepts such as:

* Entities
* Value Objects
* Aggregates
* Domain Events
* Business rules

Examples of domain concepts include:

* `Money`
* `JobSite`
* `OrganizationId`

These concepts allow business meaning to be represented explicitly instead of spreading primitive values and business rules throughout the application.

The domain should remain independent of infrastructure concerns such as:

* Entity Framework Core
* SQL Server
* RabbitMQ
* Azure Service Bus
* Cosmos DB
* HTTP

**Deep dive:** [`deep-dives/domain-model.md`](deep-dives/domain-model.md)

---

# 9. Application and Domain Dependency Direction

The architecture follows a dependency direction where the domain remains independent from infrastructure.

Conceptually:

```text id="s2o7mi"
                 ┌──────────────┐
                 │     Host     │
                 └───────┬──────┘
                         │
                         ▼
                 ┌──────────────┐
                 │ Application  │
                 └───────┬──────┘
                         │
                         ▼
                 ┌──────────────┐
                 │    Domain    │
                 └──────────────┘
                         ▲
                         │
                 ┌───────┴──────┐
                 │Infrastructure│
                 └──────────────┘
```

Infrastructure implements abstractions required by the application rather than forcing the domain to depend on infrastructure technologies.

This allows domain logic to remain testable and independent of external systems.

---

# 10. Transactional Data

SQL Server acts as the transactional source of truth.

The transactional model is responsible for maintaining the consistency of business operations such as job creation.

A simplified job creation flow is:

```text id="2d7r5k"
HTTP Request
     │
     ▼
Create Job Feature
     │
     ▼
Application Logic
     │
     ▼
Domain
     │
     ├───────────────┐
     │               │
     ▼               ▼
Persist Job      Domain Event
     │               │
     └───────┬───────┘
             ▼
       Outbox Message
```

The job and its corresponding outbox message are persisted as part of the same transaction.

---

# 11. Transactional Outbox

The system uses the Transactional Outbox Pattern to reliably transition from synchronous transactional processing to asynchronous messaging.

Without an outbox, the following failure scenario is possible:

```text id="5i0l3m"
Save Job
   │
   ▼
Database Commit ✓
   │
   ▼
Publish Event ✗
```

The job exists, but downstream systems never receive the event.

With the outbox:

```text id="g7d5r9"
┌─────────────────────────────┐
│       Database Transaction  │
│                             │
│       Job                   │
│       OutboxMessage         │
│                             │
└──────────────┬──────────────┘
               │
            Commit
               │
               ▼
       Outbox Processor
               │
               ▼
        Message Broker
```

The transaction therefore establishes a durable record of the event that needs to be published.

**Deep dive:** [`deep-dives/outbox.md`](deep-dives/outbox.md)

---

# 12. Integration Events

Domain events represent something that happened within a domain.

Integration events are used when that occurrence needs to cross a module or process boundary.

For example:

```text id="n3q4w1"
Job Created
    │
    ▼
Domain Event
    │
    ▼
Outbox
    │
    ▼
Integration Event
    │
    ▼
Message Broker
    │
    ▼
JobFeed
```

This distinction helps prevent infrastructure-level messaging concerns from leaking directly into domain logic.

**Deep dive:** [`deep-dives/messaging.md`](deep-dives/messaging.md)

---

# 13. Read Model Architecture

The worker feed is intentionally separated from the transactional job model.

The transactional model answers questions such as:

> "What jobs exist and what is their current state?"

The feed model answers questions such as:

> "Which jobs should this worker see?"

These are different access patterns.

The architecture therefore uses:

```text id="w2g6p4"
                Write Model
                    │
                 SQL Server
                    │
                    │ Event
                    ▼
              Message Broker
                    │
                    ▼
              Feed Processor
                    │
                    ▼
               Read Model
                    │
                 Cosmos DB
                    │
                    ▼
              Worker Feed API
```

The feed can therefore be optimized independently from the transactional schema.

This is a deliberate form of **CQRS-style separation** rather than introducing CQRS merely as a framework pattern.

**Deep dive:** [`deep-dives/job-feed.md`](deep-dives/job-feed.md)

---

# 14. Eventual Consistency

Because the feed is updated asynchronously, it is not necessarily updated at the exact moment a job transaction commits.

For example:

```text id="6t7s1c"
Job created
    │
    ▼
SQL Server committed
    │
    │
    │   asynchronous processing
    │
    ▼
Feed updated
```

There may therefore be a short period during which a newly created job exists in the transactional model but is not yet visible in the worker feed.

This is an intentional trade-off.

The benefit is that job creation does not depend synchronously on the entire feed-processing pipeline.

---

# 15. Cosmos DB

Cosmos DB is used specifically for the job-feed read model.

The feed's access patterns are expected to involve dimensions such as:

* Worker skills
* Geographic location
* Job availability
* Other feed-specific filtering

The read model can therefore be shaped around those access patterns rather than mirroring the normalized transactional database.

Partitioning and query design are important parts of this architecture and are documented separately.

**Deep dive:** [`deep-dives/job-feed.md`](deep-dives/job-feed.md)

---

# 16. Messaging Architecture

The application uses a messaging abstraction rather than directly coupling business logic to a specific message broker.

Currently:

```text id="7y8p3d"
Local Development
       │
       ▼
   RabbitMQ
```

The target Azure-oriented infrastructure is:

```text id="0f4k2b"
Azure Environment
       │
       ▼
Azure Service Bus
```

The application-level messaging contract remains independent of the underlying broker.

This allows infrastructure to change without requiring business modules to understand broker-specific APIs.

**Deep dive:** [`deep-dives/messaging.md`](deep-dives/messaging.md)

---

# 17. Background Processing

Background processing is used for operations that do not need to block the original HTTP request.

A key example is feed generation.

```text id="w6k3n2"
HTTP Request
     │
     ▼
Create Job
     │
     ▼
Commit Transaction
     │
     ▼
HTTP Response
     
          asynchronous
              │
              ▼
       Outbox Processing
              │
              ▼
       Message Processing
              │
              ▼
        Feed Projection
```

Azure Functions are used as part of the asynchronous processing architecture.

The separation allows independently retryable processing and prevents long-running operations from becoming part of the request lifecycle.

**Deep dive:** [`deep-dives/background-processing.md`](deep-dives/background-processing.md)

---

# 18. Failure and Reliability Considerations

The architecture assumes that asynchronous components can fail independently.

For example:

```text
Job API       ✓
SQL Server    ✓
Outbox        ✓
Broker        ✗
Feed          ✗
```

The job itself should remain successfully committed even if downstream processing is temporarily unavailable.

The outbox provides a durable mechanism for retrying publication.

Similarly, downstream consumers should be designed with the assumption that messages may need to be retried.

This is one of the reasons asynchronous processing is treated as a separate concern rather than being hidden inside the job creation request.

Further reliability considerations are documented in:

* [`deep-dives/outbox.md`](deep-dives/outbox.md)
* [`deep-dives/messaging.md`](deep-dives/messaging.md)

---

# 19. Why Not Microservices?

Microservices were considered but are not the default architecture for Hiredaily.

The primary reason is that **business boundaries and deployment boundaries do not need to be identical from the beginning**.

Starting with microservices would introduce additional requirements such as:

* Multiple deployments
* Service discovery/networking
* Distributed configuration
* Distributed tracing
* More complicated local development
* More complicated testing
* Multiple independently managed infrastructure components

The modular monolith provides many of the organizational benefits of service boundaries while avoiding those costs initially.

If a module eventually develops a strong reason to scale, deploy, or operate independently, it can become a candidate for extraction.

**Deep dive:** [`decisions/001-modular-monolith.md`](decisions/001-modular-monolith.md)

---

# 20. Key Architectural Trade-offs

The architecture deliberately makes several trade-offs.

| Decision                 | Benefit                                 | Trade-off                         |
| ------------------------ | --------------------------------------- | --------------------------------- |
| Modular monolith         | Clear boundaries with simple deployment | Modules share a process           |
| SQL + Cosmos DB          | Optimize different workloads            | Multiple data stores              |
| Separate feed read model | Efficient feed queries                  | Eventual consistency              |
| Outbox                   | Reliable event publication              | Additional infrastructure         |
| Asynchronous processing  | Decoupled workloads                     | More complex failure handling     |
| Message abstraction      | Broker independence                     | Additional abstraction            |
| DDD                      | Explicit business model                 | More modeling effort              |
| Vertical slices          | Feature cohesion                        | Some duplication between features |
| Azure Functions          | Natural fit for background workloads    | Distributed execution model       |

These trade-offs are intentional rather than accidental consequences of the technology choices.

---

# 21. Architectural Evolution

The architecture is intentionally designed to evolve.

A possible future progression is:

```text id="2m8n4v"
                    Current
                      │
                      ▼
              Modular Monolith
                      │
             ┌────────┴────────┐
             │                 │
        Scale workload     Scale team
             │                 │
             ▼                 ▼
      Independent       Independent
       processing        module/service
```

The first priority is establishing meaningful business boundaries.

Physical service separation should only be introduced when there is a concrete reason such as:

* Independent scaling requirements
* Independent deployment requirements
* Team ownership boundaries
* Reliability isolation
* Different infrastructure requirements

---

# 22. Related Documentation

The architecture document intentionally stays at the system level.

Implementation-specific details are documented separately:

### Architecture Decisions

* [`decisions/001-modular-monolith.md`](decisions/001-modular-monolith.md)
* [`decisions/002-sql-server.md`](decisions/002-sql-server.md)
* [`decisions/003-cosmos-feed.md`](decisions/003-cosmos-feed.md)
* [`decisions/004-outbox.md`](decisions/004-outbox.md)
* [`decisions/005-messaging.md`](decisions/005-messaging.md)

### Technical Deep Dives

* [`deep-dives/domain-model.md`](deep-dives/domain-model.md)
* [`deep-dives/vertical-slices.md`](deep-dives/vertical-slices.md)
* [`deep-dives/job-feed.md`](deep-dives/job-feed.md)
* [`deep-dives/outbox.md`](deep-dives/outbox.md)
* [`deep-dives/messaging.md`](deep-dives/messaging.md)
* [`deep-dives/background-processing.md`](deep-dives/background-processing.md)
* [`deep-dives/persistence.md`](deep-dives/persistence.md)
* [`deep-dives/identity.md`](deep-dives/identity.md)

These documents should focus on the **reasoning, alternatives, implementation details, and trade-offs** behind the architecture rather than simply repeating what the code does.
