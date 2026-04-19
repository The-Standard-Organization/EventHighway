---
name: The Standard Testing
description: Enforces Standard TDD discipline, validation testing, exception mapping, controller acceptance tests, and UI component testing.
the standard version: v2.12.0
skill version: v0.3.0.0
---

# The Standard Testing

## What this skill is

This skill governs how The Standard is tested, verified, and proven.
It covers test-driven development, validation strategies, exception mapping, unit test structure, controller tests, and UI component tests.

## Explicit coverage map

This skill explicitly covers:

- Foundation-service implementation and validation/testing patterns from the Services chapter
- Structural, logical, external, and dependency validation testing
- Exception mapping and category testing
- Processing-service testing responsibilities
- Orchestration-service call-order testing
- Aggregation-service testing restrictions
- REST controller unit and acceptance tests
- UI component testing for bases, core components, and pages
- The supplied implementation specification sections on unit testing, partial test organization, conventions, AAA, and test order
- TDD FAIL/PASS discipline relevant to test creation and implementation verification

## When to use

Use this skill whenever writing, reviewing, expanding, fixing, or sequencing tests.
Use it whenever deciding what to test first, how to map exceptions in tests, or how to prove a Standard-compliant flow.

## Core testing doctrine

0. Follow TDD.
1. Write the failing test first.
2. Verify the test actually fails.
3. Write the minimum implementation required to pass.
4. Verify the full relevant suite passes.
5. Refactor without changing behavior.
6. Repeat.

## Validation testing rules

### Validation Source of Truth

0. Validation rules MUST be inferred from all authoritative sources:
   - Foundation service business rules
   - Storage-layer configuration (`StorageBroker.[Entity].Configurations.cs`)
   - Domain expectations implied by usage

1. The storage configuration represents **minimum enforced constraints**:
   - Required vs optional
   - Maximum length
   - Minimum length (if configured)
   - Precision / scale / format where applicable

2. Foundation services represent **the enforcement boundary**:
   - All constraints that can cause persistence failure MUST be validated before reaching storage
   - Validation must prevent database exceptions where deterministically possible

### Validation Alignment Rules

0. Foundation validation MUST be **equal to or stricter than** storage constraints.

1. The following are **ALLOWED (strengthening rules)**:
   - Storage: optional → Foundation: required
   - Storage: optional → Foundation: constrained (min/max length)
   - Storage: max length → Foundation: smaller max length

2. The following are **NOT ALLOWED (weakening or missing rules)**:
   - Storage: required → Foundation: not validated as required
   - Storage: max length → Foundation: no length validation

3. Violations of alignment MUST be treated as:
   - A design defect
   - A test failure condition
   - A review blocker

### Validation Responsibility Rule

0. The database MUST NOT be relied upon to enforce:
   - Required field validation
   - Length validation
   - Format validation

1. The ONLY acceptable database-enforced constraints without prior validation are:
   - Foreign key constraints
   - Uniqueness / duplicate key constraints
   - Concurrency constraints

2. Any validation that can be performed deterministically in the foundation service MUST be performed there


### Validation Test Derivation from Storage Configuration

0. Storage configuration (`StorageBroker.[Entity].Configurations.cs`) defines **constraints**, not validation behavior.

1. For every constraint defined in storage configuration, there MUST exist a corresponding validation rule in the foundation service:
   - Required fields
   - Length constraints (min/max)
   - Precision / format constraints where applicable

2. Validation tests MUST:
   - Target the **foundation service validation methods only**
   - NOT directly test storage configuration behavior
   - Prove that each storage-defined constraint is enforced at the foundation layer

3. For each property constraint, validation tests MUST focus on constraint violations and boundary breaches:

   - Invalid case (violates constraint) → FAIL
     - Required field missing / null
     - Value exceeding maximum length
     - Value below minimum length (if applicable)

   - Boundary violation cases:
     - Just above maximum → FAIL
     - Just below minimum → FAIL (if applicable)

4. Valid scenarios (within acceptable range) are covered by logic tests and MUST NOT be redundantly tested in validation tests.

5. Missing alignment between storage configuration and foundation validation MUST be treated as:
   - A design defect
   - A test failure condition
   - A review blocker

6. Automation MAY assist in identifying constraints (e.g., via EF metadata), but:
   - Generated tests MUST still validate foundation behavior
   - Automation MUST NOT result in tests that validate the database layer directly

7. All validation tests MUST:
   - Follow Standard naming conventions
   - Be explicit and intention-revealing


### Validation order

0. Structural validations first.
1. Logical validations second.
2. External validations third.
3. Dependency validations when the external resource is the source of the failure.

### Circuit-breaking validations

0. Null checks and other hard-stop guards must break immediately.
1. If continuing would create invalid dereference or meaningless work, stop immediately.

