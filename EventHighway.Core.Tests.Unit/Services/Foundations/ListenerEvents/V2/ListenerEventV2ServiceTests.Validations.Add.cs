// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 nullListenerEventV2 = null;

            var nullListenerEventV2Exception =
                new NullListenerEventV2Exception(message: "Listener event is null.");

            var expectedListenerEventV2ValidationException =
                new ListenerEventV2ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV2Exception);

            // when
            ValueTask<ListenerEventV2> addListenerEventV2Task =
                this.listenerEventV2Service.AddListenerEventV2Async(
                    nullListenerEventV2,
                    randomCancellationToken);

            ListenerEventV2ValidationException actualListenerEventV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ValidationException>(
                    addListenerEventV2Task.AsTask);

            // then
            actualListenerEventV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfListenerEventV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventStatusV2 invalidListenerEventV2Status =
                GetInvalidEnum<ListenerEventStatusV2>();

            var invalidListenerEventV2 = new ListenerEventV2
            {
                Id = Guid.Empty,
                Response = invalidText,
                Status = invalidListenerEventV2Status,
                EventV2Id = Guid.Empty,
                EventAddressV2Id = Guid.Empty,
                EventListenerV2Id = Guid.Empty
            };

            var invalidListenerEventV2Exception =
                new InvalidListenerEventV2Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.EventV2Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.EventAddressV2Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.EventListenerV2Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.EventParticipantV2Id),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.Status),
                values: "Value is not recognized");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.CreatedDate),
                values: "Required");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.UpdatedDate),
                values: "Required");

            var expectedListenerEventV2ValidationException =
                new ListenerEventV2ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2Exception);

            // when
            ValueTask<ListenerEventV2> addListenerEventV2Task =
                this.listenerEventV2Service.AddListenerEventV2Async(
                    invalidListenerEventV2,
                    randomCancellationToken);

            ListenerEventV2ValidationException actualListenerEventV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ValidationException>(
                    addListenerEventV2Task.AsTask);

            // then
            actualListenerEventV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset anotherRandomDateTimeOffset = GetRandomDateTimeOffset();
            ListenerEventV2 randomListenerEventV2 = CreateRandomListenerEventV2(dates: randomDateTimeOffset);
            ListenerEventV2 invalidListenerEventV2 = randomListenerEventV2;
            invalidListenerEventV2.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidListenerEventV2Exception =
                new InvalidListenerEventV2Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.CreatedDate),
                values: $"Date is not the same as {nameof(ListenerEventV2.UpdatedDate)}");

            var expectedListenerEventV2ValidationException =
                new ListenerEventV2ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventV2> addListenerEventV2Task =
                this.listenerEventV2Service.AddListenerEventV2Async(
                    invalidListenerEventV2,
                    randomCancellationToken);

            ListenerEventV2ValidationException actualListenerEventV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ValidationException>(
                    addListenerEventV2Task.AsTask);

            // then
            actualListenerEventV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfter)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            ListenerEventV2 randomListenerEventV2 =
                CreateRandomListenerEventV2(randomDateTimeOffset
                    .AddSeconds(seconds: minutesBeforeAndAfter));

            ListenerEventV2 invalidListenerEventV2 = randomListenerEventV2;

            var invalidListenerEventV2Exception =
                new InvalidListenerEventV2Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.CreatedDate),
                values: "Date is not recent");

            var expectedListenerEventV2ValidationException =
                new ListenerEventV2ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventV2> addListenerEventV2Task =
                this.listenerEventV2Service.AddListenerEventV2Async(
                    invalidListenerEventV2,
                    randomCancellationToken);

            ListenerEventV2ValidationException actualListenerEventV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ValidationException>(
                    addListenerEventV2Task.AsTask);

            // then
            actualListenerEventV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventParticipantV2IdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            ListenerEventV2 randomListenerEventV2 =
                CreateRandomListenerEventV2(randomDateTimeOffset);

            ListenerEventV2 invalidListenerEventV2 = randomListenerEventV2;
            invalidListenerEventV2.EventParticipantV2Id = Guid.Empty;

            var invalidListenerEventV2Exception =
                new InvalidListenerEventV2Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.EventParticipantV2Id),
                values: "Required");

            var expectedListenerEventV2ValidationException =
                new ListenerEventV2ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventV2> addListenerEventV2Task =
                this.listenerEventV2Service.AddListenerEventV2Async(
                    invalidListenerEventV2,
                    randomCancellationToken);

            ListenerEventV2ValidationException actualListenerEventV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ValidationException>(
                    addListenerEventV2Task.AsTask);

            // then
            actualListenerEventV2ValidationException.Should().BeEquivalentTo(
                expectedListenerEventV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
