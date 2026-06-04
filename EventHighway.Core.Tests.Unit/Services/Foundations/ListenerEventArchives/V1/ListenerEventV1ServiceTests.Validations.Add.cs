// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V1
{
    public partial class ListenerEventArchiveV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventArchiveV1IsNullAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 nullListenerEventArchiveV1 = null;

            var nullListenerEventArchiveV1Exception =
                new NullListenerEventArchiveV1Exception(message: "Listener event archive is null.");

            var expectedListenerEventArchiveV1ValidationException =
                new ListenerEventArchiveV1ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventArchiveV1Exception);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1Service.AddListenerEventArchiveV1Async(nullListenerEventArchiveV1);

            ListenerEventArchiveV1ValidationException actualListenerEventArchiveV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1ValidationException>(
                    addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1ValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfListenerEventArchiveV1IsInvalidAndLogItAsync(
            string invalidText)
        {
            ListenerEventArchiveStatusV1 invalidListenerEventArchiveV1Status =
                GetInvalidEnum<ListenerEventArchiveStatusV1>();

            var invalidListenerEventArchiveV1 = new ListenerEventArchiveV1
            {
                Id = Guid.Empty,
                Response = invalidText,
                ResponseReasonPhrase = invalidText,
                Status = invalidListenerEventArchiveV1Status,
                EventId = Guid.Empty,
                EventAddressId = Guid.Empty,
                EventListenerId = Guid.Empty
            };

            var invalidListenerEventArchiveV1Exception =
                new InvalidListenerEventArchiveV1Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV1Exception.AddData(
                key: nameof(ListenerEventArchiveV1.Id),
                values: "Required");

            invalidListenerEventArchiveV1Exception.AddData(
                key: nameof(ListenerEventArchiveV1.EventId),
                values: "Required");

            invalidListenerEventArchiveV1Exception.AddData(
                key: nameof(ListenerEventArchiveV1.EventAddressId),
                values: "Required");

            invalidListenerEventArchiveV1Exception.AddData(
                key: nameof(ListenerEventArchiveV1.EventListenerId),
                values: "Required");

            invalidListenerEventArchiveV1Exception.AddData(
                key: nameof(ListenerEventArchiveV1.Status),
                values: "Value is not recognized");

            invalidListenerEventArchiveV1Exception.AddData(
                key: nameof(ListenerEventArchiveV1.CreatedDate),
                values: "Required");

            invalidListenerEventArchiveV1Exception.AddData(
                key: nameof(ListenerEventArchiveV1.UpdatedDate),
                values: "Required");

            invalidListenerEventArchiveV1Exception.AddData(
                key: nameof(ListenerEventArchiveV1.ArchivedDate),
                values: "Required");

            var expectedListenerEventArchiveV1ValidationException =
                new ListenerEventArchiveV1ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV1Exception);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1Service.AddListenerEventArchiveV1Async(invalidListenerEventArchiveV1);

            ListenerEventArchiveV1ValidationException actualListenerEventArchiveV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1ValidationException>(
                    addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1ValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnAddIfArchivedDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            ListenerEventArchiveV1 randomListenerEventArchiveV1 =
                CreateRandomListenerEventArchiveV1(randomDateTimeOffset
                    .AddMinutes(minutes: minutesBeforeAndAfter));

            ListenerEventArchiveV1 invalidListenerEventArchiveV1 = randomListenerEventArchiveV1;

            var invalidListenerEventArchiveV1Exception =
                new InvalidListenerEventArchiveV1Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV1Exception.AddData(
                key: nameof(ListenerEventArchiveV1.ArchivedDate),
                values: "Date is not recent");

            var expectedListenerEventArchiveV1ValidationException =
                new ListenerEventArchiveV1ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1Service.AddListenerEventArchiveV1Async(invalidListenerEventArchiveV1);

            ListenerEventArchiveV1ValidationException actualListenerEventArchiveV1ValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1ValidationException>(
                    addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1ValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV1ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