### Continuous validations

0. When multiple fields can be invalid independently, collect them before throwing.
1. Use upsertable exception data.
2. Use dynamic rules with condition + message.
3. Use a validations collector routine.
4. Throw once the collector contains errors.

### Hybrid continuous validations

0. Validate parent objects before validating child properties.
1. Split nested validation into levels to avoid unintended null-reference failures.

## Foundation-service test rules

0. Test the happy path first.
1. Then test structural validations.
2. Then test logical validations.
3. Then test external validations.
4. Then test dependency validations.
5. Then test dependency exceptions.
6. Then test service exceptions.
7. Always verify logging and broker calls.
8. Always verify no unwanted calls occurred.
9. Always keep validation and exception behaviors local and explicit.

## Processing-service test rules

0. Test higher-order logic, not primitive broker details.
1. Validate only what the processing service uses.
2. Test shifters.
   - Example: object -> bool or object -> count.
3. Test combinations.
   - Example: retrieve + add, retrieve + modify, ensure-exists, upsert.
4. Test processing exception mapping from foundation exceptions.

## Orchestration-service test rules

0. Test multi-entity flow combinations.
1. Test mapping/branching between contracts when present.
2. Test call order when the flow depends on order.
3. Prefer natural order when inputs/outputs force sequencing.
4. Use explicit order verification when sequencing is not naturally encoded.
5. Verify orchestration-level exception wrapping and unwrapping.
6. Test normalization outcomes indirectly through dependency shape and resulting behavior.

## Aggregation-service test rules

0. Do not test dependency call order in aggregation services.
1. Do not use mock-sequence style order assertions for aggregation services.
2. Test only basic structural validations and exposure-level aggregation behavior.
3. Aggregation services may multi-call or pass-through; test the contract and exposure abstraction, not orchestration logic.

## Controller and protocol test rules

0. Controllers require unit tests for mapping logic.
1. Unit-test success code mappings.
2. Unit-test validation / dependency / service error mappings.
3. Unit-test security i.e authorization / authentication failure mappings.
3. Acceptance-test every endpoint.
4. Clean up test data after acceptance tests.
5. Emulate external resources not owned by the microservice when running acceptance tests.
6. Integration and end-to-end testing are valid beyond unit + acceptance.

## UI component test rules

### Base components

0. Treat bases as thin wrappers.
1. Test their exposed APIs and wrapper behavior when needed.
2. Do not put business logic into bases.

### Core components

0. Core components are test-driven.
1. Test elements.
   - Existence
   - Properties
   - Actions
2. Existence may be tested by property assignment, searching by id, or general search.
3. Test styles when styles are part of the component contract.
4. Test actions that mutate state, create components, or trigger service calls.
5. Core components should integrate with one and only one view service.

### Pages / containers

0. Pages are simpler route containers.
1. They generally do not require unit tests.
2. They should not contain business logic.

## Unit-test conventions from the supplied implementation profile

0. Mirror partial-class split in tests.
1. Use setup/helpers in the root test file.
2. Split tests into logic, validations, and exceptions files.
3. Use GWT: Given / When / Then.
4. Mock all dependencies.
5. Use readable assertions.
6. Use deep cloning to protect expectation identity.
7. Use randomized data by default.
8. Verify exact dependency calls.
9. End with VerifyNoOtherCalls.
10. Keep naming explicit and scenario-driven.

## Exact test implementation order for foundation-service add routines

When implementing an Add{Entity}Async routine under the implementation profile, follow this order:

0. ShouldAdd{Entity}Async
1. ShouldThrowValidationExceptionOnAddIf{Entity}IsNullAndLogItAsync
2. ShouldThrowValidationExceptionOnAddIf{Entity}IsInvalidAndLogItAsync
3. ShouldThrowDependencyValidationExceptionOnAddIfBadRequestErrorOccursAndLogItAsync
4. ShouldThrowDependencyValidationExceptionOnAddIfConflictErrorOccursAndLogItAsync
5. ShouldThrowCriticalDependencyExceptionOnAddIfUnauthorizedErrorOccursAndLogItAsync
6. ShouldThrowCriticalDependencyExceptionOnAddIfForbiddenErrorOccursAndLogItAsync
7. ShouldThrowCriticalDependencyExceptionOnAddIfNotFoundErrorOccursAndLogItAsync
8. ShouldThrowCriticalDependencyExceptionOnAddIfUrlNotFoundErrorOccursAndLogItAsync
9. ShouldThrowDependencyExceptionOnAddIfInternalServerErrorOccursAndLogItAsync
10. ShouldThrowDependencyExceptionOnAddIfServiceUnavailableErrorOccursAndLogItAsync
11. ShouldThrowCriticalDependencyExceptionOnAddIfHttpRequestErrorOccursAndLogItAsync
12. ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync

