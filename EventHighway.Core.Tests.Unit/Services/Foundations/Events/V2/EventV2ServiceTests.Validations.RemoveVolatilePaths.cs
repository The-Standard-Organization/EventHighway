// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveVolatilePathsIfEventAddressIdAndVolatilePathsAreInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2(dates: GetRandomDateTimeOffset());

            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventAddressId = System.Guid.Empty;

            var loopDetectionConfiguration = new LoopDetection
            {
                VolatilePaths = new List<VolatilePaths>()
            };

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.UpsertDataList(
                key: nameof(EventV2.EventAddressId),
                value: "Required");

            invalidEventV2Exception.UpsertDataList(
                key: nameof(VolatilePaths.VolatileContentPaths),
                value: "Required");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                .Returns(loopDetectionConfiguration);

            // when
            ValueTask<string> removeVolatilePathsTask =
                this.eventV2Service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    removeVolatilePathsTask.AsTask);

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
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveVolatilePathsIfEventV2IsNullAndLogItAsync()
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
            ValueTask<string> removeVolatilePathsTask =
                this.eventV2Service.RemoveVolatilePathsAsync(
                    nullEventV2,
                    randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    removeVolatilePathsTask.AsTask);

            // then
            actualEventV2ValidationException.Should().BeEquivalentTo(
                expectedEventV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
