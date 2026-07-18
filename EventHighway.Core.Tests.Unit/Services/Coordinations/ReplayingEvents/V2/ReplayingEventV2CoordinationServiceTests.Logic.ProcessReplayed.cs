// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldProcessReplayedListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(randomTake);

            List<ListenerEventV2> randomListenerEventV2Batch =
                CreateRandomListenerEventV2s(randomTake);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.replayingListenerEventV2OrchestrationServiceMock
                .SetupSequence(service =>
                    service.RetrieveBatchOfReplayListenerEventV2sAsync(
                        randomTake, randomCancellationToken))
                .ReturnsAsync(randomListenerEventV2Batch)
                .ReturnsAsync(new List<ListenerEventV2>());

            foreach (ListenerEventV2 listenerEventV2 in randomListenerEventV2Batch)
            {
                this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                    service.ProcessReplayListenerEventV2Async(
                        listenerEventV2, randomCancellationToken))
                    .ReturnsAsync(listenerEventV2);
            }

            // when
            await this.replayingEventV2CoordinationService
                .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            // then
            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Exactly(2));

            foreach (ListenerEventV2 listenerEventV2 in randomListenerEventV2Batch)
            {
                this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                    service.ProcessReplayListenerEventV2Async(
                        listenerEventV2, randomCancellationToken),
                    Times.Once);
            }

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.replayingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldContinueProcessingReplayedListenerEventV2sWhenOneItemFailsAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(randomTake);

            List<ListenerEventV2> randomListenerEventV2Batch =
                CreateRandomListenerEventV2s(count: 3);

            ListenerEventV2 firstListenerEventV2 = randomListenerEventV2Batch[0];
            ListenerEventV2 failingListenerEventV2 = randomListenerEventV2Batch[1];
            ListenerEventV2 lastListenerEventV2 = randomListenerEventV2Batch[2];

            var itemException = new Exception();

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.replayingListenerEventV2OrchestrationServiceMock
                .SetupSequence(service =>
                    service.RetrieveBatchOfReplayListenerEventV2sAsync(
                        randomTake, randomCancellationToken))
                .ReturnsAsync(randomListenerEventV2Batch)
                .ReturnsAsync(new List<ListenerEventV2>());

            this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ProcessReplayListenerEventV2Async(
                    firstListenerEventV2, randomCancellationToken))
                .ReturnsAsync(firstListenerEventV2);

            this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ProcessReplayListenerEventV2Async(
                    failingListenerEventV2, randomCancellationToken))
                .ThrowsAsync(itemException);

            this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ProcessReplayListenerEventV2Async(
                    lastListenerEventV2, randomCancellationToken))
                .ReturnsAsync(lastListenerEventV2);

            // when
            await this.replayingEventV2CoordinationService
                .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            // then
            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Exactly(2));

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ProcessReplayListenerEventV2Async(
                    firstListenerEventV2, randomCancellationToken),
                Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ProcessReplayListenerEventV2Async(
                    failingListenerEventV2, randomCancellationToken),
                Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ProcessReplayListenerEventV2Async(
                    lastListenerEventV2, randomCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(itemException),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.replayingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotReprocessAlreadyAttemptedReplayedListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(randomTake);

            List<ListenerEventV2> persistentListenerEventV2Batch =
                CreateRandomListenerEventV2s(randomTake);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.replayingListenerEventV2OrchestrationServiceMock
                .SetupSequence(service =>
                    service.RetrieveBatchOfReplayListenerEventV2sAsync(
                        randomTake, randomCancellationToken))
                .ReturnsAsync(persistentListenerEventV2Batch)
                .ReturnsAsync(persistentListenerEventV2Batch)
                .ReturnsAsync(new List<ListenerEventV2>());

            foreach (ListenerEventV2 listenerEventV2 in persistentListenerEventV2Batch)
            {
                this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                    service.ProcessReplayListenerEventV2Async(
                        listenerEventV2, randomCancellationToken))
                    .ReturnsAsync(listenerEventV2);
            }

            // when
            await this.replayingEventV2CoordinationService
                .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            // then
            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Exactly(2));

            foreach (ListenerEventV2 listenerEventV2 in persistentListenerEventV2Batch)
            {
                this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                    service.ProcessReplayListenerEventV2Async(
                        listenerEventV2, randomCancellationToken),
                    Times.Once);
            }

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.replayingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