For storage-based services, substitute the storage equivalents such as duplicate-key, DbUpdate, and SQL exceptions.

## Testing and exception/localization addendum from the supplied implementation profile

## 8. Unit Testing

Unit tests follow The Standard's **partial-class + three-axis** approach.

### 8.1 Test Class Structure

Each entity's tests mirror the same partial-class split as the service:

| Partial file                                      | Tests                                                  |
| ------------------------------------------------- | ------------------------------------------------------ |
| `{Entity}ServiceTests.cs`                        | Setup, mocks, helpers (`CreateRandom{Entity}`, etc.)   |
| `{Entity}ServiceTests.Logic.{Method}.cs`         | Happy-path / success-case tests                        |
| `{Entity}ServiceTests.Validations.{Method}.cs`   | Validation failure tests                               |
| `{Entity}ServiceTests.Exceptions.{Method}.cs`    | Dependency & service exception tests                   |

### 8.2 Conventions

| Convention           | Detail                                                                                      |
| -------------------- | ------------------------------------------------------------------------------------------- |
| Mocking              | **Moq** — `Mock<IStorageBroker>`, `Mock<IModernApiBroker>`, `Mock<ILoggingBroker>`         |
| Assertions           | **FluentAssertions** — `Should().BeEquivalentTo()`                                         |
| Deep cloning         | **DeepCloner** — to isolate input/expected/actual objects                                   |
| Data generation      | **Tynamix.ObjectFiller** — `Filler<{Entity}>` with custom property setup                  |
| Exception comparison | `Xeption.SameExceptionAs()` via `SameExceptionAs` expression helper                        |
| Test naming          | `Should{Action}Async` / `ShouldThrow{Exception}On{Action}If{Condition}AndLogItAsync`      |
| Verify calls         | Every test verifies broker calls (`Times.Once` / `Times.Never`) and ends with `VerifyNoOtherCalls()` |
| Test framework       | **xUnit** — `[Fact]` for single cases, `[Theory] [InlineData]` for parameterised cases     |

### 8.3 Test Pattern — GWT (Given / When / Then)

```
// given  — build input, configure mocks, construct expected exception
// when   — invoke the service method
// then   — assert result / exception, verify broker interactions
```

### 8.4 Test Implementation Order

Tests **must** be written and committed in the following strict order. This ordering ensures
each category builds upon the prior one:

1. **Happy Path** — `ShouldAdd{Entity}Async`
2. **Structural Validations** — null entity check (`ShouldThrowValidationExceptionOnAddIf{Entity}IsNullAndLogItAsync`)
3. **Logical Validations** — property-level checks using `[Theory] [InlineData]` (`ShouldThrowValidationExceptionOnAddIf{Entity}IsInvalidAndLogItAsync`)
4. **External Dependency Validation Exceptions** — `BadRequest` → `Conflict`
5. **External Critical Dependency Exceptions** — `Unauthorized` → `Forbidden` → `NotFound` → `UrlNotFound`
6. **External Non-Critical Dependency Exceptions** — `InternalServerError` → `ServiceUnavailable`
7. **Transport-Level Exception** — `HttpRequestException`
8. **Catch-All Service Exception** — `Exception`

For storage-based services, steps 4–7 are replaced with the corresponding SQL/EF exceptions
(`DuplicateKeyException`, `DbUpdateException`, `SqlException`).

