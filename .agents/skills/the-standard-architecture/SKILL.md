---
name: The Standard Architecture
description: Enforces Standard-compliant modeling, brokers, services, aggregation, exposers, REST APIs, and UI architecture.
the standard version: v2.12.0
skill version: v0.3.0.0
---

# The Standard Architecture

## What this skill is

This skill operationalizes the architecture chapters of The Standard.
It governs how systems are decomposed, how responsibilities are assigned, and how dependencies flow.
It includes the architecture of models, brokers, services, aggregation, exposers, communication protocols, APIs, and user interfaces.

## Explicit coverage map

This skill explicitly covers:

- 0.1.2 Modeling
  - Data Carrier Models
    - Primary Models
    - Secondary Models
    - Relational Models
    - Hybrid Models
  - Operational Models
    - Integration Models (Brokers)
    - Processing Models (Services)
    - Exposure Models (Exposers)
  - Configuration Models
- 1 Brokers
  - Introduction
  - On The Map
  - Characteristics
    - Implements a Local Interface
    - No Flow Control
    - No Exception Handling
    - Own Their Configurations
    - Natives from Primitives
    - Naming Conventions
    - Language
    - Up & Sideways
    - One Resource, One Broker
  - Organization
  - Broker Types
    - Entity Brokers
    - Support Brokers
  - Implementation
    - Asynchronization Abstraction
  - FAQs / Clarifications
  - Standardized Provider Abstraction Libraries (SPAL)
    - Extensibility
    - Configurability
    - Distributability
    - External Mockability (Cloud-Foreign)
    - Local to Global
- 2 Services
  - Introduction
  - Services Operations
    - Validations
    - Processing
    - Integration
  - Services Types
    - Validators
    - Orchestrators
    - Aggregators
  - Overall Rules
    - Do or Delegate
    - Two-Three (Florance Pattern)
    - Single Exposure Point
    - Same or Primitives I/O Model
    - Every Service for Itself
    - Flow Forward
    - For APIs
  - Foundation Services (Broker-Neighboring)
    - Introduction
    - On The Map
    - Characteristics
      - Pure-Primitive
      - Single Entity Integration
      - Business Language
    - Responsibilities
      - Abstraction
      - Validation
        - Circuit-Breaking Validations
        - Continuous Validations
        - Upsertable Exceptions
        - Dynamic Rules
        - Rules & Validations Collector
        - Hybrid Continuous Validations
        - Structural Validations
        - Logical Validations
        - External Validations
        - Dependency Validations
      - Mapping
        - Non-Local Models
      - Exception Handling
        - Exceptions Mappings
          - Localise External Exceptions
          - Caragorize Exceptions
        - Logging
  - Processing Services (Higher-Order Business Logic)
    - Introduction
    - On The Map
    - Characteristics
      - Language
      - Functions Language
      - Pass-Through
      - Class-Level Language
      - Dependencies
      - One-Foundation
      - Used-Data-Only Validations
    - Responsibilities
      - Higher-Order Logic
      - Shifters
      - Combinations
      - Signature Mapping
      - Non-Exception Local Models
      - Exception Handling      
        - Unwrap and Localise Foundation Exceptions
        - Caragorize Exceptions
        - Logging
  - Orchestration Services (Complex Higher Order Logic)
    - Introduction
    - On The Map
    - Characteristics
      - Language
      - Functions Language
      - Pass-Through
      - Class-Level Language
      - Dependencies
      - Dependency Balance (Florance Pattern)
      - Two-Three
      - Full-Normalization
      - Semi-Normalization
      - No-Normalization
      - Meaningful Breakdown
      - Contracts
        - Physical Contracts
        - Virtual Contracts
      - Cul-De-Sac
    - Responsibilities
      - Advanced Logic
      - Flow Combinations
      - Call Order
        - Natural Order
        - Enforced Order
      - Exception Handling      
        - Unwrap and Localise Foundation Exceptions
        - Caragorize Exceptions
        - Logging
    - Variations
      - Coordination
      - Management
      - Uber Management
      - Unit of Work
  - Aggregation Services (The Knot)
    - Introduction
    - On The Map
    - Characteristics
      - No Dependency Limitation
      - No Order Validation
      - Basic Validations
      - Pass-Through
      - Optionality
      - Routine-Level Aggregation
      - Pure Dependency Contracts
    - Responsibilities
      - Abstraction
      - Exceptions Aggregation
- 3 Exposers
  - Introduction
  - Purpose
  - Pure Mapping
  - Types of Exposure Components
    - Communication Protocols
    - User Interfaces
    - I/O Components
  - Single Point of Contact
  - Summary
- 3.1 Communication Protocols
  - Principles & Rules
    - Results Communication
    - Error Reports
  - RESTful APIs
    - Language
    - Beyond CRUD Routines
    - Similar Verbs
    - Route Conventions
      - Nouns & Verbs
      - Controller Routes
      - Routine/Method-Specific Routes
      - Plural-Singular-Plural
      - Query Parameters & OData
      - X-WWW-Form-UrlEncoded Parameters
    - Codes & Responses
      - Success Codes (2xx)
      - User Error Codes (4xx)
      - System Error Codes (5xx)
      - All Codes
    - Single Dependency
    - Single Contract
    - Organization
    - Home Controller
- 3.2 User Interfaces
  - Principles & Rules
    - Progress (Loading)
      - Basic Progress
      - Remaining Progress
      - Detailed Progress
    - Results
      - Simple
      - Partial Details
      - Full Details
    - Error Reports
      - Informational
      - Referential / Implicit Actions
      - Actionable
    - Single Dependency
    - Anatomy
      - Bases
      - Components
      - Containers
    - UI Component Types
      - Web Applications
      - Mobile Applications
      - Other Types
  - Web Applications
    - Anatomy
      - Base Component
        - Implementation
        - Utilization
        - Restrictions
      - Core Component
        - Elements
          - Existence
            - Property Assignment
            - Searching by Id
            - General Search
          - Properties
          - Actions
        - Styles
        - Actions
        - Restrictions
      - Pages
      - Unobtrusiveness
      - Organization

