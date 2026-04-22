# Good Example: Commit Messages (FAIL/PASS Discipline)

## Pattern for TDD Commits

```
[FAIL]: [Test method name]
[PASS]: [Implementation description]
```

---

## Foundation Service Add — Full Commit Sequence

### Step 0: Happy path

```
[FAIL]: ShouldAddStudentAsync
```
*Test is written, run, verified to fail (no implementation yet). Committed.*

```
[PASS]: AddStudentAsync — happy path implementation
```
*Minimum implementation to pass `ShouldAddStudentAsync`. All tests pass. Committed.*

---

### Step 1: Null validation

```
[FAIL]: ShouldThrowValidationExceptionOnAddIfStudentIsNullAndLogItAsync
```
*Null check test written and verified to fail. Committed.*

```
[PASS]: AddStudentAsync — null student validation
```
*`ValidateStudentIsNotNull` added. Test passes. All tests pass. Committed.*

---

### Step 2: Invalid fields validation

```
[FAIL]: ShouldThrowValidationExceptionOnAddIfStudentIsInvalidAndLogItAsync
```
*Invalid fields test written for Id, Name, CreatedDate, UpdatedDate. Verified to fail. Committed.*

```
[PASS]: AddStudentAsync — invalid student fields validation
```
*Continuous validation for all fields added. All tests pass. Committed.*

---

### Step 3: Dependency validation (BadRequest)

```
[FAIL]: ShouldThrowDependencyValidationExceptionOnAddIfBadRequestErrorOccursAndLogItAsync
```

```
[PASS]: AddStudentAsync — BadRequest dependency validation exception handling
```

---

## Non-TDD Commit (PATCH)

```
PATCH: Fix null reference in student name formatter
```

No FAIL/PASS prefix needed for non-TDD patches.

---

## Infrastructure Commit

```
INFRA: Initial project setup — solution, projects, CI pipeline
```

---

## Rules Reminder

- Every `[FAIL]` commit must be preceded by running the test and seeing RED.
- Every `[PASS]` commit must be preceded by running the FULL suite and seeing GREEN.
- Never combine multiple tests in one FAIL commit.
- Never combine multiple implementation units in one PASS commit.
