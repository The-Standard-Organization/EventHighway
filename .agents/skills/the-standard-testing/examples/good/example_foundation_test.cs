// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant Foundation Service Test
// File: StudentServiceTests.Logic.Add.cs
// Demonstrates: GWT, fat arrow for setup, randomized data, deep clone,
// exact broker verification, VerifyNoOtherCalls.

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace MyProject.Tests.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        // test-010: Happy path first
        // test-100: GWT pattern
        // test-104: Should{Action}Async naming
        [Fact]
        public async Task ShouldAddStudentAsync()
        {
            // given
            // test-035: Randomized data via ObjectFiller
            Student randomStudent = CreateRandomStudent();
            Student inputStudent = randomStudent;

            // test-034: Deep clone to isolate expected from actual
            Student storageStudent = inputStudent.DeepClone();
            Student expectedStudent = storageStudent.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertStudentAsync(inputStudent))
                    .ReturnsAsync(storageStudent);

            // when
            Student actualStudent =
                await this.studentService.AddStudentAsync(inputStudent);

            // then
            // test-102: FluentAssertions
            actualStudent.Should().BeEquivalentTo(expectedStudent);

            // test-030: Verify exact broker calls
            this.storageBrokerMock.Verify(broker =>
                broker.InsertStudentAsync(inputStudent),
                    Times.Once);

            // test-031: No unwanted calls
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// GOOD EXAMPLE: Root test file (StudentServiceTests.cs)
// Contains setup, mocks, and helper methods.
// ---------------------------------------------------------------

namespace MyProject.Tests.Unit.Services.Foundations.Students
{
    // test-090, test-091: Root file contains setup, mocks, helpers
    public partial class StudentServiceTests
    {
        // test-101: Mock all dependencies using Moq
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IStudentService studentService;

        public StudentServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.studentService = new StudentService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        // test-035: Helper using ObjectFiller for random data
        private Student CreateRandomStudent()
        {
            return CreateStudentFiller(dates: GetRandomDateTimeOffset()).Create();
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            DateTimeOffset.UtcNow.AddDays(GetRandomNegativeNumber());

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static Filler<Student> CreateStudentFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Student>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}
