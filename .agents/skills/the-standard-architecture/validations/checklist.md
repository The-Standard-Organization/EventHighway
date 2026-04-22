# The Standard Architecture — Validation Checklist

Run this checklist before approving any architecture design or code review.
Each item is binary: PASS or FAIL.

---

## BROKERS

- [ ] **arch-001** Every broker implements a local interface.
- [ ] **arch-002** No flow control (`if`, `switch`, `for`, `while`) exists inside any broker method.
- [ ] **arch-003** No exception handling exists in any broker — exceptions propagate raw.
- [ ] **arch-004** Broker configuration (connection strings, API keys) is injected through the constructor only.
- [ ] **arch-006** Broker method names use infrastructure language (Insert, Select, Update, Delete, Post, Get, Put).
- [ ] **arch-007** No broker calls another entity broker.
- [ ] **arch-008** Each broker wraps exactly one external resource.
- [ ] **arch-009** All broker methods are async (return `Task` or `ValueTask`).

---

## FOUNDATION SERVICES

- [ ] **arch-020** Foundation service inputs and outputs are the same entity type.
- [ ] **arch-021** Foundation service interacts with exactly one entity type.
- [ ] **arch-022** Foundation service method names use business language (Add, Retrieve, Modify, Remove).
- [ ] **arch-023** All inputs are validated before delegating to a broker.
- [ ] **arch-024** Structural validations (null, empty, default) run first and break immediately.
- [ ] **arch-025** Logical validations run after structural validations.
- [ ] **arch-026** External validations (existence checks) run after logical validations.
- [ ] **arch-027** Dependency validations cover broker-specific failure conditions.
- [ ] **arch-028** Continuous validations collect all invalid fields before throwing (upsertable exception data).
- [ ] **arch-029** All broker exceptions are caught, localized, categorized, and logged.
- [ ] **arch-030** Exception types follow the five Standard categories: Validation, DependencyValidation, CriticalDependency, Dependency, Service.
- [ ] **arch-031** No unwanted broker calls occur (verified in tests).

---

## PROCESSING SERVICES

- [ ] **arch-040** Processing service depends on exactly one foundation service.
- [ ] **arch-041** Only fields actually used are validated.
- [ ] **arch-045** Foundation exceptions are unwrapped and re-wrapped as processing exceptions.

---

## ORCHESTRATION SERVICES

- [ ] **arch-050** Service has 2-3 dependencies (Florance Pattern).
- [ ] **arch-052** Call order is explicitly enforced when flow correctness depends on it.
- [ ] **arch-054** All dependency exceptions are unwrapped and re-wrapped.

---

## AGGREGATION SERVICES

- [ ] **arch-060** No call order validation exists.
- [ ] **arch-061** No mock-sequence style assertions exist.
- [ ] **arch-062** Only basic structural validations are present.

---

## EXPOSERS

- [ ] **arch-070** No business logic exists in any controller.
- [ ] **arch-071** Each entity has exactly one controller.
- [ ] **arch-072** Routes follow `/api/[entities]` convention (plural lowercase).
- [ ] **arch-073** Correct HTTP codes: 200 for GET/PUT/DELETE, 201 for POST.
- [ ] **arch-074** Correct error codes: 400 for validation, 500 for dependency/service errors.

---

## GENERAL ARCHITECTURE

- [ ] **arch-080** No same-layer service calls (no Foundation → Foundation).
- [ ] **arch-081** No service calls infrastructure directly (must use brokers).
- [ ] **arch-082** Dependency flow is strictly Exposer → Service → Broker → External Resource.
- [ ] **arch-083** Every service type is declared and tested independently.

---

## RESULT

| Layer | PASS / FAIL |
|---|---|
| Brokers | |
| Foundation Services | |
| Processing Services | |
| Orchestration Services | |
| Aggregation Services | |
| Exposers | |
| General Architecture | |

**Overall: PASS only when every row is PASS.**
