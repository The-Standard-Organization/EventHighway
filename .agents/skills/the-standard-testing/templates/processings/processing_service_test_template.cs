// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Processing Service Tests — All three axes
// Replace [Entity] / [Entities] / [Namespace] with actual values.
// Demonstrates: Logic, Validations, and Exceptions files.

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using [Namespace].Models.Foundations.[Entities];
using [Namespace].Models.Processings.[Entities].Exceptions;
using Moq;
using Xeptions;
using Xunit;

// ---------------------------------------------------------------
// File: [Entity]ProcessingServiceTests.Logic.cs
// test-092: Happy-path tests
// ---------------------------------------------------------------

namespace [Namespace].Tests.Unit.Services.Processings.[Entities]
{
    public partial class [Entity]ProcessingServiceTests
    {
        // test-010: Happy path first
        // test-040: Processing service happy path
        // test-043: Combination operations (retrieve+add, retrieve+modify)
        [Fact]
        public async Task ShouldModify[Entity]IfOneExistsAndNotAddAsync()
        {
            // Given
            [Entity] random[Entity] = CreateRandom[Entity]();
            [Entity] storage[Entity] = random[Entity];
            [Entity] modified[Entity] = storage[Entity].DeepClone();
            modified[Entity].UpdatedDate = DateTimeOffset.UtcNow;
            [Entity] updated[Entity] = modified[Entity].DeepClone();
            [Entity] expected[Entity] = updated[Entity];

            this.[entity]ServiceMock.Setup(service =>
                service.Retrieve[Entity]ByIdAsync(modified[Entity].Id))
                    .ReturnsAsync(value: storage[Entity]);

            this.[entity]ServiceMock.Setup(service =>
                service.Modify[Entity]Async(modified[Entity]))
                    .ReturnsAsync(value: updated[Entity]);

            // When
            [Entity] actual[Entity] = await this.[entity]ProcessingService
                .ModifyOrAdd[Entity]Async(modified[Entity]);

            // Then
            actual[Entity].Should().BeEquivalentTo(expected[Entity]);

            this.[entity]ServiceMock.Verify(service =>
                service.Retrieve[Entity]ByIdAsync(modified[Entity].Id),
                    Times.Once);

            this.[entity]ServiceMock.Verify(service =>
                service.Modify[Entity]Async(modified[Entity]),
                    Times.Once);

            this.[entity]ServiceMock.Verify(service =>
                service.Add[Entity]Async(modified[Entity]),
                    Times.Never);

            this.[entity]ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        // test-010: Happy path first
        // test-040: Processing service happy path
        // test-043: Combination operations (retrieve+add, retrieve+modify)
        [Fact]
        public async Task ShouldAdd[Entity]If[Entity]DoesNotExistsAsync()
        {
            // Given
            [Entity] random[Entity] = CreateRandom[Entity]();
            [Entity] input[Entity] = random[Entity];
            [Entity] storage[Entity] = input[Entity].DeepClone();
            [Entity] expected[Entity] = storage[Entity];

            this.[entity]ServiceMock.Setup(service =>
                service.Retrieve[Entity]ByIdAsync(input[Entity].Id))
                    .ReturnsAsync(value: null);

            this.[entity]ServiceMock.Setup(service =>
                service.Add[Entity]Async(input[Entity]))
                    .ReturnsAsync(value: storage[Entity]);

            // When
            await this.[entity]ProcessingService.ModifyOrAdd[Entity]Async(input[Entity]);

            // Then
            this.[entity]ServiceMock.Verify(service =>
                service.Retrieve[Entity]ByIdAsync(input[Entity].Id),
                    Times.Once);

            this.[entity]ServiceMock.Verify(service =>
            service.Add[Entity]Async(input[Entity]),
            Times.Once);

            this.[entity]ServiceMock.Verify(service =>
                service.Modify[Entity]Async(input[Entity]),
                    Times.Never);

            this.[entity]ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: [Entity]ProcessingServiceTests.Exceptions.cs
// test-094: Exception mapping tests
// ---------------------------------------------------------------

namespace [Namespace].Tests.Unit.Services.Processings.[Entities]
{
    public partial class [Entity]ProcessingServiceTests
    {
        // test-015: Processing layer maps Foundation DependencyValidation → Processing DependencyValidation
        // test-044: Processing exception mapping (Foundation → Processing)
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyOrAddIfErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            [Entity] some[Entity] = CreateRandom[Entity]();
            [Entity] input[Entity] = some[Entity];

            var expected[Entity]ProcessingDependencyValidationException =
                new [Entity]ProcessingDependencyValidationException(
                    message: "[Entity] processing dependency validation error occurred, please try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.[entity]ServiceMock.Setup(service =>
                service.Retrieve[Entity]ByIdAsync(input[Entity].Id))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<[Entity]> [entity]ModifyOrAddTask =
                this.[entity]ProcessingService.ModifyOrAdd[Entity]Async(input[Entity]);

            [Entity]ProcessingDependencyValidationException actualException =
                await Assert.ThrowsAsync<[Entity]ProcessingDependencyValidationException>(
                    [entity]ModifyOrAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expected[Entity]ProcessingDependencyValidationException);

            this.[entity]ServiceMock.Verify(service =>
                service.Retrieve[Entity]ByIdAsync(input[Entity].Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expected[Entity]ProcessingDependencyValidationException))),
                         Times.Once);

            this.[entity]ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        // test-016: Processing layer maps Foundation Dependency → Processing Dependency
        // test-044: Processing exception mapping (Foundation → Processing)
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnModifyOrAddIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            [Entity] some[Entity] = CreateRandom[Entity]();
            [Entity] input[Entity] = some[Entity];

            var expected[Entity]ProcessingDependencyException =
                new [Entity]ProcessingDependencyException(
                    message: "[Entity] processing dependency error occurred, please try again.",
                    innerException: dependencyException.InnerException as Xeption);

            this.[entity]ServiceMock.Setup(service =>
                service.Retrieve[Entity]ByIdAsync(input[Entity].Id))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<[Entity]> [entity]ModifyOrAddTask =
                this.[entity]ProcessingService.ModifyOrAdd[Entity]Async(input[Entity]);

            [Entity]ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<[Entity]ProcessingDependencyException>(
                    [entity]ModifyOrAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expected[Entity]ProcessingDependencyException);

            this.[entity]ServiceMock.Verify(service =>
                service.Retrieve[Entity]ByIdAsync(input[Entity].Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expected[Entity]ProcessingDependencyException))),
                         Times.Once);