> **Rule — Test Verification Before Commit:** Each FAIL commit must have the test
> **actually running and failing**. Each PASS commit must have **all** tests
> running and passing. Never commit a FAIL without verifying the test runner
> reports a genuine failure. See [Section 12.1.3 — Commits](#1213-commits) for details.

---

## 9. Key Libraries

| Package                | Purpose                                           |
| ---------------------- | ------------------------------------------------- |
| `Xeption`              | Enhanced exceptions with data aggregation         |
| `EFxceptions`          | EF Core wrapper that throws meaningful exceptions |
| `RESTFulSense`         | HTTP client wrapper for external API brokers      |
| `Moq`                  | Mock framework for unit tests                     |
| `FluentAssertions`     | Readable assertion syntax                         |
| `DeepCloner`           | Value-based deep cloning of test objects           |
| `Tynamix.ObjectFiller` | Random test data generation                       |
| `xunit`                | Unit test framework                               |

---

## Exception Handling Principles

### 0. Scope

These rules govern exception design, localisation, categorisation, propagation, and testing across all service layers.

---

### 1. Localisation (MANDATORY)

0. External (non-local) exceptions MUST be localised at the boundary (Foundation).
1. Native exceptions (SQL, HTTP, SDK) MUST NOT cross service boundaries.
2. Localisation MUST convert native exceptions into domain-specific exceptions.

**Data Preservation (MANDATORY)**

3. ALL relevant data from the external exception MUST be copied to the local exception:
   - `Data` dictionary
   - Constraint / validation metadata
   - Identifiers / keys

4. The localised exception MUST carry this data so that:
   - The **immediate inner exception** contains full validation detail after categorisation.

---

### 2. Categorisation

0. All exceptions MUST be categorised into one of:
   - Validation
   - DependencyValidation
   - Dependency
   - Service

1. Categorisation defines upstream handling and exposer mapping.

---

### 3. Propagation (Unwrap / Rewrap)

Each service layer MUST:

0. Catch downstream exceptions  
1. UNWRAP the categorical exception  
2. PRESERVE the LOCAL exception  
3. REWRAP into its OWN categorical exception  

This prevents leakage of lower-layer concerns and enforces layer contracts.

---

### 4. Inner Exception Preservation (MANDATORY)

0. The original local exception MUST always be preserved as the inner exception.
1. No layer may discard or replace the local exception.

This guarantees:
- Traceability
- Correct exposer mapping (e.g. HTTP Conflict / FailedDependency)
- Retention of validation data

---

### 5. Layer Responsibilities

#### Foundation
0. Localise external exceptions  
1. Populate local exception data  
2. Categorise into Validation / DependencyValidation / Dependency / Service  

#### Processing
0. MUST ONLY handle categorised exceptions  
1. MUST NOT depend on foundation exception types  
2. MUST rewrap into processing-level exceptions  

#### Orchestration
0. MUST handle exceptions from all dependencies  
1. MUST unify into a single categorical exception per type  
2. MUST unwrap and preserve inner exceptions  

---

### 6. Catch-All (MANDATORY)

0. Every service MUST implement:
   - `catch (Exception)`
1. MUST map to ServiceException

This prevents leakage of unknown failures.

---

### 7. Logging

0. Each layer MUST log BEFORE rethrowing  
1. MUST log the categorised exception only  

---

### 8. Testing

#### Orchestration Exception Tests

0. SHOULD use `[Theory]`  
1. MUST cover multiple dependency exception types in a single test  
2. MUST avoid duplication  

---

### 9. Design Intent

These rules ensure:

0. Full abstraction from external systems  
1. Stable layer contracts  
2. Simplified exposer logic  
3. Complete validation visibility at the local exception level  

---

## Exception Handling Cross-References (Enforcement)

### Validation Testing Alignment

0. All validation tests MUST align with Exception Handling Principles:
   - Localisation MUST be verified (no native exceptions exposed)
   - Data preservation MUST be verified on local exceptions
   - Validation exceptions MUST contain full error details in inner exception
   - From processing service layer upwards, validation exceptions and dependency validation exceptions from its dependencies rewrap to [Entity][Layer]DependencyValidationExceptions 
   - From processing service layer upwards, dependency exceptions and service exceptions from its dependencies rewrap to [Entity][Layer]DependencyExceptions

1. Validation tests MUST assert:
   - Correct local exception type
   - Correct categorised exception type
   - Inner exception contains validation data (Data dictionary populated)

---

### Foundation Exception Tests (Enforcement)

0. Tests MUST verify localisation:
   - Native exception → Local exception → Categorised exception

1. Tests MUST verify:
   - External exception data is copied to local exception
   - Local exception is preserved as inner exception

2. Tests MUST NOT allow:
   - Native exception leakage
   - Missing Data dictionary propagation

---

### Processing Exception Tests (Enforcement)

0. Tests MUST assert:
   - Rewrapping into processing-level exception
   - Inner exception preservation

1. Tests SHOULD:
   - Use `[Theory]` to test multiple dependency validation exceptions of the same type in one test   
   - Use `[Theory]` to test multiple dependency exceptions of the same type in one test
   - Avoid duplication by testing multiple cases in a single test method

---

### Orchestration Exception Tests (Enforcement)

0. Tests MUST assert:
   - Rewrapping into orchestration-level exception   
   - Inner exception is preserved (local exception)
   - Categorical exception is replaced at orchestration level

1. Tests SHOULD:
   - Use `[Theory]` to test multiple dependency validation exceptions of the same type in one test   
   - Use `[Theory]` to test multiple dependency exceptions of the same type in one test
   - Avoid duplication by testing multiple cases in a single test method

---

### Catch-All Enforcement

0. Tests MUST verify:
   - Unknown exceptions are mapped to ServiceException
   - No raw Exception escapes any service layer

---

### Logging Enforcement

0. Tests MUST verify:
   - Logging occurs before exception is thrown
   - Logged exception is the categorised exception

---

### Design Integrity Rule

0. Any violation of exception handling principles MUST be treated as:
   - A design defect
   - A test failure
   - A review blocker
