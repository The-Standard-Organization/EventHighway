// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveCountBySignatureIfEventV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 nullEventV2 = null;

            var nullEventV2Exception =
                new NullEventV2Exception(message: "Event is null.");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV2Exception);

            // when
            ValueTask<int> retrieveCountTask =
                this.eventV2Service.RetrieveEventV2CountBySignatureAsync(
                    nullEventV2,
                    randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    retrieveCountTask.AsTask);

            // then
            actualEventV2ValidationException.Should().BeEquivalentTo(
                expectedEventV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveCountBySignatureIfFieldsAreInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 invalidEventV2 = new EventV2
            {
                EventAddressId = Guid.Empty,
                EventName = string.Empty,
                ContentHash = string.Empty
            };

            LoopDetection nullLoopDetectionConfig = null;

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.UpsertDataList(
                key: nameof(EventV2.EventAddressId),
                value: "Required");

            invalidEventV2Exception.UpsertDataList(
                key: nameof(EventV2.EventName),
                value: "Required");

            invalidEventV2Exception.UpsertDataList(
                key: nameof(EventV2.ContentHash),
                value: "Required");

            invalidEventV2Exception.UpsertDataList(
                key: nameof(LoopDetection.Window),
                value: "Required");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                    .Returns(nullLoopDetectionConfig);

            // when
            ValueTask<int> retrieveCountTask =
                this.eventV2Service.RetrieveEventV2CountBySignatureAsync(
                    invalidEventV2,
                    randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    retrieveCountTask.AsTask);

            // then
            actualEventV2ValidationException.Should().BeEquivalentTo(
                expectedEventV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
