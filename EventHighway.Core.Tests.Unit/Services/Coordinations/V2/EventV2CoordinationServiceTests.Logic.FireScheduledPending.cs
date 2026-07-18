// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldClaimEventV2BeforeFiringAndSkipWhenAlreadyClaimedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 firstEventV2 = CreateRandomEventV2();
            EventV2 claimedEventV2 = CreateRandomEventV2();
            EventV2 lastEventV2 = CreateRandomEventV2();

            IQueryable<EventV2> retrievedEventV2s =
                new[] { firstEventV2, claimedEventV2, lastEventV2 }.AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.TryClaimScheduledEventV2Async(
                    claimedEventV2.Id, randomCancellationToken))
                        .ReturnsAsync(0);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.TryClaimScheduledEventV2Async(
                    firstEventV2.Id, randomCancellationToken))
                        .ReturnsAsync(1);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.TryClaimScheduledEventV2Async(
                    lastEventV2.Id, randomCancellationToken))
                        .ReturnsAsync(1);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(), randomCancellationToken))
                        .ReturnsAsync((EventV2 eventV2, CancellationToken _) => eventV2);

            // when
            await this.eventV2CoordinationService
                .FireScheduledPendingEventV2sAsync(randomCancellationToken);

            // then
            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.TryClaimScheduledEventV2Async(firstEventV2.Id, randomCancellationToken),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.TryClaimScheduledEventV2Async(claimedEventV2.Id, randomCancellationToken),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.TryClaimScheduledEventV2Async(lastEventV2.Id, randomCancellationToken),
                    Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(firstEventV2, randomCancellationToken),
                    Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(claimedEventV2, randomCancellationToken),
                    Times.Never);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(lastEventV2, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldFireScheduledPendingEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventV2> retrievedEventV2s = CreateRandomEventV2s().ToList();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventV2s.AsQueryable());

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.TryClaimScheduledEventV2Async(
                    It.IsAny<Guid>(), randomCancellationToken))
                        .ReturnsAsync(1);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(), randomCancellationToken))
                        .ReturnsAsync((EventV2 eventV2, CancellationToken _) => eventV2);

            // when
            await this.eventV2CoordinationService
                .FireScheduledPendingEventV2sAsync(randomCancellationToken);

            // then
            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV2sAsync(randomCancellationToken),
                    Times.Exactly(2));

            foreach (EventV2 retrievedEventV2 in retrievedEventV2s)
            {
                this.eventV2OrchestrationServiceMock.Verify(service =>
                    service.TryClaimScheduledEventV2Async(
                        retrievedEventV2.Id, randomCancellationToken),
                            Times.Once);

                this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                    service.FireEventV2Async(retrievedEventV2, randomCancellationToken),
                        Times.Once);
            }

            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPageScheduledPendingEventV2sByBatchSizeAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventV2 firstEventV2 = CreateRandomEventV2();
            firstEventV2.ScheduledDate = baseDate;

            EventV2 secondEventV2 = CreateRandomEventV2();
            secondEventV2.ScheduledDate = baseDate.AddMinutes(1);

            EventV2 thirdEventV2 = CreateRandomEventV2();
            thirdEventV2.ScheduledDate = baseDate.AddMinutes(2);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(new BatchConfiguration { BatchSizeForBulkProcessing = 2 });

            // Each retrieval reflects the atomic claim removing already-claimed events.
            this.eventV2OrchestrationServiceMock
                .SetupSequence(service =>
                    service.RetrieveScheduledPendingEventV2sAsync(randomCancellationToken))
                .ReturnsAsync(new[] { firstEventV2, secondEventV2, thirdEventV2 }.AsQueryable())
                .ReturnsAsync(new[] { thirdEventV2 }.AsQueryable())
                .ReturnsAsync(Array.Empty<EventV2>().AsQueryable());

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.TryClaimScheduledEventV2Async(
                    It.IsAny<Guid>(), randomCancellationToken))
                        .ReturnsAsync(1);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(), randomCancellationToken))
                        .ReturnsAsync((EventV2 eventV2, CancellationToken _) => eventV2);

            // when
            await this.eventV2CoordinationService
                .FireScheduledPendingEventV2sAsync(randomCancellationToken);

            // then
            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV2sAsync(randomCancellationToken),
                    Times.Exactly(3));

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(firstEventV2, randomCancellationToken),
                    Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(secondEventV2, randomCancellationToken),
                    Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(thirdEventV2, randomCancellationToken),
                    Times.Once);

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
                .FireScheduledPendingEventV2sAsync(randomCancellationToken);

            // then
            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV2sAsync(randomCancellationToken),
                    Times.Exactly(2));

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.TryClaimScheduledEventV2Async(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

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

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.TryClaimScheduledEventV2Async(
                    It.IsAny<Guid>(), randomCancellationToken))
                        .ReturnsAsync(1);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(firstEventV2, randomCancellationToken))
                    .ReturnsAsync(firstEventV2);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(failingEventV2, randomCancellationToken))
                    .ThrowsAsync(itemException);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(lastEventV2, randomCancellationToken))
                    .ReturnsAsync(lastEventV2);

            // when
            await this.eventV2CoordinationService
                .FireScheduledPendingEventV2sAsync(randomCancellationToken);

            // then
            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(firstEventV2, randomCancellationToken),
                    Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(failingEventV2, randomCancellationToken),
                    Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(lastEventV2, randomCancellationToken),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.TryClaimScheduledEventV2Async(failingEventV2.Id, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(itemException),
                    Times.Once);

            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
