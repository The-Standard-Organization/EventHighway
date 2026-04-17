# The Standard Testing — Anti-Patterns

---

## AP-TEST-001: Implementation Before Test

**What it is:** Writing the service implementation before writing a failing test.

**Example:**
```
// Day 1: Engineer writes StudentService.AddStudentAsync()
// Day 1: Engineer then writes ShouldAddStudentAsync test
```

**Why harmful:** The test is not a design tool — it becomes a post-hoc documentation exercise. The test is likely to pass on the first run (no red phase), meaning the TDD discipline was bypassed. Tests written after implementation often miss edge cases.

**How to fix:** Write the test first. Run it. Verify it fails (red). Then write the minimum implementation. Verify it passes (green).

---

## AP-TEST-002: Hard-Coded Test Values

**What it is:** Using literal values in test setup instead of randomized data.

**Example:**
```csharp
var student = new Student
{
    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), // hard-coded
    Name = "John"  // hard-coded
};
```

**Why harmful:** Hard-coded values create the illusion of a passing test for only one specific state. The implementation may have logic that only works for "John" or a specific ID. Randomized data catches these hidden assumptions.

**How to fix:**
```csharp
Student randomStudent = CreateRandomStudent(); // uses ObjectFiller
```

---

## AP-TEST-003: Missing VerifyNoOtherCalls

**What it is:** Tests that verify expected calls but do not verify that NO other unexpected calls occurred.

**Example:**
```csharp
this.storageBrokerMock.Verify(broker =>
    broker.InsertStudentAsync(inputStudent), Times.Once);
// MISSING: this.storageBrokerMock.VerifyNoOtherCalls();
```

**Why harmful:** Without `VerifyNoOtherCalls()`, a future implementation change could add an extra broker call (e.g., logging to storage, calling another broker), and the test would still pass. This allows silent regressions.

**How to fix:** Always end every test with `VerifyNoOtherCalls()` on all mocks.

---

## AP-TEST-004: Missing Logging Verification

**What it is:** Error-case tests that do not verify the logging broker received the exception.

**Example:**
```csharp
// when
await Assert.ThrowsAsync<StudentValidationException>(...);

// then
// MISSING: verify logging broker called
this.storageBrokerMock.VerifyNoOtherCalls();
this.loggingBrokerMock.VerifyNoOtherCalls(); // will pass vacuously if no LogError call expected
```

**Why harmful:** Logging is a critical observable behavior in Standard-compliant systems. If logging is broken, production errors become invisible. Without verifying it in tests, a refactor could silently remove logging and no test would catch it.

**How to fix:**
```csharp
this.loggingBrokerMock.Verify(broker =>
    broker.LogError(It.Is(SameExceptionAs(expectedStudentValidationException))),
        Times.Once);
```

---

## AP-TEST-005: Shared Object References Between Input and Expected

**What it is:** Using the same object reference for input, storage response, and expected value.

**Example:**
```csharp
Student inputStudent = CreateRandomStudent();
// VIOLATION: input, storage, and expected all point to the same object
this.storageBrokerMock.Setup(b => b.InsertStudentAsync(inputStudent))
    .ReturnsAsync(inputStudent); // same reference

Student expectedStudent = inputStudent; // same reference

Student actualStudent = await this.studentService.AddStudentAsync(inputStudent);

actualStudent.Should().BeEquivalentTo(expectedStudent); // passes vacuously
```

**Why harmful:** If the service modifies the input object in-place, all three references see the modification. The test cannot detect incorrect mutations.

**How to fix:**
```csharp
Student inputStudent = CreateRandomStudent();
Student storageStudent = inputStudent.DeepClone();
Student expectedStudent = storageStudent.DeepClone();
```

---

## AP-TEST-006: Aggregation Test Asserting Call Order

**What it is:** Using `MockSequence` or similar mechanisms in aggregation service tests.

**Example:**
```csharp
// In aggregation service test — VIOLATION
var sequence = new MockSequence();
studentServiceMock.InSequence(sequence).Setup(s => s.AddStudentAsync(It.IsAny<Student>()));
courseServiceMock.InSequence(sequence).Setup(s => s.AddCourseAsync(It.IsAny<Course>()));
```

**Why harmful:** Aggregation services have no ordering contract. Testing order here creates a brittle test that breaks if the implementation optimizes by reordering calls (e.g., running them in parallel). The test now couples to implementation details rather than contract behavior.

**How to fix:** Test that the correct service calls were made with the correct arguments. Do not assert order.

---

## AP-TEST-007: Tests Out of Standard Order

**What it is:** Writing exception tests before validation tests, or validation tests before the happy path.

**Example:**
```
// VIOLATION order in file:
ShouldThrowDependencyExceptionOnAddIfInternalServerErrorOccurs  // exception test first
ShouldThrowValidationExceptionOnAddIfStudentIsNull              // validation test second
ShouldAddStudentAsync                                           // happy path last
```

**Why harmful:** The Standard's test order reflects the order of comprehension — you understand what the system does (happy path) before understanding what it guards against (validations) and what can go wrong (exceptions). Reversed order creates confusion and makes code review harder.

**How to fix:** Follow the exact 13-step order for foundation service tests. Always happy path first.

---

## AP-TEST-008: Continuous Validation Test Stops at First Error

**What it is:** A validation test that only verifies one field error, not all errors simultaneously.

**Example:**
```csharp
// Test only expects Id error — misses Name, CreatedDate, UpdatedDate
invalid[Entity]Exception.AddData(
    key: nameof(Student.Id),
    values: "Id is required");
// Missing AddData calls for Name, CreatedDate, UpdatedDate
```

**Why harmful:** Continuous validation must collect all errors. If the test only verifies one, you cannot tell if the implementation stops after the first error (failing test-021) or collects all errors correctly.

**How to fix:** Build the expected exception with ALL field errors that should be collected, then assert `BeEquivalentTo` against it.
