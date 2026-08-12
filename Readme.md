# Hiredaily

> A backend-focused job marketplace designed to connect daily-wage workers with relevant jobs based on skills and location.

Hiredaily is a personal engineering project exploring how to design and build a scalable job-matching platform using **.NET, Domain-Driven Design, modular architecture, asynchronous processing, and polyglot persistence**.

The goal of the project is not just to build a job portal, but to explore the architectural and engineering challenges involved in building a system where transactional workloads, asynchronous processing, and high-volume read workloads have different requirements.

---

## Overview

Hiredaily has two primary users:

* **Employers** — create organizations and publish jobs.
* **Workers** — discover jobs relevant to their skills and location.

A simplified flow looks like:

```text
                 ┌─────────────────┐
                 │     Employer    │
                 └────────┬────────┘
                          │
                     Create Job
                          │
                          ▼
                 ┌─────────────────┐
                 │    Job Module   │
                 └────────┬────────┘
                          │
                    Domain Event
                          │
                          ▼
                 ┌─────────────────┐
                 │     Outbox      │
                 └────────┬────────┘
                          │
                    Integration
                       Event
                          │
                          ▼
                 ┌─────────────────┐
                 │  Message Broker │
                 └────────┬────────┘
                          │
                          ▼
                 ┌─────────────────┐
                 │  Feed Processor │
                 └────────┬────────┘
                          │
                          ▼
                 ┌─────────────────┐
                 │   Cosmos DB     │
                 │   Read Model    │
                 └────────┬────────┘
                          │
                          ▼
                 ┌─────────────────┐
                 │  Worker Feed    │
                 └─────────────────┘
```

The architecture deliberately separates the **transactional job-management workload** from the **job-feed read workload**.

---

## Architecture

Hiredaily is currently designed as a **modular monolith** rather than a collection of microservices.

The major business modules are:

* **Identity** — authentication and user identity
* **Organization** — employer organizations
* **Job** — job creation and management
* **JobFeed** — worker-facing job discovery

The system follows a combination of:

* Domain-Driven Design
* Clean Architecture principles
* Vertical Slice Architecture
* Domain Events
* Integration Events
* CQRS-style separation of read and write workloads
* Transactional Outbox Pattern
* Asynchronous processing

### Why a modular monolith?

The system has multiple business domains, but initially does not require the operational complexity of independently deployed microservices.

A modular monolith allows the project to maintain clear domain boundaries while keeping deployment and development relatively simple.

The architecture also leaves room for individual modules to evolve independently if the system eventually requires service extraction.

---

## Data Architecture

Hiredaily uses different storage technologies for different workloads.

### SQL Server

SQL Server is the transactional source of truth for the write side of the application.

It is used for data where transactional consistency and relational integrity are important, such as:

* Users
* Organizations
* Jobs
* Other transactional domain data

### Cosmos DB

The worker job feed has fundamentally different access patterns from job creation.

Workers need to retrieve jobs efficiently based on factors such as:

* Location
* Skills
* Other feed-related criteria

Rather than forcing the transactional database to serve both workloads, Hiredaily uses **Cosmos DB as a read-optimized feed model**.

This introduces eventual consistency between the transactional model and the worker feed, but allows each datastore to be optimized for its respective workload.

More details can be found in the [architecture documentation](docs/architecture.md).

---

## Asynchronous Processing

Creating a job does not require the entire worker-feed processing pipeline to complete synchronously.

The system uses an asynchronous flow:

```text
Job Transaction
      │
      ├── Job persisted
      │
      └── Outbox message persisted
                    │
                    ▼
             Message Broker
                    │
                    ▼
             Feed Processing
                    │
                    ▼
               Cosmos DB
```

The **Transactional Outbox Pattern** is used to ensure that a business transaction and the corresponding integration event are persisted reliably.

This allows downstream processing to happen asynchronously without making job creation dependent on the availability of the entire feed-processing pipeline.

---

## Messaging

The application uses an abstraction around messaging so that the application is not tightly coupled to a particular broker implementation.

RabbitMQ is currently used for local development, while the architecture is designed with **Azure Service Bus** as the target cloud messaging infrastructure.

This allows the same application-level messaging concepts to be used across development and production environments without coupling the domain or application layers directly to the infrastructure implementation.

---

## Key Engineering Decisions

Some of the key architectural decisions in Hiredaily include:

| Decision                   | Reason                                                                     |
| -------------------------- | -------------------------------------------------------------------------- |
| Modular monolith           | Maintain clear domain boundaries without premature microservice complexity |
| SQL Server                 | Transactional source of truth                                              |
| Cosmos DB                  | Optimize the worker feed for its read/access patterns                      |
| Separate read model        | Prevent feed queries from competing with transactional workloads           |
| Outbox pattern             | Reliably publish integration events after transactions                     |
| Asynchronous processing    | Decouple job creation from feed generation                                 |
| Message broker abstraction | Avoid coupling application logic to a specific broker                      |
| DDD                        | Model business rules and domain boundaries explicitly                      |
| Vertical slices            | Keep features cohesive and reduce unnecessary application-layer coupling   |

Detailed decisions and trade-offs are documented separately in [`docs/`](docs/).

---

## Project Structure

```text
Hiredaily/
│
├── Hiredaily-api/
│   └── Backend implementation
│
├── Hiredaily-web/
│   └── Web application
│
├── docs/
│   ├── architecture.md
│   ├── decisions/
│   └── deep-dives/
│
└── README.md
```

### Backend

The backend contains the domain modules, application logic, infrastructure integrations, persistence, messaging, and background processing.

See [`Hiredaily-api/README.md`](Hiredaily-api/README.md) for backend-specific architecture and implementation details.

### Web

The web application provides the user-facing interface for workers and employers.

See [`Hiredaily-web/README.md`](Hiredaily-web/README.md) for frontend-specific details.

---

## Documentation

The root README intentionally provides only the high-level architecture.

Detailed technical decisions are documented separately:

* [Architecture Overview](docs/architecture.md)
* [Architecture Decision Records](docs/decisions/)
* [Technical Deep Dives](docs/deep-dives/)

The documentation focuses on **why** architectural decisions were made, their alternatives, and their trade-offs rather than simply describing the technologies used.

---

## Technology Stack

### Backend

* .NET
* C#
* ASP.NET Core
* Entity Framework Core
* SQL Server

### Messaging & Processing

* RabbitMQ
* Azure Service Bus *(target production infrastructure)*
* Azure Functions
* Transactional Outbox

### Data

* SQL Server
* Azure Cosmos DB

### Frontend

* React
* TypeScript
* Vite

### Architecture

* Domain-Driven Design
* Modular Monolith
* Clean Architecture
* Vertical Slice Architecture
* Domain & Integration Events
* CQRS-style read/write separation

---

## Current Status

Hiredaily is an **active engineering project** rather than a finished production product.

The core architectural foundations and several infrastructure components have been implemented. Some application flows, frontend functionality, and production infrastructure are still under development.

The project prioritizes demonstrating the engineering decisions and architecture behind the system rather than attempting to implement every possible feature of a production job marketplace.

---

## What I Am Exploring

The main purpose of Hiredaily is to explore practical engineering problems such as:

* How to establish meaningful boundaries inside a modular monolith
* How to model business rules using DDD
* How to reliably publish events from transactional operations
* How to handle eventual consistency
* How to build a read model optimized for a specific access pattern
* How to introduce asynchronous processing without tightly coupling components
* How to design an architecture that can evolve toward independently deployed services when necessary

---

## License

This project is primarily a personal engineering and learning project.
