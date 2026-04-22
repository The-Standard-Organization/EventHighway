// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Foundation Service Tests — All three axes
// Replace [Entity] / [Entities] / [Namespace] with actual values.
// Demonstrates: Logic, Validations, and Exceptions files.

// ---------------------------------------------------------------
// File: [Entity]ServiceTests.Logic.Add.cs
// test-092: Happy-path tests
// ---------------------------------------------------------------

namespace [Namespace].Tests.Unit.Services.Foundations.[Entities]
{
    public partial class [Entity]ServiceTests
    {
        // test-010: Happy path first
        // test-110: Step 0 — ShouldAdd{Entity}Async
        [Fact]
        public async Task ShouldAdd[Entity]Async()
        {
            // given
            [Entity] random[Entity] = CreateRandom[Entity]();
            [Entity] input[Entity] = random[Entity];
            [Entity] storage[Entity] = input[Entity].DeepClone();
            [Entity] expected[Entity] = storage[Entity].DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.Insert[Entity]Async(input[Entity]))
                    .ReturnsAsync(storage[Entity]);

            // when
            [Entity] actual[Entity] =
                await this.[entity]Service.Add[Entity]Async(input[Entity]);

            // then
            actual[Entity].Should().BeEquivalentTo(expected[Entity]);

            this.storageBrokerMock.Verify(broker =>
                broker.Insert[Entity]Async(input[Entity]),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: [Entity]ServiceTests.Validations.Add.cs
// test-093: Validation failure tests
// ---------------------------------------------------------------

namespace [Namespace].Tests.Unit.Services.Foundations.[Entities]
{
    public partial class [Entity]ServiceTests
    {
        // test-110: Step 1 — ShouldThrowValidationExceptionOnAddIf{Entity}IsNull
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIf[Entity]IsNullAndLogItAsync()
        {
            // given
            [Entity] null[Entity] = null;

            var null[Entity]Exception =
                new Null[Entity]Exception(message: "[Entity] is null.");

            var expected[Entity]ValidationException =
                new [Entity]ValidationException(
                    message: "[Entity] validation error occurred, fix the errors and try again.",
                    innerException: null[Entity]Exception);

            // when
            ValueTask<[Entity]> add[Entity]Task =
                this.[entity]Service.Add[Entity]Async(null[Entity]);

            [Entity]ValidationException actual[Entity]ValidationException =
                await Assert.ThrowsAsync<[Entity]ValidationException>(
                    add[Entity]Task.AsTask);

            // then
            actual[Entity]ValidationException.Should()
                .BeEquivalentTo(expected[Entity]ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expected[Entity]ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.Insert[Entity]Async(It.IsAny<[Entity]>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        // test-110: Step 2 — ShouldThrowValidationExceptionOnAddIf{Entity}IsInvalid
        // test-103: [Theory][InlineData] for parameterized
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIf[Entity]IsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalid[Entity] = new [Entity]
            {
                Name = invalidText
            };

            var invalid[Entity]Exception =
                new Invalid[Entity]Exception(
                    message: "Invalid [entity]. Please correct the errors and try again.");

            invalid[Entity]Exception.AddData(
                key: nameof([Entity].Id),
                values: "Id is required");

            invalid[Entity]Exception.AddData(
                key: nameof([Entity].Name),
                values: "Text is required");

            invalid[Entity]Exception.AddData(
                key: nameof([Entity].CreatedDate),
                values: "Date is required");

            invalid[Entity]Exception.AddData(
                key: nameof([Entity].UpdatedDate),
                values: new[]
                {
                    "Date is required",
                    $"Date is not the same as {nameof([Entity].CreatedDate)}"
                });

            var expected[Entity]ValidationException =
                new [Entity]ValidationException(
                    message: "[Entity] validation error occurred, fix the errors and try again.",
                    innerException: invalid[Entity]Exception);

            // when
            ValueTask<[Entity]> add[Entity]Task =
                this.[entity]Service.Add[Entity]Async(invalid[Entity]);

            [Entity]ValidationException actual[Entity]ValidationException =
                await Assert.ThrowsAsync<[Entity]ValidationException>(
                    add[Entity]Task.AsTask);

            // then
            actual[Entity]ValidationException.Should()
                .BeEquivalentTo(expected[Entity]ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expected[Entity]ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.Insert[Entity]Async(It.IsAny<[Entity]>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: [Entity]ServiceTests.Exceptions.Add.cs
// test-094: Dependency and service exception tests
// ---------------------------------------------------------------

namespace [Namespace].Tests.Unit.Services.Foundations.[Entities]
{
    public partial class [Entity]ServiceTests
    {
        // test-110: Step 3 — DependencyValidationException (BadRequest)
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfBadRequestErrorOccursAndLogItAsync()
        {
            // given
            [Entity] some[Entity] = CreateRandom[Entity]();
            var httpResponseBadRequestException = new HttpResponseBadRequestException();
            
            httpResponseBadRequestException.Data.Add(
                key: nameof([Entity].Id),
                values: "Id is required");

            //**test-111** [ERROR] All external/native exceptions must be localized.
            //**test-112** [ERROR] Localized exceptions must preserve external/native exceptions as the InnerException.
            //**test-113** [ERROR] Localized exceptions must carry the Data collection from the original exception.
            var invalidPost[Entity]Exception =
                new Invalid[Entity]Exception(
                    message: "Invalid [entity] error occurred, fix errors and try again.",
                    innerException: httpResponseBadRequestException,
                    data: httpResponseBadRequestException.Data);

            var expected[Entity]DependencyValidationException =
                new [Entity]DependencyValidationException(
                    message: "[Entity] dependency validation error occurred, fix the errors.",
                    innerException: invalidPost[Entity]Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.Insert[Entity]Async(some[Entity]))
                    .ThrowsAsync(httpResponseBadRequestException);

            // when
            ValueTask<[Entity]> add[Entity]Task =
                this.[entity]Service.Add[Entity]Async(some[Entity]);

            [Entity]DependencyValidationException actual[Entity]DependencyValidationException =
                await Assert.ThrowsAsync<[Entity]DependencyValidationException>(
                    add[Entity]Task.AsTask);

            // then
            actual[Entity]DependencyValidationException.Should()
                .BeEquivalentTo(expected[Entity]DependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.Insert[Entity]Async(some[Entity]),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expected[Entity]DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        // test-110: Step 12 — ServiceException (catch-all)
        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            [Entity] some[Entity] = CreateRandom[Entity]();
            var serviceException = new Exception();

            var failed[Entity]ServiceException =
                new Failed[Entity]ServiceException(
                    message: "Unexpected service error occurred. Contact support.",
                    innerException: serviceException);

            var expected[Entity]ServiceException =
                new [Entity]ServiceException(
                    message: "[Entity] service error occurred, contact support.",
                    innerException: failed[Entity]ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.Insert[Entity]Async(some[Entity]))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<[Entity]> add[Entity]Task =
                this.[entity]Service.Add[Entity]Async(some[Entity]);

            [Entity]ServiceException actual[Entity]ServiceException =
                await Assert.ThrowsAsync<[Entity]ServiceException>(
                    add[Entity]Task.AsTask);

            // then
            actual[Entity]ServiceException.Should()
                .BeEquivalentTo(expected[Entity]ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.Insert[Entity]Async(some[Entity]),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expected[Entity]ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