## When to use

Use this skill for system design, architecture review, decomposition, refactoring, API design, UI architecture, dependency-flow design, or when deciding where behavior belongs.

## System-wide architecture rules

0. Every system must be decomposed into purpose, dependency, and exposure.
1. At low level, that means:
   - Brokers = dependencies
   - Services = purpose
   - Exposers = exposure
2. Keep the flow forward:
   - Exposer -> Service -> Broker -> External resource
3. Never reverse the flow.
4. Never blur layer boundaries.
5. Never hide architecture behind magical abstractions.

## Modeling rules

### Data-carrier models

0. Primary models are pillars of the system.
   - They are physically self-sufficient.
1. Secondary models depend on primary models.
2. Relational models connect primary models and should mainly hold references.
3. Hybrid models are allowed only when a relationship itself must carry state or details.
4. Keep physical models anemic and flat.
5. Use virtual contracts only when orchestration truly needs them.

### Operational models

0. Integration models are brokers.
1. Processing models are services.
2. Exposure models are exposers.
3. Configuration models compose the system and manage startup, DI, middleware, or platform-specific configuration.

## Broker rules

0. A broker is a liaison between business logic and the outside world.
1. Brokers must implement a local interface.
2. Brokers must contain no business logic.
3. Brokers must contain no flow control.
   - No if statements for business decisions.
   - No loops for business decisions.
   - No switch cases for business decisions.
4. Brokers must not handle exceptions.
   - Let native exceptions propagate to broker-neighboring services.
5. Brokers must own their configurations.
6. Brokers may construct native models from primitive inputs.
7. Brokers must speak the language of their technology.
   - Storage -> Insert / Select / Update / Delete
   - Queue -> Enqueue / Dequeue
   - REST -> Get / Post / Put / Delete / custom HTTP method when needed
8. Brokers cannot call other brokers.
9. Brokers cannot depend on services.
10. One resource, one broker.
11. Use support brokers for generic capabilities such as time and logging.
12. Use entity brokers / api brokers for resource- or entity-specific integrations.
13. Prefer partial interfaces and partial classes to organize multi-entity brokers.
14. Prefer generic helper methods in broker root partials so entity partials do not touch native clients directly.
15. Use asynchronous abstractions consistently.
16. Prefer ValueTask in Standard examples and abstractions when that aligns with the implementation profile.
17. Brokers live under Brokers/ and their namespaces.
18. Broker configurations live in appsettings.json or equivalent configuration stores
19. Broker configuration classes live under Brokers/ and their namespaces.

### Asynchronization Abstraction (§1.5.1)

Every publicly exposed interface method — on brokers, services, and exposers — must
return `ValueTask` or `ValueTask<T>`, **even if the current implementation does not
internally await anything**.

This is The Standard §1.5.1: the public contract is uniformly async so callers never
need to change if an implementation later becomes truly asynchronous.

| Pattern | Verdict |
|---|---|
| `public async ValueTask LogWarningAsync(string message) => this.logger.LogWarning(message);` | **Correct** — async keyword, direct call |
| `public ValueTask<IQueryable<Student>> SelectAllStudentsAsync() => ValueTask.FromResult(...)` | **Correct** — ValueTask.FromResult wraps sync result |
| `public async ValueTask<IQueryable<Student>> SelectAllStudentsAsync() => await this.SelectAllAsync<Student>();` | **Correct** — async/await through generic helper |
| `public IQueryable<Student> SelectAllStudents() => this.Students.AsNoTracking();` | **WRONG** — synchronous, no ValueTask |
| `public ValueTask LogWarningAsync(string message) => new ValueTask(Task.Run(() => ...));` | **WRONG** — Task.Run wraps a synchronous call needlessly |

**Consequence for services:** `CreateAndLog*` helpers must be async too, because
`ILoggingBroker.LogErrorAsync` returns `ValueTask`. Catch blocks must use `throw await`:

```csharp
// WRONG
private StudentValidationException CreateAndLogValidationException(Xeption exception)
{
    ...
    this.loggingBroker.LogError(studentValidationException);  // no such sync method
    return studentValidationException;
}

// CORRECT
private async ValueTask<StudentValidationException> CreateAndLogValidationException(
    Xeption exception)
{
    var studentValidationException = new StudentValidationException(...);
    await this.loggingBroker.LogErrorAsync(studentValidationException);
    return studentValidationException;
}

// CORRECT call site in TryCatch
catch (NullStudentException nullStudentException)
{
    throw await CreateAndLogValidationException(nullStudentException);
}
```

### Broker clarifications

0. Brokers are broader than repositories.
1. Providers are not the same as brokers.
2. Native exceptions may leak through brokers by design; foundation services localize them.
3. Partialization is preferred for multi-entity brokers because configuration ownership stays centralized.
4. Suppress warnings at the project level when truly needed; otherwise fix them.

### SPAL rules

0. Standardized Provider Abstraction Libraries must be extensible.
1. They must be configurable.
2. They must be distributable.
3. They must support external mockability for local / airplane-mode operation.
4. They should move from local need to global reusable library when possible.
5. They are subsystems and should themselves follow Brokers / Services / Exposers.

## Service-wide rules

0.  Services contain business logic.
1.  Service operations break into validations, processing, and integration.
2.  Service types break into validators, orchestrators, and aggregators.
3.  Do or delegate, but not both.
4.  Enforce Florance Pattern where applicable.
5.  Exposure layers must have a single point of contact with business logic.
6.  Services should accept and return the same contract or primitives/aggregations of that contract.
      - Methods that has primitive inputs must consider using a contract model count exceed three.
7.  Every service validates its own inputs and outputs.
8.  Services cannot call other services at the same level.
9.  Service methods cannot call other service methods at the same level.
    - If shared logic exists, extract it to a private method that both public methods can call.
10. Public APIs cannot call public APIs at the same level.
11. Flow forward only.

## Foundation service rules

0. Foundation services are broker-neighboring services.
1. Their purpose is validation, abstraction, mapping, and primitive business language.
2. They must remain pure-primitive.
   - No multi-step higher-order business logic.