            this.[entity]ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        // test-017: Processing layer wraps unexpected exceptions in ServiceException
        // test-044: Processing exception mapping (Foundation → Processing)
        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyOrAddIfServiceErrorOccursAsync()
        {
            // given
            [Entity] some[Entity] = CreateRandom[Entity]();
            [Entity] input[Entity] = some[Entity];

            var serviceException = new Exception();

            var failed[Entity]ProcessingServiceException =
                new Failed[Entity]ProcessingServiceException(
                    message: "Failed [Entity] processing service error occurred, please contact support.",
                    innerException: serviceException);

            var expected[Entity]ProcessingServiveException =
                new [Entity]ProcessingServiceException(
                    message: "[Entity] processing service error occurred, please contact support.",
                    innerException: failed[Entity]ProcessingServiceException);

            this.[entity]ServiceMock.Setup(service =>
                service.Retrieve[Entity]ByIdAsync(input[Entity].Id))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<[Entity]> add[Entity]Task =
                this.[entity]ProcessingService.ModifyOrAdd[Entity]Async(input[Entity]);

            [Entity]ProcessingServiceException actualException =
                await Assert.ThrowsAsync<[Entity]ProcessingServiceException>(add[Entity]Task.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expected[Entity]ProcessingServiveException);

            this.[entity]ServiceMock.Verify(service =>
                service.Retrieve[Entity]ByIdAsync(input[Entity].Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expected[Entity]ProcessingServiveException))),
                         Times.Once);

            this.[entity]ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: [Entity]ProcessingServiceTests.Validations.cs
// test-093: Validation failure tests
// ---------------------------------------------------------------

namespace [Namespace].Tests.Unit.Services.Processings.[Entities]
{
    public partial class [Entity]ProcessingServiceTests
    {
        // test-011: Null validation
        // test-020: Null checks before property validation
        // test-041: Processing service validates null input
        [Fact]
        public async Task ShouldThrowValidationExceptionsOnModifyOrAddIf[Entity]ProcessingIsNullAndLogItAsync()
        {
            // given
            [Entity] null[Entity] = null;

            var null[Entity]ProcessingException =
                new Null[Entity]ProcessingException(message: "[Entity] is null.");

            var expected[Entity]ProcessingValidationException =
                new [Entity]ProcessingValidationException(
                    message: "[Entity] processing validation error occurred, please try again.",
                    innerException: null[Entity]ProcessingException);

            // when
            ValueTask<[Entity]> Add[Entity]Task =
                this.[entity]ProcessingService.ModifyOrAdd[Entity]Async(null[Entity]);

            [Entity]ProcessingValidationException actual[Entity]ProcessingValidationException =
                await Assert.ThrowsAsync<[Entity]ProcessingValidationException>(Add[Entity]Task.AsTask);

            //then
            actual[Entity]ProcessingValidationException.Should()
                .BeEquivalentTo(expected[Entity]ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expected[Entity]ProcessingValidationException))),
                        Times.Once);

            this.[entity]ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        // test-012: Property validation
        // test-027: Property validation with AddData
        // test-041: Processing service validates null input
        [Fact]
        public async Task ShouldThrowValidationExceptionsOnModifyOrAddIfIdIsInvalidAndLogItAsync()
        {
            // given
            [Entity] invalid[Entity] = new [Entity]();

            var invalidArgument[Entity]ProcessingException =
                new InvalidArgument[Entity]ProcessingException(
                    message: "Invalid argument(s). Please correct the errors and try again.");

            invalidArgument[Entity]ProcessingException.AddData(
                key: "Id",
                values: "Id is required");

            var expected[Entity]ProcessingValidationException =
                new [Entity]ProcessingValidationException(
                    message: "[Entity] processing validation error occurred, please try again.",
                    innerException: invalidArgument[Entity]ProcessingException);

            // when
            ValueTask<[Entity]> Retrieve[Entity]Task =
                this.[entity]ProcessingService.ModifyOrAdd[Entity]Async(invalid[Entity]);

            [Entity]ProcessingValidationException actual[Entity]ProcessingValidationException =
                await Assert.ThrowsAsync<[Entity]ProcessingValidationException>(Retrieve[Entity]Task.AsTask);

            //then
            actual[Entity]ProcessingValidationException.Should()
                .BeEquivalentTo(expected[Entity]ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expected[Entity]ProcessingValidationException))),
                        Times.Once);

            this.[entity]ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
