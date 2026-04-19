# The Standard Testing — Validation Checklist

Run this checklist before committing any test code or approving a PR.
Each item is binary: PASS or FAIL.

---

## TDD DISCIPLINE

- [ ] **test-001** The failing test was written before the implementation.
- [ ] **test-002** The test was verified to actually fail (test runner shows red) before the FAIL commit.
- [ ] **test-003** Only the minimum implementation to pass the test was written.
- [ ] **test-004** The full relevant test suite was verified to pass before the PASS commit.
- [ ] **test-005** Refactoring did not change behavior — all tests still pass.

---

## TEST IMPLEMENTATION ORDER

- [ ] **test-010** Happy path test is implemented first.
- [ ] **test-011** Structural validation tests come before logical validation tests.
- [ ] **test-012** Logical validation tests come before external validation tests.
- [ ] **test-013** External validation tests come before dependency validation tests.
- [ ] **test-110** For foundation Add operations, the exact 13-step order is followed.

---

## TEST QUALITY

- [ ] **test-030** Exact broker calls are verified (`Times.Once` / `Times.Never`).
- [ ] **test-031** Every test ends with `VerifyNoOtherCalls()` on all mocks.
- [ ] **test-032** Logging broker verification is present for all error cases.
- [ ] **test-034** Deep cloning is used to isolate input/expected/actual objects.
- [ ] **test-035** All test data is randomized via ObjectFiller — no hard-coded values.
- [ ] **test-036** Exception equality uses `Xeption.SameExceptionAs()`.

---

## VALIDATION TEST COVERAGE

- [ ] **test-020** Null entity test exists and verifies circuit-breaking (throws immediately).
- [ ] **test-021** Invalid fields test collects ALL errors before asserting (continuous validation).
- [ ] **test-022** Exception data uses `UpsertDataList` / `AddData`.

---

## FILE STRUCTURE

- [ ] **test-090** Tests mirror the partial-class split of the service: root + Logic + Validations + Exceptions.
- [ ] **test-091** Root test file contains only: constructor, mocks, helper methods, filler configuration.
- [ ] **test-092** Happy-path tests are in the Logic file.
- [ ] **test-093** Validation failure tests are in the Validations file.
- [ ] **test-094** Exception tests are in the Exceptions file.

---

## CONVENTIONS

- [ ] **test-100** All tests use GWT pattern with `// given`, `// when`, `// then` comments.
- [ ] **test-101** All dependencies are mocked with Moq.
- [ ] **test-102** Assertions use FluentAssertions (`Should().BeEquivalentTo()`).
- [ ] **test-103** Single-case tests use `[Fact]`; parameterized tests use `[Theory][InlineData]`.
- [ ] **test-104** Test names follow `Should{Action}Async` or `ShouldThrow{Exception}On{Action}If{Condition}AndLogItAsync`.

---

## SERVICE-TYPE SPECIFIC

- [ ] **test-060** No mock-sequence order assertions in aggregation service tests.
- [ ] **test-073** Every controller endpoint has an acceptance test.
- [ ] **test-074** Acceptance test data is cleaned up after each run.
- [ ] **test-080** Core UI components are test-driven.
- [ ] **test-083** Each core component integrates with exactly one view service.

---

## RESULT

| Category | PASS / FAIL |
|---|---|
| TDD Discipline | |
| Test Implementation Order | |
| Test Quality | |
| Validation Coverage | |
| File Structure | |
| Conventions | |
| Service-Type Specific | |

**Overall: PASS only when every row is PASS.**
