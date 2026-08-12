# Messaging Architecture

## Overview

Hiredaily uses asynchronous messaging to decouple transactional operations from downstream processing.

The messaging architecture is designed around an application-level abstraction so that business and application code does not need to depend directly on a particular message broker.

The current infrastructure contains implementations for:

* RabbitMQ
* Azure Service Bus

The high-level architecture is:

```text
Application
     │
     ▼
IMessagePublisher
     │
     ├──────────────► RabbitMqPublisher
     │
     └──────────────► ServiceBusPublisher
```

Consumers are similarly represented through an application-level `IMessageConsumer` abstraction.

This allows the messaging infrastructure to change without requiring the application features themselves to understand broker-specific APIs.

---

# 1. Why an Abstraction?

A business feature should not need to know whether a message is being sent through RabbitMQ or Azure Service Bus.

For example, the Outbox publisher only depends on `IMessagePublisher`.

```text
Outbox Publisher
       │
       ▼
IMessagePublisher
       │
       ├──── RabbitMQ
       │
       └──── Azure Service Bus
```

The abstraction provides a boundary between the application and infrastructure.

The application therefore deals with concepts such as:

* Publishing a message
* Consuming a message
* Message payload
* Message metadata

while the infrastructure deals with:

* Connections
* Channels
* Exchanges
* Queues
* Topics
* Broker-specific message types

---

# 2. Application Messaging Contracts

The core publishing abstraction is deliberately small:

```text
IMessagePublisher
    │
    └── PublishAsync(MessageEnvelope)
```

`IMessagePublisher` exposes a single `PublishAsync` operation that accepts a `MessageEnvelope`.

Similarly, consumers implement:

```text
IMessageConsumer
    │
    └── SubscribeAsync(...)
```

The consumer abstraction accepts a handler that receives the message payload, routing information, and cancellation token.

The small interfaces are intentional: application code does not need to know about RabbitMQ channels or Azure Service Bus senders.

---

# 3. Message Envelope

The `MessageEnvelope` represents the information required by the infrastructure to publish a message.

It contains:

* `Payload`
* Optional payload metadata

```text
MessageEnvelope
├── Payload
└── PayloadMetadata
```

The current implementation defines the envelope as a record containing the payload string and an optional dictionary of metadata.

This allows broker-specific information to remain outside the actual message payload.

For example, the Outbox publisher currently places the event type into metadata as the routing key before publishing.

Conceptually:

```text
MessageEnvelope
│
├── Payload
│
└── Metadata
       │
       └── routing-key
```

---

# 4. Message Contract

The application defines an `IMessage` abstraction containing common message metadata:

* `CreatedAt`
* `EventId`
* `EventName`

A generic `IMessage<T>` additionally exposes strongly typed `EventData`.

The payload itself is represented by `IMessagePayload`, which currently acts as a marker interface.

The intended conceptual structure is:

```text
IMessage
│
├── CreatedAt
├── EventId
└── EventName
       │
       ▼
IMessage<T>
│
└── EventData
       │
       ▼
IMessagePayload
```

This provides a common contract for integration messages while allowing individual events to carry their own payload types.

---

# 5. Generic Integration Event Metadata

Hiredaily also contains a lightweight `IntegrationEvent` type.

Its purpose is to allow common information about an integration event to be read without knowing the concrete event type.

It contains:

* `CreatedAt`
* `EventId`
* `EventName`

The source code explicitly describes this as supporting deserialization of the event name and ID without knowing the exact integration-event type.

This provides a metadata-level representation of an integration event independently of its concrete payload.

---

# 6. Domain Event Dispatching

The messaging architecture is related to, but distinct from, domain-event dispatching.

Domain-event handlers implement:

```text
IDomainEventHandler<T>
```

with a single `Handle(T evt)` operation.

The `IntegrationEventDispatcher` resolves the handler corresponding to the concrete event type through dependency injection and invokes it dynamically.

Conceptually:

```text
Domain Event
     │
     ▼
IntegrationEventDispatcher
     │
     ▼
IDomainEventHandler<T>
```

This is the application-level event handling mechanism.

The actual broker interaction happens later through the Outbox and `IMessagePublisher`.

---

# 7. Messaging and the Outbox

Messaging is deliberately separated from the transactional operation.

