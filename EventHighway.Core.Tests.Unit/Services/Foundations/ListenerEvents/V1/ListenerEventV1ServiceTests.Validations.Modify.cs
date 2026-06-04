// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V1
{
    public partial class ListenerEventV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfListenerEventV1IsNullAndLogItAsync()
        {
            // given
            ListenerEventV1 nullListenerEventV1 = null;

            var nullListenerEventV1Exception =
                new NullListenerEventV1Exception(message: "Listener event is null.");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV1Exception);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(nullListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnModifyIfListenerEventV1IsInvalidAndLogItAsync(
            string invalidText)
        {
            ListenerEventStatusV1 invalidListenerEventV1Status =
                GetInvalidEnum<ListenerEventStatusV1>();

            var invalidListenerEventV1 = new ListenerEventV1
            {
                Id = Guid.Empty,
                Response = invalidText,
                Status = invalidListenerEventV1Status,
                EventId = Guid.Empty,
                EventAddressId = Guid.Empty,
                EventListenerId = Guid.Empty
            };

            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.Id),
                values: "Required");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.Response),
                values: "Required");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.EventId),
                values: "Required");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.EventAddressId),
                values: "Required");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.EventListenerId),
                values: "Required");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.Status),
                values: "Value is not recognized");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.CreatedDate),
                values: "Required");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.UpdatedDate),

                values:
                    [
                        "Required",
                        $"Date is the same as {nameof(ListenerEventV1.CreatedDate)}"
                    ]);

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(invalidListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            ListenerEventV1 randomListenerEventV1 = CreateRandomListenerEventV1(dates: randomDateTimeOffset);
            ListenerEventV1 invalidListenerEventV1 = randomListenerEventV1;

            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.UpdatedDate),
                values: $"Date is the same as {nameof(ListenerEventV1.CreatedDate)}");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(invalidListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1(dates: randomDateTimeOffset);

            ListenerEventV1 invalidListenerEventV1 = randomListenerEventV1;

            invalidListenerEventV1.UpdatedDate =
                invalidListenerEventV1.UpdatedDate.AddMinutes(minutesBeforeOrAfter);

            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.UpdatedDate),
                values: "Date is not recent");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(invalidListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfListenerEventV1DoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            ListenerEventV1 randomListenerEventV1 = CreateRandomListenerEventV1(dates: randomDateTimeOffset);
            ListenerEventV1 nonExistListenerEventV1 = randomListenerEventV1;
            Guid nonExistListenerEventV1Id = nonExistListenerEventV1.Id;
            nonExistListenerEventV1.CreatedDate = randomDateTimeOffset.AddDays(randomDaysAgo);
            ListenerEventV1 nullListenerEventV1 = null;

            var notFoundListenerEventV1Exception =
                new NotFoundListenerEventV1Exception(
                    message: $"Could not find listener event with id: {nonExistListenerEventV1Id}.");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: notFoundListenerEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV1ByIdAsync(nonExistListenerEventV1Id))
                    .ReturnsAsync(nullListenerEventV1);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(nonExistListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(nonExistListenerEventV1Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomDaysAgo = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            ListenerEventV1 randomListenerEventV1 = CreateRandomListenerEventV1(dates: randomDateTimeOffset);
            ListenerEventV1 invalidListenerEventV1 = randomListenerEventV1;
            Guid invalidListenerEventV1Id = invalidListenerEventV1.Id;
            invalidListenerEventV1.CreatedDate = randomDateTimeOffset.AddDays(randomDaysAgo);
            DateTimeOffset randomOtherDateTime = GetRandomDateTimeOffset();
            ListenerEventV1 storageListenerEventV1 = invalidListenerEventV1.DeepClone();
            storageListenerEventV1.CreatedDate = randomOtherDateTime;

            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.CreatedDate),
                values: $"Date is not the same as storage");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV1ByIdAsync(invalidListenerEventV1Id))
                    .ReturnsAsync(storageListenerEventV1);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(invalidListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(invalidListenerEventV1Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsEarlierThanStorageAndLogItAsync()
        {
            // given
            int randomTimeAgo = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset earlierDateTime = randomDateTimeOffset.AddDays(days: randomTimeAgo);
            ListenerEventV1 randomListenerEventV1 = CreateRandomListenerEventV1(dates: randomDateTimeOffset);
            ListenerEventV1 invalidListenerEventV1 = randomListenerEventV1;
            Guid invalidListenerEventV1Id = invalidListenerEventV1.Id;
            invalidListenerEventV1.CreatedDate = earlierDateTime;
            ListenerEventV1 storageListenerEventV1 = invalidListenerEventV1.DeepClone();
            DateTimeOffset earlierSeconds = randomDateTimeOffset.AddSeconds(seconds: randomTimeAgo);
            invalidListenerEventV1.UpdatedDate = earlierSeconds;

            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.UpdatedDate),
                values: $"Date is earlier than storage");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV1ByIdAsync(invalidListenerEventV1Id))
                    .ReturnsAsync(storageListenerEventV1);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(invalidListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(invalidListenerEventV1Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
