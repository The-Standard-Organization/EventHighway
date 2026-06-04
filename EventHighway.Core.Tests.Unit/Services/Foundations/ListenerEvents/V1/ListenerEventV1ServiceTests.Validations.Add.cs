// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V1
{
    public partial class ListenerEventV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventV1IsNullAndLogItAsync()
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
            ValueTask<ListenerEventV1> addListenerEventV1Task =
                this.listenerEventV1Service.AddListenerEventV1Async(nullListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    addListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfListenerEventV1IsInvalidAndLogItAsync(
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
                values: "Required");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            // when
            ValueTask<ListenerEventV1> addListenerEventV1Task =
                this.listenerEventV1Service.AddListenerEventV1Async(invalidListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    addListenerEventV1Task.AsTask);

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
                broker.InsertListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset anotherRandomDateTimeOffset = GetRandomDateTimeOffset();
            ListenerEventV1 randomListenerEventV1 = CreateRandomListenerEventV1(dates: randomDateTimeOffset);
            ListenerEventV1 invalidListenerEventV1 = randomListenerEventV1;
            invalidListenerEventV1.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.CreatedDate),
                values: $"Date is not the same as {nameof(ListenerEventV1.UpdatedDate)}");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventV1> addListenerEventV1Task =
                this.listenerEventV1Service.AddListenerEventV1Async(invalidListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    addListenerEventV1Task.AsTask);

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
                broker.InsertListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1(randomDateTimeOffset
                    .AddMinutes(minutes: minutesBeforeAndAfter));

            ListenerEventV1 invalidListenerEventV1 = randomListenerEventV1;

            var invalidListenerEventV1Exception =
                new InvalidListenerEventV1Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV1Exception.AddData(
                key: nameof(ListenerEventV1.CreatedDate),
                values: "Date is not recent");

            var expectedListenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventV1> addListenerEventV1Task =
                this.listenerEventV1Service.AddListenerEventV1Async(invalidListenerEventV1);

            ListenerEventV1ValidationException actualListenerEventV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventV1ValidationException>(
                    addListenerEventV1Task.AsTask);

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
                broker.InsertListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
