# The Standard Testing — Rules

## TDD DOCTRINE

**test-001** [ERROR] Write the failing test first. Never write implementation before a failing test exists.
**test-002** [ERROR] Verify the test actually fails before committing a FAIL commit.
**test-003** [ERROR] Write the minimum implementation required to pass the test. No more.
**test-004** [ERROR] Verify the full relevant test suite passes before committing a PASS commit.
**test-005** [ERROR] Refactor without changing behavior. All tests must still pass after refactoring.

## VALIDATION TEST ORDER

**test-010** [ERROR] Test happy path first.
**test-011** [ERROR] Test structural validations second (null, empty, whitespace, default).
**test-012** [ERROR] Test logical validations third (business rules).
**test-013** [ERROR] Test external validations fourth (existence checks).
**test-014** [ERROR] Test dependency validations fifth (broker-specific failures).
**test-015** [ERROR] Test dependency exceptions sixth (storage/API errors).
**test-016** [ERROR] Test service exceptions seventh (catch-all unexpected errors).
**test-017** [ERROR] All services must validate their input parameters before executing any business logic.
**test-018** [ERROR] Services must validate output data when it is reused within the same routine.
**test-019** [ERROR] Validation order must be: structural → logical → external → dependency.

## VALIDATION TYPES

**test-020** [ERROR] Structural validations must break immediately (circuit-breaking): null entity → throw immediately, do not continue.
**test-021** [ERROR] Continuous validations must collect ALL invalid fields before throwing — never throw on the first failure when multiple fields can be invalid.
**test-022** [ERROR] Continuous validations use upsertable exception data (`UpsertDataList`).
**test-023** [ERROR] Continuous validations use dynamic rules: `{ Condition = ..., Message = ... }`.
**test-024** [ERROR] Hybrid continuous validations: validate parent object before validating child properties.
**test-025** [ERROR] Validations must prevent deterministic failures before reaching external dependencies or storage.
**test-026** [ERROR] Validation must not rely on downstream systems (e.g., databases) to enforce constraints.
**test-027** [ERROR] Validation errors must be aggregated and thrown once using a single validation exception.

## FOUNDATION SERVICE TESTS

**test-030** [ERROR] Always verify exact broker calls (Times.Once, Times.Never).
**test-031** [ERROR] Always end with `VerifyNoOtherCalls()` on all mocks.
**test-032** [ERROR] Always verify the logging broker received the expected error.
**test-033** [ERROR] Keep validation and exception behaviors local and explicit — no shared helpers.
**test-034** [ERROR] Use deep cloning (DeepCloner) to prevent shared state between input, expected, and actual objects.
**test-035** [ERROR] Use randomized data (Tynamix.ObjectFiller) — never use hard-coded test values.
**test-036** [ERROR] Use `Xeption.SameExceptionAs()` for exception equality comparison.
**test-037** [ERROR] Foundation services are the primary validation boundary and must enforce all deterministic validations.
**test-038** [ERROR] Foundation validations must be equal to or stricter than storage constraints.
**test-039** [ERROR] Foundation services must validate required fields, length, format, and all persistence constraints.

## PROCESSING SERVICE TESTS

**test-040** [ERROR] Test higher-order logic, not primitive broker details.
**test-041** [ERROR] Only validate fields the processing service actually uses.
**test-042** [ERROR] Test shifter operations: entity → bool, entity → count.
**test-043** [ERROR] Test combination operations: retrieve+add (EnsureExists), retrieve+modify (Upsert).
**test-044** [ERROR] Test exception mapping from foundation exceptions to processing exceptions.
**test-045** [ERROR] Processing services must perform used-data-only validation.
**test-046** [ERROR] Processing services must validate only data required for their logic.
**test-047** [ERROR] Processing services must not revalidate full entity constraints handled by foundation services.
**test-048** [ERROR] Processing services must validate required identifiers and null checks.
**test-049** [ERROR] Processing services must rely on foundation services for full validation enforcement.

## ORCHESTRATION SERVICE TESTS

**test-050** [ERROR] Test multi-entity flow combinations.
**test-051** [ERROR] Test call order when the flow depends on it — use explicit sequence or natural order.
**test-052** [ERROR] Prefer natural order (input/output encoding) over mock-sequence style verification.
**test-053** [ERROR] Verify orchestration-level exception wrapping and unwrapping.
**test-054** [ERROR] Orchestration services must perform structural validation only.
**test-055** [ERROR] Orchestration services must validate input existence and required identifiers.
**test-056** [ERROR] Orchestration services must not perform full entity validation.
**test-057** [ERROR] Orchestration services must delegate validation to downstream services.
**test-058** [ERROR] Orchestration services must validate only data required for their logic.

## AGGREGATION SERVICE TESTS

**test-060** [ERROR] Do NOT test dependency call order in aggregation service tests.
**test-061** [ERROR] Do NOT use mock-sequence style order assertions for aggregation services.
**test-062** [ERROR] Test only basic structural validations and the exposure-level aggregation behavior.
**test-063** [ERROR] Aggregation services must perform minimal validation.
**test-064** [ERROR] Aggregation services must validate only input existence and required properties.
**test-065** [ERROR] Aggregation services must not perform business or domain validation.

## CONTROLLER/PROTOCOL TESTS

