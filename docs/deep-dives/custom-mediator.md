# Mediator

## Overview

Hiredaily implements its own lightweight **Mediator pattern** to decouple API endpoints and application features from their command/query handlers.

The mediator provides a single entry point for sending commands and requests:

```text
Endpoint
   │
   ▼
IMediatr
   │
   ▼
Pipeline
   │
   ▼
Handler
   │
   ▼
Domain / Application Logic
```

The implementation also provides a pipeline mechanism that can execute cross-cutting behavior before the request reaches its handler.

The current implementation supports:

* Commands
* Requests with responses
* Request/command handlers
* Pipeline behaviors
* Validation
* Result-based responses
* Scoped handler resolution

---

# 1. Why a Mediator?

Without a mediator, an API endpoint can become directly coupled to the concrete application service or handler it needs to execute.

For example:

```text
Endpoint
   │
   ▼
CreateJobHandler
```

With the mediator:

```text
Endpoint
   │
   ▼
IMediatr
   │
   ▼
CreateJobCommand
   │
   ▼
CreateJobHandler
```

The endpoint only needs to know that it has a command and that the mediator can dispatch it.

This fits naturally with Hiredaily's feature-oriented architecture, where each feature owns its request and handler.

---

# 2. Requests and Commands

Hiredaily distinguishes between commands and requests that return a response.

A command implements `ICommand`, which contains common request metadata:

* `RequestId`
* `RequestedAt`
* `RequestedBy`

A request with a response extends `ICommand` through `IRequest<T>`:

```text
ICommand
   │
   └── IRequest<T>
```

Conceptually:

```text
Command
   │
   └── Operation that changes state

Request<T>
   │
   └── Operation that returns a response
```

Both ultimately pass through the mediator.

---

# 3. Handlers

Commands and requests have corresponding handler abstractions.

A command handler receives an `ICommand` and returns an `IResult`.

```text
ICommand
    │
    ▼
ICommandHandler<TCommand>
    │
    ▼
IResult
```

Requests have a generic handler:

```text
IRequest<TResponse>
    │
    ▼
IRequestHandler<TRequest, TResponse>
    │
    ▼
IResult<TResponse>
```

This keeps the handler contract explicit while allowing the mediator to resolve the appropriate implementation dynamically.

---

# 4. IMediatr

The application-facing abstraction is `IMediatr`.

It exposes two `Send` operations:

```text
Send<TRequest, TResponse>(...)
Send<TCommand>(...)
```

The first handles requests that return a response, while the second handles commands.

Application code therefore depends on the abstraction:

```text
IMediatr
```

rather than directly depending on the concrete `Mediatr` implementation.

---

# 5. Dispatching

The concrete `Mediatr` implementation is responsible for resolving the appropriate handler through dependency injection.

For a request, it creates a scope and resolves:

```text
IRequestHandler<TRequest, TResponse>
```

before invoking `Handle`.

Commands follow the same approach:

```text
ICommand
   │
   ▼
Mediatr.Send()
   │
   ▼
Create DI Scope
   │
   ▼
Resolve ICommandHandler<TCommand>
   │
   ▼
Handle()
```

---

# 6. Scoped Handler Resolution

The mediator explicitly creates a dependency-injection scope before resolving the handler.

This is important because application handlers can depend on scoped services such as:

* `DbContext`
* Repositories
* Unit of Work
* Other scoped application services

The mediator therefore does not resolve the handler directly from the root service provider.

For requests and commands, a new scope is created before resolving the corresponding handler.

This keeps the handler and its scoped dependencies within the same lifetime boundary.

---

# 7. Pipeline Behaviors

One of the more interesting parts of the implementation is the pipeline mechanism.

Before resolving and executing a handler, the mediator runs the command through a configurable pipeline.

```text
Command
   │
   ▼
Pipeline
   │
   ├── Validation
   │
   ├── Other behaviors
   │
   └── Handler
```

The mediator calls `RunPipeline` before resolving the handler. If the pipeline returns a failure, the handler is never invoked.

This provides a centralized mechanism for cross-cutting command behavior.

---

# 8. Pipeline Behavior Abstraction

Pipeline behaviors implement `IPipelineBehavior`.

Each behavior exposes:

* A reference to the next behavior
* A `Start` method for executing the behavior

Conceptually:

```text
Behavior A
    │
    ▼
Behavior B
    │
    ▼
Behavior C
    │
    ▼
Handler
```

Each behavior can perform work before passing execution to the next behavior.

---

# 9. Building the Pipeline

`BehaviorCollection` constructs the behavior chain.

When a behavior is added:

1. It is resolved from dependency injection.
2. If it is the first behavior, it becomes the first node.
3. Otherwise it is attached to the current behavior.
4. The current pointer is moved to the newly added behavior.

The resulting structure is effectively a linked chain:

```text
First
  │
  ▼
Behavior 1
  │
  ▼
Behavior 2
  │
  ▼
Behavior 3
  │
  ▼
...
```

`IBehaviorCollection` exposes the ability to add behaviors and retrieve the first behavior in the chain.

---

# 10. Pipeline Configuration

Pipeline composition is separated from pipeline execution.

`IBehaviorConfiguration` provides a configuration boundary:

```text
IBehaviorConfiguration
        │
        ▼
Configure(IBehaviorCollection)
```