3. They must integrate with one entity broker only.
4. Support brokers like logging and time are allowed.
5. They must translate technology language to business language.
   - Insert -> Add
   - Select -> Retrieve
   - Update -> Modify
   - Delete -> Remove
6. They must wrap logic in TryCatch / exception-noise-cancellation.
7. They must perform validation before dependency calls.
8. They are the last abstraction layer before core business logic.
9. Foundation services live under Services/Foundations.
10. Fondation service models live under Models/Foundations/{Entity Plural}/{Entity}.cs
11. Fondation service exceptionmodels live under Models/Foundations/{Entity Plural}/Exceptions/ 

### Validation rules

0. Validation order is mandatory:
   - Structural
   - Logical
   - External
1. Circuit-breaking validations exit immediately.
2. Continuous validations accumulate errors, then break the circuit.
3. Use upsertable exceptions for accumulated validation data.
4. Use dynamic rules that include both condition and human-readable message.
5. Use a rule collector to aggregate and then throw.
6. Use hybrid continuous validation for nested models.
7. Validate outgoing data when the current routine uses it.
8. Map native failures into local exception models.

### Exception rules for foundation services

0. Localize native exceptions. 
   - The data dictionary from native exception must be assigned to the localised exception's data dictionary.
   - The inner exception of the localised exception must be the native exception when possible.
1. Categorize exceptions into validation, dependency validation, dependency, and service exceptions.
2. Preserve inner localized exceptions when moving upstream.
3. Log at the appropriate severity.
4. Critical dependency failures are infrastructure/configuration failures.
5. Do not leak raw native exception semantics into upstream pure business logic.

## Processing service rules

0. Processing services hold higher-order single-entity business logic.
1. They may combine primitive operations from one foundation service.
2. They may use utility brokers.
3. They may not use entity brokers directly.
4. They may depend on one and only one foundation service.
5. Their naming must include the entity and the Processing suffix.
6. They validate only what they use from the input.
7. They may shift outcomes to primitives such as bool or int.
8. They may combine multiple primitive routines into one higher-order routine.
9. They map exceptions from foundation layer to processing-layer exception categories.
10. They must localise and categorise exceptions from the foundation layer.
11. Processing services live under Services/Processings.
10. Processing service virtual models live under Models/Processings/{Entity Plural}/{Entity}.cs
11. Processing service exceptionmodels live under Models/Processings/{Entity Plural}/Exceptions/ 

## Orchestration service rules

0. Orchestration services combine multi-entity operations.
1. They may depend on foundation services or processing services, but not a mixed set of both for entity/business dependencies.
2. Utility brokers are allowed in addition.
3. Florance Pattern is mandatory.
4. Two-Three rule is mandatory for entity/business service dependencies.
5. If dependencies exceed the rule, normalize.
   - Full normalization
   - Semi-normalization
   - No-normalization only as the last option
6. Every breakdown must be meaningful.
7. Orchestration services may be pass-through when contract purity is preserved.
8. Orchestration services may expose physical contracts or virtual contracts.
9. They are responsible for mapping and branching.
10. They are responsible for calling dependencies in the proper order.
11. Natural order is preferred when dependencies require it.
12. Enforced order must be tested when the call dependency is not naturally encoded in input/output relationships.
13. Cul-De-Sac orchestration is valid for event/listener scenarios.
14. Variants include coordination, management, and uber-management services.
15. Keep a unit-of-work mindset.
16. Prefer eventing when it reduces orchestration complexity safely.
17. They map exceptions from foundation layer or processing layer to orchestration-layer exception categories.
18. They must localise and categorise exceptions from the foundation layer or processing layer.
19. Orchestration services live under Services/Orchestrations.
20. Orchestration service virtual models live under Models/Orchestrations/{Entity Plural}/{Entity}.cs
21. Orchestration service exceptionmodels live under Models/Orchestrations/{Entity Plural}/Exceptions/ 

## Aggregation service rules

0. Aggregation services are the knot at the border of core business logic.
1. They provide a single exposure point when many same-variation services share the same contract.
2. They do not add business logic.
3. They can have many same-variation dependencies.
4. They do not validate call order.
5. They only perform basic structural validations.
6. They may aggregate by multi-call routine or by pass-through methods.
7. They are optional.
8. Their dependencies must share the same contract family.
9. They must aggregate exceptions the same way orchestration-like services do.
10. They must localise and categorise exceptions the same way orchestration-like services do.
11. Aggregation services lives under Services/Aggregations.
12. Aggregation service exceptionmodels live under Models/Aggregations/{Entity Plural}/Exceptions/ 

## Exposer rules

0. Exposers are disposable mapping layers.
1. Their purpose is duplex mapping in and out of the core business logic.
2. They are pure mapping only.
3. They may not talk to brokers.
4. They may not contain business logic.
5. They may have one and only one service dependency.
6. That dependency must be a service, not a broker.
7. They must provide a single point of contact.

### Exposure component types

0. Communication protocols
1. User interfaces
2. I/O components / background daemons

## Communication protocol rules

0. Return core-business results in the protocol’s form.
1. Return error reports faithfully and with standardized codes.
2. Support result communication and error reporting.
3. Keep mapping thin and explicit.

## RESTful API rules

0. Controllers speak HTTP verb language.
   - Post / Get / Put / Delete
1. Custom verbs are allowed when the business operation goes beyond CRUD and the standard verbs do not fit.
2. Similar verbs are allowed across different routines when names and routes differentiate them.
3. Routes must never contain verbs.
4. Routes must use nouns.
5. Enforce the single-noun principle.
6. Prefer resource intersections to noun-collisions.
7. Controller classes are plural.
8. Controller routes should usually follow api/[controller].
9. Method-specific routes extend the controller route.
10. Follow plural-singular-plural route patterns for nested resource intersections.
11. Use query parameters and OData where appropriate for queryable reads.
12. x-www-form-urlencoded is allowed for form-style endpoint inputs.
13. Controller methods should not accept more than three parameters; beyond that, design a model.
14. Controllers have one service dependency.
15. Controllers honor a single contract family.
16. Controllers live under Controllers.
17. Every system should have a HomeController heartbeat endpoint.
18. HomeController should not require security and should only indicate aliveness.