**test-070** [ERROR] Unit-test every success code mapping (200 for GET/PUT/DELETE, 201 for POST).
**test-071** [ERROR] Unit-test every error mapping: Validation → 400, DependencyValidation → 400, CriticalDependency → 500, Dependency → 500, Service → 500.
**test-072** [ERROR] Unit-test authorization/authentication failure mappings.
**test-073** [ERROR] Acceptance-test every endpoint.
**test-074** [ERROR] Clean up all test data after acceptance tests run.
**test-075** [ERROR] Emulate external resources not owned by the microservice during acceptance tests.

## UI COMPONENT TESTS

**test-080** [ERROR] Core components are test-driven.
**test-081** [ERROR] Test element existence, properties, and actions for core components.
**test-082** [ERROR] Test styles when they are part of the component contract.
**test-083** [ERROR] Core components integrate with exactly one view service.
**test-084** [WARNING] Base components: test exposed APIs and wrapper behavior, not internal implementation.
**test-085** [WARNING] Pages generally do not require unit tests.

## TEST FILE STRUCTURE

**test-090** [ERROR] Mirror the partial-class split in tests: root + Logic + Validations + Exceptions files.
**test-091** [ERROR] Root test file (`{Entity}ServiceTests.cs`) contains setup, mocks, and helper methods.
**test-092** [ERROR] `{Entity}ServiceTests.Logic.{Method}.cs` contains happy-path tests.
**test-093** [ERROR] `{Entity}ServiceTests.Validations.{Method}.cs` contains validation failure tests.
**test-094** [ERROR] `{Entity}ServiceTests.Exceptions.{Method}.cs` contains dependency/service exception tests.

## TEST CONVENTIONS

**test-100** [ERROR] Use GWT pattern: Given / When / Then with inline comments.
**test-101** [ERROR] Mock all dependencies using Moq.
**test-102** [ERROR] Use FluentAssertions for readable assertions (`Should().BeEquivalentTo()`).
**test-103** [ERROR] Use xUnit: `[Fact]` for single cases, `[Theory] [InlineData]` for parameterized cases.
**test-104** [ERROR] Test method names: `Should{Action}Async` for success cases; `ShouldThrow{Exception}On{Action}If{Condition}AndLogItAsync` for error cases.

## EXACT IMPLEMENTATION ORDER (Foundation Service Add)

**test-110** [ERROR] Test implementation order for Add must follow:
  0. `ShouldAdd{Entity}Async`
  1. `ShouldThrowValidationExceptionOnAddIf{Entity}IsNullAndLogItAsync`
  2. `ShouldThrowValidationExceptionOnAddIf{Entity}IsInvalidAndLogItAsync`
  3. `ShouldThrowValidationExceptionOnAddIf{Entity}HasIsInvalidLengthPropertiesAndLogItAsync` (if applicable)
  4. `ShouldThrowDependencyValidationExceptionOnAddIfBadRequestErrorOccursAndLogItAsync`
  5. `ShouldThrowDependencyValidationExceptionOnAddIfConflictErrorOccursAndLogItAsync`
  6. `ShouldThrowCriticalDependencyExceptionOnAddIfUnauthorizedErrorOccursAndLogItAsync`
  7. `ShouldThrowCriticalDependencyExceptionOnAddIfForbiddenErrorOccursAndLogItAsync`
  8. `ShouldThrowCriticalDependencyExceptionOnAddIfNotFoundErrorOccursAndLogItAsync`
  9. `ShouldThrowCriticalDependencyExceptionOnAddIfUrlNotFoundErrorOccursAndLogItAsync`
  10. `ShouldThrowDependencyExceptionOnAddIfInternalServerErrorOccursAndLogItAsync`
  11. `ShouldThrowDependencyExceptionOnAddIfServiceUnavailableErrorOccursAndLogItAsync`
  12. `ShouldThrowCriticalDependencyExceptionOnAddIfHttpRequestErrorOccursAndLogItAsync`
  13. `ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync`

**Note:** Similar patterns with layer-appropriate variations apply to other methods (Modify, Remove, Retrieve) and other service layers (Processing, Orchestration, Aggregation).


## EXCEPTION TESTING

**test-111** [ERROR] All external/native exceptions must be localized into custom exceptions before leaving the service boundary.
**test-112** [ERROR] Localized exceptions must preserve external/native exceptions as the InnerException on the localized exception.
**test-113** [ERROR] Localized exceptions must carry the Data collection from the original exception.
**test-114** [ERROR] Services must only catch dependency-level exceptions relevant to their layer.
**test-115** [ERROR] Catch-all exceptions must be mapped to ServiceException.
**test-116** [ERROR] Exceptions must be logged before being thrown when logging is applicable.
**test-117** [ERROR] Exception handling must use a centralized TryCatch pattern per service.
**test-121** [ERROR] From processing service layer upwards, validation exceptions from dependencies must be rewrapped as `[Entity][Layer]DependencyValidationException`.
**test-122** [ERROR] From processing service layer upwards, dependency validation exceptions from dependencies must be rewrapped as `[Entity][Layer]DependencyValidationException`.
**test-123** [ERROR] From processing service layer upwards, dependency exceptions from dependencies must be rewrapped as `[Entity][Layer]DependencyException`.
**test-124** [ERROR] From processing service layer upwards, service exceptions from dependencies must be rewrapped as `[Entity][Layer]DependencyException`.

## VALIDATION EXCEPTIONS

**test-118** [ERROR] Validation failures must result in ValidationException.
**test-119** [ERROR] Validation exceptions must include aggregated error details in the Data dictionary.
**test-120** [ERROR] Validation exceptions must follow localisation and categorisation rules.
