# Job Feed Architecture

## Overview

The Hiredaily Job Feed is a read-optimized model designed specifically for workers searching for jobs based on **location and skills**.

The transactional Job model and the worker-facing feed have different access patterns. Rather than querying the transactional model directly for every feed request, Hiredaily projects job information into a dedicated **Cosmos DB read model**.

The high-level flow is:

```text
                    Job Module
                        │
                        │ Integration Events
                        ▼
                  Message Broker
                        │
                        ▼
                Feed Module
                        │
                        ▼
              Cosmos DB Read Model
                        │
                        ▼
                  Worker Feed
```

The feed is therefore **eventually consistent** with the transactional Job model.

---

# 1. Why a Separate Read Model?

The transactional Job model is optimized for managing the lifecycle of a job.

The worker feed has a different access pattern.

A worker wants to answer questions such as:

> "Show me available jobs near my location that require my skills."

Trying to make the transactional model serve this workload directly would couple job management and job discovery to the same query model.

Instead, Hiredaily creates a separate read model containing the information required by the feed.

```text
Transactional Model

       Job
        │
        ▼
    SQL Server


Read Model

       Job
        │
        ▼
   Cosmos DB
        │
        ▼
   Worker Feed
```

This allows the feed model to be shaped around its actual queries rather than around the normalized transactional model.

---

# 2. Event-Driven Projection

The feed is populated and updated through integration events.

For example, when a job is created:

```text
Job Created
    │
    ▼
Outbox
    │
    ▼
Message Broker
    │
    ▼
JobCreatedMessageHandler
    │
    ▼
JobFeed
    │
    ▼
Cosmos DB
```

The `JobCreatedMessageHandler` deserializes the incoming message, creates a `JobFeed` representation and inserts it through `IJobFeedRepository`.

This keeps the feed projection independent from the transactional Job module.

---

# 3. The Feed Projection

The initial projection contains the information required by the worker feed, including:

* Job ID
* Organization ID
* Title
* Creation/update timestamps
* Hourly rate
* Job site
* Required skills
* Location information
* Active state

The Cosmos document represents this read-oriented version of the job.

The repository maps a `JobFeed` into a `JobFeedDocument` before storing it in Cosmos DB.

The resulting document contains both business information and feed-specific data such as:

```text
GeoHash
LocationBucket
```

These fields are important for the feed's location-based access pattern.

---

# 4. Why Cosmos DB?

Cosmos DB is used because the feed is fundamentally a **read-oriented workload** with location-based access patterns.

The data can be denormalized specifically for the queries required by the feed.

The transactional model does not need to be structured around these queries.

Instead:

```text
SQL Server
    │
    │ Transactional model
    │
    ▼
Integration Events
    │
    ▼
Cosmos DB
    │
    │ Read-optimized model
    ▼
Worker Feed
```

This is a deliberate separation between the **write model** and **read model**.

---

# 5. Location-Based Partitioning

Location is one of the most important characteristics of the feed.

The Cosmos document stores a location bucket generated from the job's geographic coordinates.

The implementation generates a geohash with a default precision of **7 characters**.

The same generated value is used to create the partition key:

```text
Location
   │
   ▼
Geohash
   │
   ▼
LocationBucket
   │
   ▼
Cosmos Partition Key
```

When a `JobFeedDocument` is created, its `GeoHash` and `LocationBucket` are generated from the job's location.

This means jobs that are geographically close can be grouped into the same logical location buckets.

---

# 6. Geohash Generation

The geohash implementation divides latitude and longitude ranges repeatedly to encode the location into a Base32 string.

The implementation starts with:

```text
Latitude:  -90  → 90
Longitude: -180 → 180
```

and alternates between longitude and latitude while progressively narrowing the ranges.

The resulting hash is used as the location representation.

```text
Latitude + Longitude
        │
        ▼
    Geohash
        │
        ▼
 Location Bucket
```

The default precision is configurable through the `CreateFrom` method, with the current default set to 7.

---

# 7. Querying Nearby Locations

A worker does not necessarily want jobs from exactly the same geohash as their current location.

The repository therefore calculates a collection of nearby location buckets.

```text
             ┌─────┬─────┬─────┐
             │     │     │     │
             │  B  │  B  │  B  │
             ├─────┼─────┼─────┤
             │     │     │     │
             │  B  │  X  │  B  │
             ├─────┼─────┼─────┤
             │     │     │     │
             │  B  │  B  │  B  │
             └─────┴─────┴─────┘

                    X
              Worker location
```

`GetNearbyBuckets` generates candidate locations around the worker's coordinates. The current implementation uses a configurable bucket radius, with a default radius of 1 and a step of 0.1 degrees.

The result is a set of partition keys that can be queried independently.

---

# 8. Skill-Based Filtering

Location alone is not sufficient.

The feed also filters jobs based on the worker's requested skills.

The repository builds a Cosmos query that checks whether a job's `requiredSkills` contains each requested skill.

The query is parameterized rather than directly embedding skill values into the query string.

Conceptually:

```text
Worker
 │
 ├── Location
 │
 └── Skills
       │
       ▼
 ┌───────────────────┐
 │ Location Buckets  │
 └─────────┬─────────┘
           │
           ▼
 ┌───────────────────┐
 │ Skill Filter      │
 └─────────┬─────────┘
           │
           ▼
       Job Feed
```

---

# 9. Partition-Aware Queries

For each nearby location bucket, the repository creates a Cosmos query iterator with that bucket explicitly supplied as the partition key.

The overall process is:

```text
Worker Location
      │
      ▼
Nearby Buckets
      │
      ├──── Bucket A ────► Cosmos
      │
      ├──── Bucket B ────► Cosmos
      │
      ├──── Bucket C ────► Cosmos
      │
      └──── ...
                    │
                    ▼
             Combined Results
```