### REST response code rules

0. Success responses:
   - 200 OK for successful GET / PUT / DELETE style outcomes
   - 201 Created for successful POSTs
   - 202 Accepted for delegated or eventual-consistency submissions
1. User error responses:
   - 400 BadRequest for validation and dependency-validation issues when mapped to user-correctable input/domain problems
   - 404 NotFound for not-found scenarios
   - 409 Conflict for already-exists scenarios
   - 423 Locked for locked-resource scenarios
   - 424 FailedDependency for invalid-reference scenarios
2. System error responses:
   - 500 InternalServerError for dependency or service failures
   - 507 InsufficientStorage for internal storage issues when applicable
3. Preserve security boundaries.
   - Do not expose internal details from dependency/service failures unless the protocol requires a sanitized representation.

## User interface rules

0. UI exposers must map progress, results, and errors.
1. Never fake progress.
2. Choose progress reporting level intentionally:
   - Basic progress
   - Remaining progress
   - Detailed progress
3. Choose results reporting level intentionally:
   - Simple
   - Partial details
   - Full details
4. It is a violation to redirect users after submission with no indication of what happened.
5. Error reports must tell users what happened, why it happened, and the next course of action when possible.
6. Error report types:
   - Informational
   - Referential / implicit action
   - Actionable
7. Translate technical error language into user-appropriate language.
8. UI exposers have one single dependency.
9. UI anatomy must separate:
   - Bases
   - Components
   - Containers
10. Base components wrap native or third-party UI elements.
11. Core components integrate with one and only one view service.
12. Containers/pages orchestrate UI components and routes only.
13. Containers may not contain UI logic.
14. Containers may not use base components directly.
15. Base components may not wrap more than one non-local component when avoidable.
16. Base components do not contain business logic, exception handling, validations, or calculations.
17. Pages are route containers and do not require business logic.
18. Separate markup, code-behind, and style files.
19. Organize UI under Views/Bases, Views/Components, and Views/Pages.
20. Use domain-driven UI organization.

### Web-application specifics

0. Base components behave like brokers.
1. Core components behave like service/controller hybrids.
2. Core components are test-driven.
3. Core components are built from:
   - Elements
   - Styles
   - Actions
4. Element testing must cover:
   - Existence
   - Properties
   - Actions
5. Existence testing may use:
   - Property assignment
   - Searching by id
   - General search
6. Styles belong primarily to core components, not bases.
7. Pages compose components and represent routes.
8. Pages typically do not require unit tests.
9. Keep UI unobtrusive; do not place CSS, C#, and markup in the same file.
10. Core components do not call other core components at the same level.
11. One view service corresponds to one core component and one view model.

## Architecture review checklist

When reviewing or generating architecture, verify all of the following:

0. Purpose is explicit.
1. Models are scoped to the purpose.
2. Dependencies and exposure are separate from purpose.
3. Broker responsibilities are thin and disposable.
4. Services own business logic.
5. Validation order is correct.
6. Service-layer flow is forward only.
7. Same-level services are not calling each other.
8. Florance Pattern is honored.
9. Exposure layers have single point of contact.
10. APIs honor route and status-code rules.
11. UI honors single dependency, unobtrusiveness, and anatomy rules.
12. Architecture remains readable, autonomous, and rewritable.

## .NET implementation profile included from the supplied implementation specification

The following addendum preserves the supplied implementation profile so the skill can enforce both the abstract Standard and the concrete .NET implementation style you attached.

## 1. Overview

The Standard is an opinionated software engineering standard authored by Hassan Habib.
It prescribes a **tri-nature** architecture consisting of:

| Layer          | Responsibility                                                |
| -------------- | ------------------------------------------------------------- |
| **Brokers**    | Thin abstraction over any external dependency (DB, API, logs) |
| **Services**   | All business logic — validation, orchestration, coordination  |
| **Exposers**   | Entry points that expose services (Controllers, Endpoints)    |

This project currently implements **Brokers** and **Foundation Services** for the
`LegacyUser` entity (storage-based) and the `Person` entity (API-based), as well as
an **API Broker** for sending `Person` entities to an external API.

---

## 2. Project Structure

