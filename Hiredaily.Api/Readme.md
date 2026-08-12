# Hiredaily API

The Hiredaily API is the backend of the Hiredaily job marketplace.

It is built with **.NET and C#** and follows a modular architecture designed around business domains rather than technical layers alone.

The API is responsible for:

* Identity and authentication
* Organization management
* Job creation and management
* Job-feed generation and retrieval
* Domain and integration events
* Asynchronous processing
* Transactional persistence
* Communication with external infrastructure

---

## Architecture

The API is structured as a **modular monolith**.

Each business capability is represented as a module with its own domain, application, and infrastructure concerns.

```text
Hiredaily.API/
│
├── src/
│   │
│   ├── BuildingBlocks/
│   │   └── Shared cross-cutting infrastructure
│   │
│   ├── Host/
│   │   └── API host / application composition
|   |   └── Azure Functions / Background Service
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

The modules are designed to have explicit boundaries and avoid directly coupling business logic across modules.

BuildingBlocks

Shared technical building blocks used across modules, such as common abstractions, infrastructure components, messaging, domain primitives, etc.

Important: keep this section focused on why it exists, not a list of every class.

Host

The composition/root of the API — application startup, dependency injection, middleware, endpoint registration, configuration, etc.

Modules

Contains the actual business modules. Each module owns its domain and application functionality and is organized around features.

Modules/
└── Job/
    └── Features/
        ├── CreateJob/
        ├── GetJob/
        └── ...

The architecture combines:

* Domain-Driven Design
* Clean Architecture principles
* Vertical Slice Architecture
* Domain Events
* Integration Events
* CQRS-style separation
* Transactional Outbox Pattern

For a detailed explanation of the overall architecture:

→ [`docs/architecture.md`](../docs/architecture.md)

---

# Modules

## Identity

Responsible for:

* User registration
* Authentication
* Identity-related operations
* Authorization

The Identity module owns identity-related business rules rather than allowing other modules to directly manipulate identity data.

**Deep dive:** [`docs/deep-dives/identity.md`](../docs/deep-dives/identity.md)

---

## Organization

Represents the employer-side organization within Hiredaily.

An organization can create and manage jobs on behalf of its business.

The module owns organization-related business rules and maintains its boundary from other modules.

**Deep dive:** [`docs/deep-dives/organization-module.md`](../docs/deep-dives/organization-module.md)

---

## Job

The Job module manages the lifecycle of jobs posted by organizations.

A simplified flow is:

```text
Create Job
    │
    ▼
Validate Command
    │
    ▼
Execute Domain Logic
    │
    ▼
Persist Job
    │
    ├───────────────┐
    │               │
    ▼               ▼
SQL Server      Domain Event
                    │
                    ▼
                 Outbox
```

The Job module is the transactional source of truth for job data.

**Deep dive:** [`docs/deep-dives/job-module.md`](../docs/deep-dives/job-module.md)

---

# Job Feed

The JobFeed module provides workers with jobs relevant to their requirements.

The feed has different access patterns from the transactional Job module, so it is implemented as a separate read model.

```text
                 Job Module
                     │
                     │ JobCreated
                     ▼
                  Outbox
                     │
                     ▼
              Message Broker
                     │
                     ▼
             Feed Processing
                     │
                     ▼
                Cosmos DB
                     │
                     ▼
                Job Feed API
```

Cosmos DB is used as the feed's read-optimized datastore.

The feed is therefore **eventually consistent** with the transactional job data.

**Deep dive:** [`docs/deep-dives/job-feed.md`](../docs/deep-dives/job-feed.md)

---

# Application Architecture

Within a module, application functionality is organized around **vertical slices**.

Instead of organizing the entire application into global folders such as:

```text
Controllers/
Services/
Repositories/
DTOs/
```

features are kept together around the business operation they implement.

For example:

```text
Job/
├── CreateJob/
│   ├── CreateJobCommand.cs
│   ├── CreateJobHandler.cs
│   ├── CreateJobEndpoint.cs
│   └── ...
│
├── GetJob/
│   ├── GetJobQuery.cs
│   ├── GetJobHandler.cs
│   └── ...
```

This keeps the code required for a feature close together and makes changes more localized.

**Deep dive:** [`docs/deep-dives/vertical-slices.md`](../docs/deep-dives/vertical-slices.md)

---

# Domain Model

Business rules are modeled within the domain rather than being implemented entirely inside API endpoints or infrastructure code.

The domain contains concepts such as:

* Entities
* Value Objects
* Domain Events
* Domain Rules
* Aggregates

For example, concepts such as `Money`, `JobSite`, and `OrganizationId` are modeled as domain concepts rather than passing primitive values throughout the application.

**Deep dive:** [`docs/deep-dives/domain-model.md`](../docs/deep-dives/domain-model.md)

---

# Persistence

The API uses **SQL Server** as the transactional datastore.

Entity Framework Core is used for persistence while keeping domain logic independent of the persistence implementation.

The general dependency direction is:

```text
API
 │
 ▼
Application
 │
 ▼
Domain
 ▲
 │