The complete flow is:

```text
Domain Operation
       │
       ▼
Domain Event
       │
       ▼
Event Handler
       │
       ▼
Outbox Message
       │
       ▼
Database Transaction
       │
       ▼
Outbox Publisher
       │
       ▼
IMessagePublisher
       │
       ▼
Broker
```

The Outbox therefore provides the reliable hand-off between the transactional part of the application and the messaging infrastructure.

See [`outbox.md`](outbox.md) for the detailed transactional behavior.

---

# 8. RabbitMQ

RabbitMQ is currently implemented as one of the messaging infrastructure providers.

A `RabbitMqConnectionFactory` creates RabbitMQ connections using the configured host, port, username, and password.

The configuration also contains:

* Exchange
* Queue

along with the connection settings.

The application therefore has a dedicated infrastructure boundary for RabbitMQ rather than exposing RabbitMQ client types to application features.

---

# 9. RabbitMQ Publisher

`RabbitMqPublisher` implements `IMessagePublisher`.

When publishing, it:

1. Validates the configured exchange.
2. Validates the payload.
3. Obtains the routing key from message metadata.
4. Creates a RabbitMQ channel.
5. Encodes the payload as UTF-8.
6. Publishes it to the configured exchange using the routing key.

The broker-specific implementation is therefore isolated behind the common publisher abstraction.

The flow is:

```text
IMessagePublisher
       │
       ▼
RabbitMqPublisher
       │
       ▼
RabbitMQ Channel
       │
       ▼
Exchange
       │
       ▼
Routing Key
```

---

# 10. Routing Keys

Routing information is carried separately from the message payload.

The current Outbox publisher places the event type into the envelope metadata under the `routing-key` key.

RabbitMQ then uses that value when publishing the message to the configured exchange.

This creates a useful separation:

```text
Payload
   │
   └── Business event data

Metadata
   │
   └── Transport information
          │
          └── routing-key
```

The application can therefore carry routing information without embedding broker-specific concepts inside the business payload.

---

# 11. RabbitMQ Consumer

The RabbitMQ consumer also implements the common `IMessageConsumer` abstraction.

It creates a channel and declares the configured queue as:

* Durable
* Non-exclusive
* Non-auto-delete

It then registers an asynchronous consumer.

Messages are decoded from UTF-8 and passed to the application-level handler together with the routing key.

Conceptually:

```text
RabbitMQ Queue
      │
      ▼
RabbitMqConsumer
      │
      ▼
IMessageConsumer handler
      │
      ▼
Application Handler
```

---

# 12. Message Acknowledgement

The RabbitMQ consumer uses explicit acknowledgement rather than automatic acknowledgement.

The consumer is configured with:

```text
autoAck = false
```

and acknowledges the message only after the application handler completes successfully.

The success path is:

```text
Receive
   │
   ▼
Process
   │
   ▼
Handler succeeds
   │
   ▼
ACK
```

If processing throws an exception, the consumer logs the error and sends a negative acknowledgement with requeue disabled.

The failure path is therefore:

```text
Receive
   │
   ▼
Process
   │
   ✗
   ▼
NACK
   │
   ▼
No requeue
```

The current behavior is an important operational characteristic of the implementation and should be considered when designing retries and dead-letter handling.

---

# 13. Azure Service Bus

Azure Service Bus is also implemented behind `IMessagePublisher`.

`ServiceBusPublisher` receives an Azure `ServiceBusClient` and creates a sender for the `"jobs"` entity.

It then creates an Azure `ServiceBusMessage` from the envelope payload and sends it through the Azure SDK.

The Service Bus configuration contains:

* Connection string
* Topic

as the configured messaging settings.

The important architectural point is that application code still sees:

```text
IMessagePublisher
```

rather than `ServiceBusSender`.

---

# 14. RabbitMQ vs Azure Service Bus

The two implementations share the same application-level publishing abstraction:

```text
                 IMessagePublisher
                  /              \
                 /                \
                ▼                  ▼
       RabbitMqPublisher    ServiceBusPublisher
                │                  │
                ▼                  ▼
            RabbitMQ          Azure Service Bus
```

This allows the infrastructure provider to vary without requiring changes to the application-level publisher.

The implementations are not identical internally because each broker has different concepts and APIs.