```
RedRhino.Core.Synchronizer/
├── Brokers/
│   ├── Apis/
│   │   ├── IModernApiBroker.cs             (partial interface — base)
│   │   ├── IModernApiBroker.Persons.cs     (partial interface — entity)
│   │   ├── ModernApiBroker.cs              (partial class — base)
│   │   └── ModernApiBroker.Persons.cs      (partial class — entity)
│   ├── Loggings/
│   │   ├── ILoggingBroker.cs
│   │   └── LoggingBroker.cs
│   └── Storages/
│       ├── IStorageBroker.cs              (partial interface — base)
│       ├── IStorageBroker.LegacyUsers.cs  (partial interface — entity)
│       ├── StorageBroker.cs               (partial class — base)
│       └── StorageBroker.LegacyUsers.cs   (partial class — entity)
├── Models/
│      ├── Foundations/
│            ├── Persons/
│            │   ├── Person.cs
│            │   ├── PersonType.cs
│            │   ├── PersonRecordState.cs
│            │   └── Exceptions/
│            │       ├── NullPersonException.cs
│            │       ├── InvalidPersonException.cs
│            │       ├── AlreadyExistsPersonException.cs
│            │       ├── FailedPersonDependencyException.cs
│            │       ├── FailedPersonServiceException.cs
│            │       ├── PersonValidationException.cs
│            │       ├── PersonDependencyException.cs
│            │       ├── PersonDependencyValidationException.cs
│            │       └── PersonServiceException.cs
│            └── LegacyUsers/
│                ├── LegacyUser.cs
│                └── Exceptions/
│                    ├── NullLegacyUserException.cs
│                    ├── InvalidLegacyUserException.cs
│                    ├── AlreadyExistsLegacyUserException.cs
│                    ├── FailedStorageLegacyUserDependencyException.cs
│                    ├── FailedLegacyUserServiceException.cs
│                    ├── LegacyUserValidationException.cs
│                    ├── LegacyUserDependencyException.cs
│                    ├── LegacyUserDependencyValidationException.cs
│                    └── LegacyUserServiceException.cs
├── Services/
│   └── Foundations/
│       ├── LegacyUsers/
│       │   ├── ILegacyUserService.cs
│       │   ├── LegacyUserService.cs               (partial — logic)
│       │   ├── LegacyUserService.Validations.cs   (partial — validations)
│       │   └── LegacyUserService.Exceptions.cs    (partial — exception handling)
│       └── Persons/
│           ├── IPersonService.cs
│           ├── PersonService.cs                    (partial — logic)
│           ├── PersonService.Validations.cs        (partial — validations)
│           └── PersonService.Exceptions.cs         (partial — exception handling)
└── Program.cs

RedRhino.Core.Synchronizer.Tests.Units/
└── Services/
    └── Foundations/
        ├── LegacyUsers/
        │   ├── LegacyUserServiceTests.cs              (partial — setup & helpers)
        │   ├── LegacyUserServiceTests.Logic.{Method}.cs        (partial — happy-path tests)
        │   ├── LegacyUserServiceTests.Validations.{Method}.cs  (partial — validation tests)
        │   └── LegacyUserServiceTests.Exceptions.{Method}.cs   (partial — exception tests)
        └── Persons/
            ├── PersonServiceTests.cs                   (partial — setup & helpers)
            ├── PersonServiceTests.Logic.{Method}.cs             (partial — happy-path tests)
            ├── PersonServiceTests.Validations.{Method}.cs       (partial — validation tests)
            └── PersonServiceTests.Exceptions.{Method}.cs        (partial — exception tests)
```

---

## 3. Brokers

Brokers are **thin wrappers** around external resources. They contain **zero business logic**.

> **Rule — Generic Helpers:** Every broker base partial **must** expose private generic
> helper methods (e.g., `InsertAsync<T>`, `PostAsync<T>`) that encapsulate the underlying
> client calls. Entity partial files **never** reference the private client member
> (e.g., `this.apiClient`, `this.Entry(...)`) directly — they delegate to the generic
> helpers instead. This keeps entity partials decoupled from the concrete client and
> allows swapping the underlying implementation in a single place.

### 3.1 Storage Broker

| Aspect               | Implementation                                                                                           |
| -------------------- | -------------------------------------------------------------------------------------------------------- |
| Base class           | `EFxceptionsContext` (from the **EFxceptions** library — wraps `DbContext` with meaningful EF exceptions) |
| Interface            | `partial interface IStorageBroker` — split per entity                                                    |
| Class                | `partial class StorageBroker` — split per entity                                                         |
| Generic CRUD helpers | Private helpers in base partial: `InsertAsync<T>`, `SelectAllAsync<T>`, `SelectAsync<T>`, `UpdateAsync<T>`, `DeleteAsync<T>` |
| Configuration        | Reads `DefaultConnection` from `IConfiguration`; calls `this.Database.Migrate()` at construction        |

**Base partial — `StorageBroker.cs`**

This file owns: `IConfiguration`, `OnConfiguring`, the constructor (with `Database.Migrate()`),
and private generic CRUD helpers. It owns **nothing else**.

> **arch-014 — No `DbSet<>` in the base partial.**
> `DbSet<Student> Students` lives in `StorageBroker.Students.cs`, not here.
> The base partial must never declare entity-specific members.

```csharp
public partial class StorageBroker : EFxceptionsContext, IStorageBroker
{
    // No DbSet<> properties here. Each entity partial declares its own.

    private readonly IConfiguration configuration;

    public StorageBroker(IConfiguration configuration)
    {
        this.configuration = configuration;

        // arch-011: Must be called — applies pending migrations at startup.
        // Omitting this means the schema is never applied.
        this.Database.Migrate();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString =
            this.configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseSqlServer(connectionString);
    }

    // arch-012: All EF operations live here. Entity partials call these helpers only.
    private async ValueTask<T> InsertAsync<T>(T entity) where T : class
    {
        this.Entry(entity).State = EntityState.Added;
        await this.SaveChangesAsync();
        return entity;
    }

    // SelectAllAsync<T> wraps IQueryable in a ValueTask so all entity methods
    // follow the uniform async/await pattern. AsNoTracking() is applied here.
    private ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
        ValueTask.FromResult<IQueryable<T>>(this.Set<T>().AsNoTracking());

    private async ValueTask<T> SelectAsync<T>(Guid entityId) where T : class =>
        await this.FindAsync<T>(entityId);

    private async ValueTask<T> UpdateAsync<T>(T entity) where T : class
    {
        this.Entry(entity).State = EntityState.Modified;
        await this.SaveChangesAsync();
        return entity;
    }

    private async ValueTask<T> DeleteAsync<T>(T entity) where T : class
    {
        this.Entry(entity).State = EntityState.Deleted;
        await this.SaveChangesAsync();
        return entity;
    }
}
```

**Entity partial — `StorageBroker.Students.cs`**

> **arch-014:** `DbSet<Student>` is declared here — in the entity partial — not in `StorageBroker.cs`.

```csharp
public partial class StorageBroker
{
    // DbSet<Student> belongs in this entity partial file, not in StorageBroker.cs.
    public DbSet<Student> Students { get; set; }

    public async ValueTask<Student> InsertStudentAsync(Student student) =>
        await this.InsertAsync(student);

    // arch-009: SelectAll* must be async ValueTask<IQueryable<T>>.
    // WRONG:   public IQueryable<Student> SelectAllStudents() => this.Students.AsNoTracking();
    // WRONG:   public IQueryable<Student> SelectAllStudents() => this.Set<Student>();
    // CORRECT: delegate to SelectAllAsync<T>() — never touch DbSet or EF members directly.
    public async ValueTask<IQueryable<Student>> SelectAllStudentsAsync() =>
        await this.SelectAllAsync<Student>();

    public async ValueTask<Student> SelectStudentByIdAsync(Guid studentId) =>
        await this.SelectAsync<Student>(studentId);

    public async ValueTask<Student> UpdateStudentAsync(Student student) =>
        await this.UpdateAsync(student);

    public async ValueTask<Student> DeleteStudentAsync(Student student) =>
        await this.DeleteAsync(student);
}
```