This keeps the feed query aware of the partitioning strategy rather than treating Cosmos DB as an unpartitioned document store.

---

# 10. Job Updates

The read model is not only created when a job is created.

Changes to the transactional Job model are represented as separate integration events.

Currently the feed has handlers for changes including:

* Hourly rate
* Title
* Required skills
* Job site

For example, an hourly-rate update deserializes the event and updates the corresponding feed document.

Similarly, title updates are applied directly to the feed document.

Required-skill changes are also projected into the read model.

Job-site changes are slightly more interesting because the location can change the document's partition.

---

# 11. Moving a Job Between Partitions

A job's location is part of the Cosmos partitioning strategy.

Therefore, changing the job location can require moving the document to a different partition.

The current implementation:

1. Retrieves the existing document.
2. Stores its previous partition key.
3. Updates the job site.
4. Generates a new geohash.
5. Generates a new location bucket.
6. Upserts the document using the new partition.
7. Deletes the old document from the previous partition.

This behavior is implemented in `UpdateJobSite`.

Conceptually:

```text
Old Location
     │
     ▼
Old Partition
     │
     │ Job Site Updated
     ▼
New Location
     │
     ▼
New Partition
```

The repository's `Save` method performs the upsert and, when the partition has changed, deletes the document from the previous partition.

This is an important consequence of using location as part of the partitioning strategy.

---

# 12. Required Skill Updates

Required skills can also affect the feed's query behavior.

When skills are updated, the repository updates the feed document's required skills and recalculates its location bucket from the document's current job site.

The event-driven projection therefore keeps the read model synchronized with changes to the source Job aggregate.

---

# 13. Read Model vs Domain Model

The Cosmos document is deliberately not treated as the authoritative Job entity.

The responsibilities are different:

| Transactional Model                        | Feed Model                     |
| ------------------------------------------ | ------------------------------ |
| Source of truth                            | Read projection                |
| Optimized for writes/business transactions | Optimized for worker queries   |
| SQL Server                                 | Cosmos DB                      |
| Domain model                               | Denormalized document          |
| Strong transactional consistency           | Eventual consistency           |
| Owns business rules                        | Represents queryable feed data |

This distinction is important.

The Job module remains responsible for the actual business state of a job.

The Feed module represents a projection of that state optimized for a particular read workload.

---

# 14. Eventual Consistency

Because the feed is updated through integration events, it is eventually consistent with the transactional Job model.

For example:

```text
Job Updated
    │
    ▼
SQL Server
    │
    ▼
Outbox
    │
    ▼
Message Broker
    │
    ▼
Feed Handler
    │
    ▼
Cosmos DB
```

There is therefore a period during which:

```text
SQL Server  → new state
Cosmos DB   → previous state
```

This is an intentional trade-off.

The benefit is that the transactional Job workflow does not need to synchronously update the feed before completing.

---

# 15. Failure Boundaries

The feed projection is deliberately separated from the transactional operation.

If the feed processor is temporarily unavailable:

```text
Job API
   │
   ▼
SQL Server ✓
   │
   ▼
Outbox ✓
   │
   ▼
Feed Processor ✗
```

The job transaction can still succeed.

The event remains available through the asynchronous messaging pipeline for later processing.

This is one of the key benefits of combining the read model with the Outbox architecture.

See [`outbox.md`](outbox.md) for the transactional side of this process.

---

# 16. End-to-End Flow

Putting the pieces together:

```text
                 Employer
                    │
                    ▼
               Create Job
                    │
                    ▼
              Job Aggregate
                    │
                    ▼
                SQL Server
                    │
              Domain Event
                    │
                    ▼
                 Outbox
                    │
                    ▼
              Message Broker
                    │
                    ▼
          JobCreatedMessageHandler
                    │
                    ▼
               JobFeed
                    │
                    ▼
                Cosmos DB
                    │
          ┌─────────┴─────────┐
          │                   │
      Location              Skills
          │                   │
          └─────────┬─────────┘
                    ▼
              Worker Feed
```

The important architectural boundary is:

> **The Job module owns the job. The Feed module owns the representation of that job required for worker discovery.**

---

# 17. Trade-offs

The current approach provides several benefits:

* Feed queries can be optimized independently from transactional queries.
* Cosmos documents can be shaped around worker access patterns.
* Location can be used as part of the partitioning strategy.
* Skill filtering can be performed directly against the read model.
* Job updates can be projected independently.
* Job creation does not synchronously depend on feed processing.

There are also costs:

* The system now maintains two representations of job data.
* The read model is eventually consistent.
* Integration events must be reliably delivered.
* Changes to partition-key-related data require additional handling.
* The geospatial bucketing strategy needs to be carefully evaluated as scale increases.

---

# 18. Current Implementation Limitations

The current implementation is intentionally a working exploration rather than a fully production-hardened feed system.

Areas that could evolve include:

* More sophisticated geospatial querying
* More precise distance calculations
* Better ranking of matching jobs
* More advanced skill matching
* Pagination and continuation-token handling
* Explicit handling of duplicate events
* Idempotency guarantees
* Better handling of concurrent updates
* Feed expiration and job deactivation
* Observability around projection lag
* More sophisticated partition sizing and distribution

These are future considerations rather than requirements for the current architecture.

---

# Related Documentation

* [`../architecture.md`](../architecture.md)
* [`outbox.md`](outbox.md)
* [`messaging.md`](messaging.md)
* [`background-processing.md`](background-processing.md)
* [`../decisions/003-cosmos-feed.md`](../decisions/003-cosmos-feed.md)