That is intentional.

The abstraction hides those infrastructure details at the application boundary while allowing each provider to use its native SDK.

---

# 15. Local Development and Production Infrastructure

RabbitMQ provides a convenient messaging infrastructure for local development.

The application also contains an Azure Service Bus implementation for the Azure-oriented environment.

This gives Hiredaily the following separation:

```text
Development
     │
     ▼
RabbitMQ


Azure Environment
     │
     ▼
Azure Service Bus
```

The business workflow remains based on the same application-level messaging abstractions.

---

# 16. Consumer Dispatch

The consumer abstraction does not require the RabbitMQ implementation to understand individual business events.

The consumer receives:

```text
Payload
Routing Key
CancellationToken
```

and passes them to the configured application handler.

This creates another useful boundary:

```text
Broker
  │
  ▼
Transport Consumer
  │
  ▼
Payload + Routing Key
  │
  ▼
Application Handler
```

The transport layer therefore focuses on receiving and acknowledging messages, while application handlers focus on interpreting and processing them.

---

# 17. Job Feed Example

The Job Feed is an example of how these messaging abstractions are used in the application.

The flow is:

```text
Job Module
    │
    ▼
Domain Event
    │
    ▼
Outbox
    │
    ▼
IMessagePublisher
    │
    ▼
RabbitMQ / Azure Service Bus
    │
    ▼
IMessageConsumer
    │
    ▼
JobCreatedMessageHandler
    │
    ▼
Cosmos DB
```

The Feed module then maintains its own read model based on these messages.

See [`job-feed.md`](job-feed.md) for the complete projection architecture.

---

# 18. Design Responsibilities

The messaging system separates responsibilities across several abstractions.

| Component                    | Responsibility                         |
| ---------------------------- | -------------------------------------- |
| `IMessage`                   | Common message metadata                |
| `IMessage<T>`                | Message metadata plus typed event data |
| `IMessagePayload`            | Marker for message payload types       |
| `MessageEnvelope`            | Transport payload and metadata         |
| `IMessagePublisher`          | Application-level message publication  |
| `IMessageConsumer`           | Application-level message consumption  |
| `RabbitMqPublisher`          | RabbitMQ-specific publication          |
| `RabbitMqConsumer`           | RabbitMQ-specific consumption          |
| `RabbitMqConnectionFactory`  | RabbitMQ connection creation           |
| `ServiceBusPublisher`        | Azure Service Bus-specific publication |
| `IntegrationEventDispatcher` | Resolves and invokes event handlers    |

This keeps the application layer relatively independent from the broker SDKs.

---

# 19. Trade-offs

The messaging abstraction provides several benefits:

* Business code is not coupled directly to RabbitMQ.
* Azure Service Bus can be introduced without changing the application-level publisher contract.
* Local development can use RabbitMQ.
* Broker-specific APIs remain in infrastructure.
* Transport metadata can be kept separate from payload data.
* Consumers can acknowledge messages only after processing succeeds.

The abstraction also introduces some costs:

* Another layer between the application and broker.
* The common interface must represent concepts shared by different brokers.
* Broker-specific capabilities cannot always be exposed directly through a generic abstraction.
* Some operational behavior still needs to be handled differently for different brokers.

The abstraction is therefore intentionally small rather than attempting to create a universal messaging framework.

---

# 20. Current Limitations and Future Considerations

The current implementation establishes the core messaging abstraction, but several areas can evolve as the system becomes more production-oriented.

Potential areas include:

* Explicit retry policies
* Dead-letter handling
* Message deduplication
* Consumer idempotency
* Message ordering requirements
* Correlation IDs
* Distributed tracing
* Message versioning
* Schema evolution
* Broker-specific delivery guarantees
* Better separation of routing and event naming
* More explicit consumer lifecycle management

In particular, the current RabbitMQ consumer disables requeue when processing fails.

That means retry/dead-letter behavior will need to be considered as part of the broader production messaging design rather than assumed to be provided by the current consumer implementation.

---

# Related Documentation

* [`../architecture.md`](../architecture.md)
* [`outbox.md`](outbox.md)
* [`job-feed.md`](job-feed.md)
* [`background-processing.md`](background-processing.md)
* [`../decisions/005-messaging.md`](../decisions/005-messaging.md)
