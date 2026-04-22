// ---------------------------------------------------------------
// BAD EXAMPLE: Non-Standard Test Violations
// Each violation annotated with the rule it breaks.
// ---------------------------------------------------------------

namespace MyProject.Tests.Bad
{
    public class BadStudentServiceTests
    {
        // test-001 VIOLATION: Implementation written before test (test doesn't exist first)
        // test-002 VIOLATION: Test not verified to fail before FAIL commit
        // test-035 VIOLATION: Hard-coded test values instead of randomized data
        // test-100 VIOLATION: No GWT pattern comments
        // test-104 VIOLATION: Method name doesn't follow Should{Action}Async convention
        [Fact]
        public async Task TestAddStudent()
        {
            // Hard-coded data — test-035 VIOLATION
            var student = new Student
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "John"
            };

            // No mock setup — incomplete test
            var result = await this.studentService.AddStudentAsync(student);

            // test-102 VIOLATION: Not using FluentAssertions
            Assert.NotNull(result);
            Assert.Equal(student.Id, result.Id);

            // test-030 VIOLATION: No broker call verification
            // test-031 VIOLATION: No VerifyNoOtherCalls()
        }

        // test-010 VIOLATION: Error test written before happy path
        // test-011 VIOLATION: Structural validation test out of order
        [Fact]
        public async Task ShouldThrowIfNull()  // test-104 VIOLATION: incomplete naming
        {
            // test-034 VIOLATION: No deep cloning — same reference used for input/expected
            var nullStudent = new Student();  // not actually null — wrong test intent

            // test-036 VIOLATION: Using Assert.Equal instead of Xeption.SameExceptionAs
            await Assert.ThrowsAsync<Exception>(  // catching base Exception, not specific type
                () => this.studentService.AddStudentAsync(nullStudent));

            // test-032 VIOLATION: Logging not verified
            // test-031 VIOLATION: No VerifyNoOtherCalls()
            // test-030 VIOLATION: No Times.Never verification for broker
        }

        // test-021 VIOLATION: First invalid field throws immediately (not collecting all)
        private void BadContinuousValidation(Student student)
        {
            if (student.Id == Guid.Empty)
                throw new Exception("Id is required");  // stops after first field

            if (string.IsNullOrWhiteSpace(student.Name))
                throw new Exception("Name is required");  // never reached if Id fails

            // Should collect ALL errors and throw once at the end
        }

        // test-060/test-061 VIOLATION: Aggregation test asserting call order
        [Fact]
        public async Task ShouldAggregateInOrder()
        {
            // test-061 VIOLATION: mock sequence in aggregation test
            var sequence = new MockSequence();
            this.studentServiceMock.InSequence(sequence).Setup(...);
            this.courseServiceMock.InSequence(sequence).Setup(...);

            // Aggregation services have no order contract — this test will be brittle
        }

        // test-090/test-093 VIOLATION: Exception tests mixed into Logic file
        // (should be in StudentServiceTests.Exceptions.Add.cs)
        // test-091 VIOLATION: Helpers defined in a test method, not in root test class
        [Fact]
        public async Task ShouldThrowDependencyException()
        {
            // helper inline — should be in root file
            Student CreateTestStudent() => new Student { Id = Guid.NewGuid() };

            var student = CreateTestStudent();
            // ...
        }
    }
}