> **Rule:** Each entity gets its own partial file for both the interface and the class.
>
> **Rule — scope before scaffolding (arch-013):** A branch named `BROKERS-student-insert`
> implements **only** `InsertStudentAsync`. If the prompt is ambiguous about which
> operations are needed, the agent must ask before creating the branch or writing any code.
> A single broker branch = a single operation.
>
> **Rule — branch action language (prac-016):** Broker branch actions use infrastructure
> verbs — `insert`, `select-all`, `select-by-id`, `update`, `delete`. Never use business
> verbs (`add`, `retrieve`, `modify`, `remove`) in a `BROKERS` branch name.

### 3.2 API Broker

| Aspect               | Implementation                                                                                   |
| -------------------- | ------------------------------------------------------------------------------------------------ |
| HTTP client          | `IRESTFulApiFactoryClient` (from the **RESTFulSense** library — wraps `HttpClient`)              |
| Interface            | `partial interface IModernApiBroker` — split per entity                                          |
| Class                | `partial class ModernApiBroker` — split per entity                                               |
| Generic HTTP helpers | Private `PostAsync<T>()` on the base partial for reuse across entities                           |
| Configuration        | Reads `ApiConfigurations:Url` from `IConfiguration` to set `HttpClient.BaseAddress`              |

**Base partial — `ModernApiBroker.cs`**

```csharp
public partial class ModernApiBroker : IModernApiBroker
{
    private readonly IRESTFulApiFactoryClient apiClient;

    public ModernApiBroker(IConfiguration configuration)
    {
        var httpClient = new HttpClient()
        {
            BaseAddress =
                new Uri(configuration.GetValue<string>("ApiConfigurations:Url"))
        };

        this.apiClient = new RESTFulApiFactoryClient(httpClient);
    }

    private async ValueTask<T> PostAsync<T>(string relativeUrl, T content) =>
        await this.apiClient.PostContentAsync<T>(relativeUrl, content);
}
```

**Entity partial — `ModernApiBroker.Persons.cs`**

```csharp
public partial class ModernApiBroker
{
    private const string PersonsRelativeUrl = "api/persons";

    public async ValueTask<Person> PostPersonAsync(Person person) =>
        await PostAsync(PersonsRelativeUrl, person);
}
```

> **Rule:** Entity partials delegate to the generic helpers (`PostAsync<T>`) — they
> never call `this.apiClient` directly.

### 3.3 Logging Broker

A dedicated abstraction over `ILogger<T>`. The broker exposes only **async `ValueTask`** methods
that correspond to standard log levels:

| Method                | Log Level   |
| --------------------- | ----------- |
| `LogInformationAsync` | Information |
| `LogTraceAsync`       | Trace       |
| `LogDebugAsync`       | Debug       |
| `LogWarningAsync`     | Warning     |
| `LogErrorAsync`       | Error       |
| `LogCriticalAsync`    | Critical    |

**`LoggingBroker.cs`**

```csharp
public class LoggingBroker : ILoggingBroker
{
    private readonly ILogger<LoggingBroker> logger;

    public LoggingBroker(ILogger<LoggingBroker> logger) =>
        this.logger = logger;

    public async ValueTask LogInformationAsync(string message) =>
        this.logger.LogInformation(message);

    public async ValueTask LogTraceAsync(string message) =>
        this.logger.LogTrace(message);

    public async ValueTask LogDebugAsync(string message) =>
        this.logger.LogDebug(message);

    public async ValueTask LogWarningAsync(string message) =>
        this.logger.LogWarning(message);

    public async ValueTask LogErrorAsync(Exception exception) =>
        this.logger.LogError(exception, exception.Message);

    public async ValueTask LogCriticalAsync(Exception exception) =>
        this.logger.LogCritical(exception, exception.Message);
}
```

**`ILoggingBroker.cs`**

```csharp
public interface ILoggingBroker
{
    ValueTask LogInformationAsync(string message);
    ValueTask LogTraceAsync(string message);
    ValueTask LogDebugAsync(string message);
    ValueTask LogWarningAsync(string message);
    ValueTask LogErrorAsync(Exception exception);
    ValueTask LogCriticalAsync(Exception exception);
}
```

> **Rule — async expression body, no Task.Run:**
> Each method uses the `async` keyword and delegates directly to `ILogger<T>`.
> Never wrap the call in `Task.Run()` or `new ValueTask(Task.Run(...))`.
> `ILogger<T>` is synchronous; wrapping it in `Task.Run()` introduces
> unnecessary thread-pool overhead and produces an inefficient heap-allocated `ValueTask`.
>
> **Wrong:**
> ```csharp
> public ValueTask LogWarningAsync(string message) =>
>     new ValueTask(Task.Run(() => this.logger.LogWarning(message)));
> ```
>
> **Correct:**
> ```csharp
> public async ValueTask LogWarningAsync(string message) =>
>     this.logger.LogWarning(message);
> ```

