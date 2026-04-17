// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant Validation Test
// File: StudentServiceTests.Validations.Add.cs
// Demonstrates: null check (circuit-breaking), invalid fields (continuous),
// exception comparison, no broker calls verification.

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace MyProject.Tests.Unit.Services.Foundations.Students
{
    public partial class StudentServiceTests
    {
        // test-011: Structural validation — null entity (circuit-breaking)
        // test-104: ShouldThrow{Exception}On{Add}If{Entity}IsNull naming
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfStudentIsNullAndLogItAsync()
        {
            // given
            Student nullStudent = null;

            var nullStudentException =
                new NullStudentException(message: "Student is null.");

            var expectedStudentValidationException =
                new StudentValidationException(
                    message: "Student validation error occurred, fix the errors and try again.",
                    innerException: nullStudentException);

            // when
            ValueTask<Student> addStudentTask =
                this.studentService.AddStudentAsync(nullStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(
                    addStudentTask.AsTask);

            // then
            // test-036: Xeption.SameExceptionAs for exception equality
            actualStudentValidationException.Should().BeEquivalentTo(
                expectedStudentValidationException);

            // test-032: Verify logging
            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))),
                        Times.Once);

            // test-030/test-031: No broker calls when validation fails
            this.storageBrokerMock.Verify(broker =>
                broker.InsertStudentAsync(It.IsAny<Student>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        // test-011: Structural + Logical validation (continuous — all invalid fields collected)
        // test-103: [Theory] [InlineData] for parameterized cases
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfStudentIsInvalidAndLogItAsync(
            string invalidStudentName)
        {
            // given
            var invalidStudent = new Student
            {
                Name = invalidStudentName  // triggers invalid name rule
            };

            var invalidStudentException =
                new InvalidStudentException(
                    message: "Invalid student. Please correct the errors and try again.");

            invalidStudentException.AddData(
                key: nameof(Student.Id),
                values: "Id is required");

            invalidStudentException.AddData(
                key: nameof(Student.Name),
                values: "Text is required");

            invalidStudentException.AddData(
                key: nameof(Student.CreatedDate),
                values: "Date is required");

            invalidStudentException.AddData(
                key: nameof(Student.UpdatedDate),
                values: new[]
                {
                    "Date is required",
                    $"Date is not the same as {nameof(Student.CreatedDate)}"
                });

            var expectedStudentValidationException =
                new StudentValidationException(
                    message: "Student validation error occurred, fix the errors and try again.",
                    innerException: invalidStudentException);

            // when
            ValueTask<Student> addStudentTask =
                this.studentService.AddStudentAsync(invalidStudent);

            StudentValidationException actualStudentValidationException =
                await Assert.ThrowsAsync<StudentValidationException>(
                    addStudentTask.AsTask);

            // then
            // test-021: Continuous validation — all fields collected
            actualStudentValidationException.Should().BeEquivalentTo(
                expectedStudentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStudentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStudentAsync(It.IsAny<Student>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
