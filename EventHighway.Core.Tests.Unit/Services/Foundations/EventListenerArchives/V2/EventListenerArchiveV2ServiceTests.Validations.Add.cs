// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListenerArchives.V2
{
    public partial class EventListenerArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventListenerArchiveV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerArchiveV2 nullEventListenerArchiveV2 = null;

            var nullEventListenerArchiveV2Exception =
                new NullEventListenerArchiveV2Exception(
                    message: "Event listener archive is null.");

            var expectedEventListenerArchiveV2ValidationException =
                new EventListenerArchiveV2ValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventListenerArchiveV2Exception);

            // when
            ValueTask<EventListenerArchiveV2> addEventListenerArchiveV2Task =
                this.eventListenerArchiveV2Service.AddEventListenerArchiveV2Async(
                    nullEventListenerArchiveV2,
                    cancellationToken);

            EventListenerArchiveV2ValidationException actualEventListenerArchiveV2ValidationException =
                await Assert.ThrowsAsync<EventListenerArchiveV2ValidationException>(
                    addEventListenerArchiveV2Task.AsTask);

            // then
            actualEventListenerArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerArchiveV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    It.IsAny<EventListenerArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventListenerArchiveV2IsInvalidAndLogItAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            var invalidEventListenerArchiveV2 = new EventListenerArchiveV2
            {
                Id = Guid.Empty,
                HandlerId = Guid.Empty,
                EventListenerId = Guid.Empty,
                EventAddressId = Guid.Empty,
                EventArchiveV2Id = Guid.Empty
            };

            var invalidEventListenerArchiveV2Exception =
                new InvalidEventListenerArchiveV2Exception(
                    message: "Event listener archive is invalid, fix the errors and try again.");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.Id),
                values: "Required");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.Name),
                values: "Required");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.Description),
                values: "Required");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.HandlerId),
                values: "Required");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.EventListenerId),
                values: "Required");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.EventAddressId),
                values: "Required");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.EventArchiveV2Id),
                values: "Required");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.CreatedDate),
                values: "Required");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.UpdatedDate),
                values: "Required");

            invalidEventListenerArchiveV2Exception.AddData(
                key: nameof(EventListenerArchiveV2.ArchivedDate),
                values: "Required");

            var expectedEventListenerArchiveV2ValidationException =
                new EventListenerArchiveV2ValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerArchiveV2Exception);

            // when
            ValueTask<EventListenerArchiveV2> addEventListenerArchiveV2Task =
                this.eventListenerArchiveV2Service.AddEventListenerArchiveV2Async(
                    invalidEventListenerArchiveV2,
                    cancellationToken);

            EventListenerArchiveV2ValidationException actualEventListenerArchiveV2ValidationException =
                await Assert.ThrowsAsync<EventListenerArchiveV2ValidationException>(
                    addEventListenerArchiveV2Task.AsTask);

            // then
            actualEventListenerArchiveV2ValidationException.Should().BeEquivalentTo(
                expectedEventListenerArchiveV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    It.IsAny<EventListenerArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