> **Rule — DI Registration:**
> The logging broker must be explicitly registered in `Program.cs`.
> `AddLogging()` (or the host builder's default) must also be present so that
> `ILogger<LoggingBroker>` resolves correctly.
>
> ```csharp
> builder.Services.AddLogging();
> builder.Services.AddTransient<ILoggingBroker, LoggingBroker>();
> ```
>
> Omitting either line causes a runtime DI resolution failure.

---

## 4. Models

### 4.1 Entity Model

Entity classes are plain POCOs residing under `Models/{Service Type}/{Entity}/`. They contain
**no behavior** — only properties. Domain comments on properties capture validation intent
(e.g., nullable rules, email format, phone format).

The `LegacyUser` class resides under `Models/Foundations/LegacyUsers/`.
The `Person` class resides under `Models/Foundations/Persons/`.

### 4.2 Exception Models

Exception models live in `Models/{Service Type}/{Entity}/Exceptions/` and form a **two-tier exception
hierarchy** per The Standard.

The `LegacyUser` class resides under `Models/Foundations/LegacyUsers/Exceptions/`.
The `Person` class resides under `Models/Foundations/Persons/Exceptions/`.

The inner/outer exception hierarchy varies by the **type of broker** the Foundation Service
consumes. Storage-based services (e.g., `LegacyUserService`) handle SQL/EF exceptions, while
API-based services (e.g., `PersonService`) handle RESTFulSense HTTP exceptions.

#### 4.2.1 Storage-Based Exceptions (LegacyUser)

##### Inner (Local) Exceptions

| Exception                                    | Purpose                                     |
| -------------------------------------------- | ------------------------------------------- |
| `NullLegacyUserException`                    | Entity is `null`                            |
| `InvalidLegacyUserException`                 | One or more property-level validation fails |
| `AlreadyExistsLegacyUserException`           | Duplicate key detected                      |
| `FailedStorageLegacyUserDependencyException` | SQL / storage-level failure                 |
| `FailedLegacyUserServiceException`           | Unexpected runtime failure                  |

##### Outer (Categorical) Exceptions

| Exception                                 | Category                 | Wrapped Inner(s)                                                              |
| ----------------------------------------- | ------------------------ | ----------------------------------------------------------------------------- |
| `LegacyUserValidationException`           | **Validation**           | `NullLegacyUserException`, `InvalidLegacyUserException`                       |
| `LegacyUserDependencyException`           | **Dependency**           | `FailedStorageLegacyUserDependencyException`                                  |
| `LegacyUserDependencyValidationException` | **DependencyValidation** | `AlreadyExistsLegacyUserException`, `InvalidLegacyUserException` (from `DbUpdateException`) |
| `LegacyUserServiceException`              | **Service**              | `FailedLegacyUserServiceException`                                            |

#### 4.2.2 API-Based Exceptions (Person)

##### Inner (Local) Exceptions

| Exception                         | Purpose                                                              |
| --------------------------------- | -------------------------------------------------------------------- |
| `NullPersonException`             | Entity is `null`                                                     |
| `InvalidPersonException`          | One or more property-level validation fails, or `BadRequest` from API|
| `AlreadyExistsPersonException`    | `Conflict` (409) from API                                            |
| `FailedPersonDependencyException` | HTTP-level failure (any HTTP error or `HttpRequestException`)        |
| `FailedPersonServiceException`    | Unexpected runtime failure                                           |

##### Outer (Categorical) Exceptions

| Exception                          | Category                      | Wrapped Inner(s)                                                                          |
| ---------------------------------- | ----------------------------- | ----------------------------------------------------------------------------------------- |
| `PersonValidationException`        | **Validation**                | `NullPersonException`, `InvalidPersonException`                                           |
| `PersonDependencyValidationException` | **DependencyValidation**   | `InvalidPersonException` (from `BadRequest`), `AlreadyExistsPersonException` (from `Conflict`) |
| `PersonDependencyException`        | **Dependency** (Critical)     | `FailedPersonDependencyException` (from `Unauthorized`, `Forbidden`, `NotFound`, `UrlNotFound`, `HttpRequestException`) |
| `PersonDependencyException`        | **Dependency** (Non-Critical) | `FailedPersonDependencyException` (from `InternalServerError`, `ServiceUnavailable`)      |
| `PersonServiceException`           | **Service**                   | `FailedPersonServiceException`                                                            |

All exceptions derive from **`Xeption`** (from the `Xeption` NuGet package), which provides
`UpsertDataList` and `ThrowIfContainsErrors` for aggregated validation data.

---

## 5. Services — Foundations

A Foundation Service sits directly on top of Brokers and is the **only consumer** of them.

### 5.1 Partial Class Layout

The service is split into three partial files:

| Partial file                          | Concern                                       |
| ------------------------------------- | --------------------------------------------- |
| `{Entity}Service.cs`                  | Constructor, DI fields, public business logic |
| `{Entity}Service.Validations.cs`      | All `Validate*` and `IsInvalid*` methods      |
| `{Entity}Service.Exceptions.cs`       | `TryCatch` delegate pattern, `CreateAndLog*` helpers |

### 5.2 Dependency Injection

A Foundation Service depends **only** on Brokers — never on other services.

**Storage-based service (LegacyUserService):**

```csharp
public partial class LegacyUserService : ILegacyUserService
{
    private readonly IStorageBroker storageBroker;
    private readonly ILoggingBroker loggingBroker;

    public LegacyUserService(
        IStorageBroker storageBroker,
        ILoggingBroker loggingBroker) { ... }
}
```

**API-based service (PersonService):**

```csharp
public partial class PersonService : IPersonService
{
    private readonly IModernApiBroker modernApiBroker;
    private readonly ILoggingBroker loggingBroker;

    public PersonService(
        IModernApiBroker modernApiBroker,
        ILoggingBroker loggingBroker) { ... }
}
```

### 5.3 Business Logic — TryCatch Pattern

Every public method delegates to a `TryCatch` wrapper that catches, wraps, logs, and
re-throws categorised exceptions:

**Storage-based:**

```csharp
public ValueTask<LegacyUser> AddLegacyUserAsync(LegacyUser legacyUser) =>
TryCatch(async () =>
{
    ValidateLegacyUser(legacyUser);

    return await this.storageBroker.InsertLegacyUserAsync(legacyUser);
});
```

**API-based:**

```csharp
public ValueTask<Person> AddPersonAsync(Person person) =>
TryCatch(async () =>
{
    ValidatePerson(person);

    return await this.modernApiBroker.PostPersonAsync(person);
});
```

### 5.4 Validations

Validations use a **dynamic rule + aggregate** pattern:

```csharp
Validate(
    (IsInvalid(legacyUser.Id),        nameof(LegacyUser.Id)),
    (IsInvalid(legacyUser.UserPK),    nameof(LegacyUser.UserPK)),
    (IsInvalid(legacyUser.UserName),  nameof(LegacyUser.UserName)),
    ...);
```

Each `IsInvalid` overload returns an anonymous object with `Condition` (bool) and `Message` (string).
The `Validate` method aggregates all failures into a single `InvalidLegacyUserException` via
`UpsertDataList` and calls `ThrowIfContainsErrors()`.

Custom validators exist for domain-specific rules:

| Validator           | Rule                              |
| ------------------- | --------------------------------- |
| `IsInvalid(Guid)`   | Must not be `Guid.Empty`          |
| `IsInvalid(int)`    | Must not be `default (0)`         |
| `IsInvalid(string)` | Must not be null/empty/whitespace |
| `IsInvalidLob(int)` | Must be greater than 0            |
| `IsInvalidEmail`    | Regex-based email format check    |

---

## 6. Exception Handling

The `TryCatch` method in each `{Entity}Service.Exceptions.cs` implements The Standard's
**exception mapping** pattern. The specific catch blocks differ based on the broker type
the service consumes.

### 6.1 Storage-Based Exception Mapping (LegacyUser)

```
Native / External Exception  →  Inner (Local) Exception       →  Outer (Categorical) Exception
──────────────────────────────────────────────────────────────────────────────────────────────────
(null input)                 →  NullLegacyUserException        →  LegacyUserValidationException
(validation rules fail)      →  InvalidLegacyUserException     →  LegacyUserValidationException
SqlException                 →  FailedStorage...Exception      →  LegacyUserDependencyException       (Critical)
DuplicateKeyException        →  AlreadyExists...Exception      →  LegacyUserDependencyValidationException
DbUpdateException            →  InvalidLegacy...Exception      →  LegacyUserDependencyValidationException
Exception (catch-all)        →  FailedService...Exception      →  LegacyUserServiceException
```

### 6.2 API-Based Exception Mapping (Person)

For services that call external APIs through RESTFulSense, the exception mapping covers
all HTTP response exceptions. The ordering of catch blocks in `TryCatch` matters — more
specific exceptions must appear before their base classes.

```
Native / External Exception               →  Inner (Local) Exception           →  Outer (Categorical) Exception         Log Level
────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
(null input)                               →  NullPersonException               →  PersonValidationException             LogError
(validation rules fail)                    →  InvalidPersonException             →  PersonValidationException             LogError
HttpResponseBadRequestException            →  InvalidPersonException             →  PersonDependencyValidationException   LogError
HttpResponseConflictException              →  AlreadyExistsPersonException       →  PersonDependencyValidationException   LogError
HttpResponseUnauthorizedException          →  FailedPersonDependencyException    →  PersonDependencyException             LogCritical
HttpResponseForbiddenException             →  FailedPersonDependencyException    →  PersonDependencyException             LogCritical
HttpResponseNotFoundException              →  FailedPersonDependencyException    →  PersonDependencyException             LogCritical
HttpResponseUrlNotFoundException           →  FailedPersonDependencyException    →  PersonDependencyException             LogCritical
HttpResponseInternalServerErrorException   →  FailedPersonDependencyException    →  PersonDependencyException             LogError
HttpResponseServiceUnavailableException    →  FailedPersonDependencyException    →  PersonDependencyException             LogError
HttpRequestException                       →  FailedPersonDependencyException    →  PersonDependencyException             LogCritical
Exception (catch-all)                      →  FailedPersonServiceException       →  PersonServiceException                LogError
```

> **Rule — Critical vs Non-Critical Dependency:** Exceptions that indicate the API endpoint
> is unreachable or inaccessible (`Unauthorized`, `Forbidden`, `NotFound`, `UrlNotFound`,
> `HttpRequestException`) are logged at `LogCriticalAsync` because they signal configuration
> or infrastructure failures. Server-side errors (`InternalServerError`, `ServiceUnavailable`)
> are logged at `LogErrorAsync` because they may be transient.

Each `CreateAndLog*` helper:

1. Wraps the inner exception in the categorical outer exception.
2. Logs via the `ILoggingBroker` at the appropriate level (`LogErrorAsync` or `LogCriticalAsync`).
3. Returns the outer exception so `TryCatch` can `throw` it.

---

## 7. Dependency Registration (Exposers)

All wiring is done in `Program.cs` following The Standard's explicit registration style:

```csharp
builder.Services.AddDbContext<StorageBroker>();
builder.Services.AddTransient<IStorageBroker, StorageBroker>();
builder.Services.AddTransient<IModernApiBroker, ModernApiBroker>();
builder.Services.AddTransient<ILoggingBroker, LoggingBroker>();
builder.Services.AddTransient<ILegacyUserService, LegacyUserService>();
builder.Services.AddTransient<IPersonService, PersonService>();
```

> Brokers and Foundation Services are registered as **Transient**.

---

## 10. Naming Conventions

| Element            | Pattern                                                       | Example                            |
| ------------------ | ------------------------------------------------------------- | ---------------------------------- |
| Broker interface   | `I{Resource}Broker`                                           | `IStorageBroker`, `IModernApiBroker`, `ILoggingBroker` |
| Broker class       | `{Resource}Broker`                                            | `StorageBroker`, `ModernApiBroker`, `LoggingBroker` |
| Broker method      | `{Action}{Entity}Async`                                       | `InsertLegacyUserAsync`, `PostPersonAsync` |
| Service interface  | `I{Entity}Service`                                            | `ILegacyUserService`               |
| Service class      | `{Entity}Service`                                             | `LegacyUserService`                |
| Service method     | `Add{Entity}Async`                                            | `AddLegacyUserAsync`               |
| Inner exception    | `{Adjective}{Entity}Exception`                                | `NullLegacyUserException`          |
| Outer exception    | `{Entity}{Category}Exception`                                 | `LegacyUserValidationException`    |
| Test class         | `{Entity}ServiceTests`                                        | `LegacyUserServiceTests`           |
| Test method        | `Should{Action}Async` / `ShouldThrow{Exception}On{Action}…`  | `ShouldAddLegacyUserAsync`         |

---
