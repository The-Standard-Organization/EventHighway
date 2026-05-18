// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventArchiveV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventArchiveV1IsNullAndLogItAsync()
        {
            // given
            EventArchiveV1 nullEventArchiveV1 = null;

            var nullEventArchiveV1OrchestrationException =
                new NullEventArchiveV1OrchestrationException(
                    message: "Event archive is null.");

            var expectedEventArchiveV1OrchestrationValidationException =
                new EventArchiveV1OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV1OrchestrationException);

            // when
            ValueTask addEventArchiveV1Task =
                this.eventArchiveV1OrchestrationService.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                    nullEventArchiveV1);

            EventArchiveV1OrchestrationValidationException
                actualEventArchiveV1OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventArchiveV1OrchestrationValidationException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.eventArchiveV1ServiceMock.Verify(broker =>
                broker.AddEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventArchiveV1sAreNullAndLogItAsync()
        {
            // given
            var invalidEventArchiveV1 = new EventArchiveV1();
            invalidEventArchiveV1.ListenerEventArchiveV1s = null;

            var nullEventArchiveV1OrchestrationException =
                new NullListenerEventArchiveV1sOrchestrationException(
                    message: "Listener event archives are null.");

            var expectedEventArchiveV1OrchestrationValidationException =
                new EventArchiveV1OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV1OrchestrationException);

            // when
            ValueTask addEventArchiveV1Task =
                this.eventArchiveV1OrchestrationService.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                    invalidEventArchiveV1);

            EventArchiveV1OrchestrationValidationException
                actualEventArchiveV1OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventArchiveV1OrchestrationValidationException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.eventArchiveV1ServiceMock.Verify(broker =>
                broker.AddEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