Infrastructure
```

Infrastructure concerns such as Entity Framework, messaging, and external services remain outside the domain.

**Deep dive:** [`docs/deep-dives/persistence.md`](../docs/deep-dives/persistence.md)

---

# Transactional Outbox

The API uses the **Transactional Outbox Pattern** for reliable event publication.

When a business operation changes transactional state, the corresponding integration event is stored in the same database transaction.

```text
┌─────────────────────────────┐
│       SQL Transaction       │
│                             │
│   Job                       │
│    +                        │
│   OutboxMessage             │
│                             │
└──────────────┬──────────────┘
               │
               ▼
        Outbox Processor
               │
               ▼
         Message Broker
```

This prevents a situation where the job is committed successfully but its corresponding event is lost because message publication failed.

**Deep dive:** [`docs/deep-dives/outbox.md`](../docs/deep-dives/outbox.md)

---

# Messaging

Messaging is abstracted from the application layer.

The current development environment uses **RabbitMQ**, while **Azure Service Bus** is the intended production messaging infrastructure.

The application therefore interacts with messaging through application-level abstractions rather than directly depending on a particular broker.

```text
Application
     │
     ▼
Messaging Abstraction
     │
     ├──────────────► RabbitMQ
     │
     └──────────────► Azure Service Bus
```

This also makes local development simpler without changing the application architecture.

**Deep dive:** [`docs/deep-dives/messaging.md`](../docs/deep-dives/messaging.md)

---

# Background Processing

Asynchronous processing is used for workloads that do not need to be completed as part of the original HTTP request.

Examples include:

* Outbox processing
* Publishing integration events
* Updating the job-feed read model
* Other asynchronous workflows

Azure Functions are used for background processing in the Azure-oriented architecture.

**Deep dive:** [`docs/deep-dives/background-processing.md`](../docs/deep-dives/background-processing.md)

---

# Error Handling

The API uses a Result-oriented approach for expected application failures rather than relying exclusively on exceptions for control flow.

The intention is to distinguish:

* Expected business/application failures
* Validation failures
* Infrastructure failures
* Unexpected programming errors

**Deep dive:** [`docs/deep-dives/error-handling.md`](../docs/deep-dives/error-handling.md)

---

# API Flow Example

A typical job creation request follows this general path:

```text
HTTP Request
     │
     ▼
Endpoint
     │
     ▼
Command
     │
     ▼
Handler
     │
     ▼
Domain
     │
     ├──────────────► SQL Server
     │
     └──────────────► Domain Event
                           │
                           ▼
                        Outbox
                           │
                           ▼
                    Message Broker
                           │
                           ▼
                     Job Feed
```

The important architectural property is that **job creation does not need to synchronously update the worker feed**.

This allows the transactional operation and feed processing to evolve independently.

---

# Technical Deep Dives

The API README intentionally provides an overview rather than documenting every implementation detail.

Detailed technical decisions are maintained separately:

| Document                                                                             | Topic                                       |
| ------------------------------------------------------------------------------------ | ------------------------------------------- |
| [`architecture.md`](../docs/architecture.md)                                         | Overall system architecture                 |
| [`decisions/`](../docs/decisions/)                                                   | Architecture Decision Records               |
| [`deep-dives/domain-model.md`](../docs/deep-dives/domain-model.md)                   | Domain modeling and DDD                     |
| [`deep-dives/vertical-slices.md`](../docs/deep-dives/vertical-slices.md)             | Vertical Slice Architecture                 |
| [`deep-dives/job-feed.md`](../docs/deep-dives/job-feed.md)                           | Feed architecture and Cosmos DB             |
| [`deep-dives/outbox.md`](../docs/deep-dives/outbox.md)                               | Transactional Outbox implementation         |
| [`deep-dives/messaging.md`](../docs/deep-dives/messaging.md)                         | Messaging architecture                      |
| [`deep-dives/background-processing.md`](../docs/deep-dives/background-processing.md) | Azure Functions and asynchronous processing |
| [`deep-dives/persistence.md`](../docs/deep-dives/persistence.md)                     | Persistence architecture                    |
| [`deep-dives/identity.md`](../docs/deep-dives/identity.md)                           | Authentication and authorization            |
| [`deep-dives/error-handling.md`](../docs/deep-dives/error-handling.md)               | Result pattern and error handling           |

These documents explain **why** decisions were made, alternatives that were considered, implementation details, and the trade-offs involved.

---

# Technology Stack

* **.NET**
* **C#**
* **ASP.NET Core**
* **Entity Framework Core**
* **SQL Server**
* **Azure Cosmos DB**
* **RabbitMQ**
* **Azure Service Bus**
* **Azure Functions**
* **Swagger / OpenAPI**

---

# Project Status

The API is under active development.

The architectural foundations are in place, while some application workflows and production infrastructure remain incomplete.

The focus of the project is on demonstrating practical backend engineering and architectural decision-making rather than implementing every possible feature of a production marketplace.

---

# Local Development

> Detailed setup instructions will be maintained here once the development environment is finalized.

At a high level, local development requires:

* .NET SDK
* SQL Server
* RabbitMQ
* Required application configuration

See the project configuration and individual infrastructure documentation for details.
