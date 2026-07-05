// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldFireScheduledPendingEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> randomEventV2s = CreateRandomEventV2s();
            IQueryable<EventV2> retrievedEventV2s = randomEventV2s;

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventV2s);

            foreach (EventV2 retrievedEventV2 in retrievedEventV2s)
            {
                this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                    service.FireEventV2Async(
                        retrievedEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(retrievedEventV2);
            }

            // when
            await this.eventV2CoordinationService
                .FireScheduledPendingEventV2sAsync(
                    randomCancellationToken);

            // then
            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken),
                        Times.Once);

            foreach (EventV2 retrievedEventV2 in retrievedEventV2s)
            {
                this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                    service.FireEventV2Async(
                        retrievedEventV2,
                        randomCancellationToken),
                            Times.Once);

                this.eventV2OrchestrationServiceMock.Verify(service =>
                    service.MarkEventV2AsImmediateAsync(
                        retrievedEventV2,
                        randomCancellationToken),
                            Times.Once);
            }

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldSkipQuarantinedEventV2WhenFiringScheduledPendingEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 quarantinedEventV2 = CreateRandomEventV2();
            quarantinedEventV2.Status = EventStatusV2.Quarantined;
            IQueryable<EventV2> retrievedEventV2s = new[] { quarantinedEventV2 }.AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventV2s);

            // when
            await this.eventV2CoordinationService
                .FireScheduledPendingEventV2sAsync(
                    randomCancellationToken);

            // then
            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken),
                        Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.MarkEventV2AsImmediateAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldContinueFiringScheduledPendingEventV2sWhenOneItemFailsAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 firstEventV2 = CreateRandomEventV2();
            EventV2 failingEventV2 = CreateRandomEventV2();
            EventV2 lastEventV2 = CreateRandomEventV2();

            IQueryable<EventV2> retrievedEventV2s =
                new[] { firstEventV2, failingEventV2, lastEventV2 }.AsQueryable();

            var itemException = new Exception();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventV2s);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(
                    firstEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(firstEventV2);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(
                    failingEventV2,
                    randomCancellationToken))
                        .ThrowsAsync(itemException);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(
                    lastEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(lastEventV2);

            // when
            await this.eventV2CoordinationService
                .FireScheduledPendingEventV2sAsync(
                    randomCancellationToken);

            // then
            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken),
                        Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    firstEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    failingEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    lastEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.MarkEventV2AsImmediateAsync(
                    firstEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.MarkEventV2AsImmediateAsync(
                    failingEventV2,
                    randomCancellationToken),
                        Times.Never);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.MarkEventV2AsImmediateAsync(
                    lastEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(itemException),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