`PipelineStartup` receives both the behavior collection and configuration and builds the pipeline during construction.

The resulting pipeline can then be started with a command.

```text
PipelineStartup
      │
      ▼
First Behavior
      │
      ▼
Next Behavior
      │
      ▼
...
```

This separates **how the pipeline is configured** from **how it is executed**.

---

# 11. Validation Pipeline

Validation is currently implemented as a pipeline behavior.

`ValidationPipelineBehavior` receives an `IServiceScopeFactory` and resolves the validator for the specific command type.

The flow is:

```text
Command
   │
   ▼
ValidationPipelineBehavior
   │
   ▼
IValidator<TCommand>
   │
   ▼
ValidateAsync()
```

The validator abstraction is specifically constrained to commands.

If validation fails, the pipeline immediately returns a failure result.

If validation succeeds, execution continues to the next behavior.

---

# 12. Validation Result

Validation produces a `ValidationResult`.

It contains:

* `IsValid`
* A collection of `ValidationError`

Each `ValidationError` contains:

* `PropertyName`
* `ErrorMessage`

This allows validation failures to be represented structurally rather than as exceptions.

For example:

```text
ValidationResult
│
├── IsValid = false
│
└── Errors
    ├── PropertyName
    └── ErrorMessage
```

---

# 13. Result-Based Execution

The mediator uses `IResult` as the common result abstraction.

A result contains:

* `IsSuccess`
* `Error`
* `ValidationResult`

A generic `IResult<T>` additionally contains the response.

The concrete `Result` implementation provides explicit success and failure states. `Result<T>` extends this with a typed response.
This gives the mediator a consistent way to propagate pipeline failures or handler results.

---

# 14. Complete Command Flow

A command therefore follows this path:

```text
HTTP Request
     │
     ▼
Endpoint
     │
     ▼
ICommand
     │
     ▼
IMediatr.Send()
     │
     ▼
PipelineStartup
     │
     ▼
ValidationPipelineBehavior
     │
     ▼
IValidator<TCommand>
     │
     ├──── Invalid ────► Result.Failure()
     │
     ▼
Next Behavior
     │
     ▼
ICommandHandler<TCommand>
     │
     ▼
Application / Domain Logic
     │
     ▼
IResult
```

If validation fails, execution stops before the handler is resolved/executed.

---

# 15. Request Flow

Requests that return a response follow a similar path:

```text
HTTP Request
     │
     ▼
IRequest<TResponse>
     │
     ▼
IMediatr.Send<TRequest,TResponse>()
     │
     ▼
Pipeline
     │
     ▼
IRequestHandler<TRequest,TResponse>
     │
     ▼
IResult<TResponse>
```

The current mediator implementation runs the pipeline and then resolves the request handler inside a newly created DI scope.

---

# 16. Why Build a Custom Mediator?

The project uses a custom implementation rather than directly depending on a third-party mediator library.

The current implementation is intentionally small and tailored to Hiredaily's requirements.

It provides the capabilities currently needed by the application:

* Command dispatching
* Request dispatching
* Handler resolution
* Pipeline behaviors
* Validation
* Result propagation
* Scoped dependency resolution

This also makes the mechanics of the mediator explicit within the project rather than hiding them behind a library.

The trade-off is that Hiredaily is responsible for maintaining and evolving this infrastructure itself.

---

# 17. Relationship to Vertical Slices

The mediator is particularly useful with Hiredaily's feature-oriented structure.

A feature can contain:

```text
CreateJob/
├── CreateJobCommand
├── CreateJobCommandHandler
├── Validator
└── Endpoint
```

The endpoint does not need to know how the handler is resolved.

Instead:

```text
CreateJobEndpoint
       │
       ▼
CreateJobCommand
       │
       ▼
IMediatr
       │
       ▼
CreateJobCommandHandler
```

This keeps the feature cohesive while the mediator provides the dispatching mechanism.

---

# 18. Design Trade-offs

### Benefits

* Decouples endpoints from handlers.
* Provides a consistent command/request execution model.
* Centralizes cross-cutting behavior.
* Allows validation to happen before handler execution.
* Keeps scoped handler dependencies within an explicit scope.
* Fits naturally with feature-oriented application code.
* Keeps the implementation small and understandable.

### Trade-offs

* Adds an additional abstraction between the endpoint and handler.
* Requires maintaining custom mediator infrastructure.
* Pipeline ordering becomes an architectural concern.
* Creating scopes during dispatch introduces additional lifetime-management considerations.
* A custom implementation does not automatically provide the ecosystem and features of a mature third-party mediator library.

---

# 19. Future Considerations

As Hiredaily evolves, the mediator could potentially support additional behaviors such as:

* Logging
* Performance measurement
* Transaction management
* Authorization
* Idempotency
* Correlation IDs
* Distributed tracing

The existing pipeline architecture provides a natural extension point for such cross-cutting concerns.

The important consideration would be to add behaviors only where they represent genuinely cross-cutting application concerns rather than turning the pipeline into a replacement for ordinary application logic.

---

# Related Documentation

* [`../architecture.md`](../architecture.md)
* [`domain-model.md`](domain-model.md)
* [`vertical-slices.md`](vertical-slices.md)
* [`outbox.md`](outbox.md)
