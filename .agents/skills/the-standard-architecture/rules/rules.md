# The Standard Architecture — Rules

## BROKERS

**arch-001** [ERROR] Every broker must implement a local interface (e.g., `IStorageBroker`).
**arch-002** [ERROR] Brokers must contain no flow control (no `if`, `switch`, `for`, `while`).
**arch-003** [ERROR] Brokers must not handle exceptions — they propagate raw to the service.
**arch-004** [ERROR] Brokers own their own configuration — connection strings and credentials are injected into the broker only.
**arch-005** [ERROR] Brokers must use native C# primitives, not framework-specific types, in their method signatures where possible.
**arch-006** [ERROR] Brokers use infrastructure language, not business language (e.g., `InsertStudentAsync` not `AddStudentAsync`).
**arch-007** [ERROR] Brokers communicate upward (to services) and sideways (to support brokers like logging). Never to other entity brokers.
**arch-008** [ERROR] One broker wraps exactly one external resource (one database, one API, one queue).
**arch-009** [ERROR] All broker methods must be asynchronous (return `Task` or `ValueTask`).
**arch-010** [ERROR] Entity brokers handle entity CRUD against a resource. Support brokers (Logging, DateTime, Identifier) provide cross-cutting infrastructure.

## FOUNDATION SERVICES

**arch-020** [ERROR] Foundation services are pure-primitive: input and output types are always the same entity type.
**arch-021** [ERROR] Foundation services integrate with exactly one entity type.
**arch-022** [ERROR] Foundation services use business language (Add, Retrieve, Modify, Remove) not infrastructure language (Insert, Select, Update, Delete).
**arch-023** [ERROR] Foundation services must validate all inputs before delegating to a broker.
**arch-024** [ERROR] Structural validations (null, empty, default values) must run first and must break immediately (circuit-breaking).
**arch-025** [ERROR] Logical validations (business rules: date ranges, valid states) run after structural validations.
**arch-026** [ERROR] External validations (existence checks in storage for Modify/Remove) run after logical validations.
**arch-027** [ERROR] Dependency validations cover broker-specific failure conditions.
**arch-028** [ERROR] Continuous validations must collect all invalid fields before throwing, using an upsertable exception data structure.
**arch-029** [ERROR] All exceptions from brokers must be caught, localized into Standard exceptions, categorized, and logged.
**arch-030** [ERROR] Exception categories: ValidationException, DependencyValidationException, CriticalDependencyException, DependencyException, ServiceException.
**arch-031** [ERROR] Foundation services must verify no unwanted broker calls occur after every operation.
**arch-032** [WARNING] Non-local models (external API models) must be mapped to local models before returning from a foundation service.

## PROCESSING SERVICES

**arch-040** [ERROR] Processing services depend on exactly one foundation service.
**arch-041** [ERROR] Processing services validate only the data they actually use — no over-validation.
**arch-042** [ERROR] Processing services implement higher-order logic: Ensure, Upsert, TryAdd, TryRemove patterns.
**arch-043** [ERROR] Processing services implement shifters: transformations from one type to another (entity → bool, entity → count).
**arch-044** [ERROR] Processing services implement combinations: retrieve+add, retrieve+modify (upsert).
**arch-045** [ERROR] Processing services must unwrap foundation exceptions and re-wrap as processing-level exceptions.
**arch-046** [ERROR] Pass-through methods (no processing logic) are allowed and must delegate directly to the foundation service.

## ORCHESTRATION SERVICES

**arch-050** [ERROR] Orchestration services must follow the Florance Pattern: 2-3 foundation or processing service dependencies.
**arch-051** [ERROR] Orchestration services coordinate multi-entity flows across multiple services.
**arch-052** [ERROR] Call order must be enforced explicitly when flow correctness depends on it.
**arch-053** [ERROR] Natural order (enforced by input/output dependencies) is preferred over mock-sequence verification.
**arch-054** [ERROR] Orchestration services must unwrap and re-wrap exceptions from their dependencies.
**arch-055** [WARNING] Orchestration service variations: Coordination (2-3 deps), Management (3+ deps), UberManagement (4+), Unit of Work.

## AGGREGATION SERVICES

**arch-060** [ERROR] Aggregation services must not validate dependency call order.
**arch-061** [ERROR] Aggregation services must not use mock-sequence style assertions.
**arch-062** [ERROR] Aggregation services perform basic structural validations only.
**arch-063** [ERROR] Aggregation services may pass-through to multiple service dependencies without ordering constraints.
**arch-064** [ERROR] Aggregation services must aggregate exceptions from all dependencies.

## EXPOSERS (CONTROLLERS)

**arch-070** [ERROR] Exposers (controllers) contain no business logic — pure mapping only.
**arch-071** [ERROR] Each entity has a single exposure point (one controller per entity).
**arch-072** [ERROR] REST controller routes follow pattern: `/api/[entities]` (plural entity name).
**arch-073** [ERROR] HTTP success codes: 200 for GET/PUT/DELETE, 201 for POST.
**arch-074** [ERROR] Validation errors → HTTP 400. Dependency validation → HTTP 400. Critical dependency → HTTP 500. Dependency → HTTP 500. Service → HTTP 500.
**arch-075** [ERROR] Controllers must not catch exceptions — they map them via a middleware or problem-details handler.

## GENERAL ARCHITECTURE

**arch-080** [ERROR] Services must never call other services at the same layer (no Foundation → Foundation calls).
**arch-081** [ERROR] Services must never call infrastructure directly — only through brokers.
**arch-082** [ERROR] Dependency flow is strictly: Exposer → Service(s) → Broker(s) → External Resource.
**arch-083** [ERROR] Every service type (Foundation, Processing, Orchestration, Aggregation) must be declared and tested independently.
