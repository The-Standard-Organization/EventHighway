// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RetryingListenerEvents.V2
{
    public partial class RetryingListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetryFailedListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(randomTake);

            List<ListenerEventV2> randomListenerEventV2Batch =
                CreateRandomListenerEventV2sWithNavProps(randomTake);

            foreach (ListenerEventV2 listenerEventV2 in randomListenerEventV2Batch)
            {
                listenerEventV2.EventListenerV2.PromotedProperties = null;
            }

            DateTimeOffset randomNow = GetRandomDateTimeOffset();

            var ranEventCallV2 = new EventCallV2
            {
                IsSuccess = true,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString()
            };

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.listenerEventV2ProcessingServiceMock
                .SetupSequence(service =>
                    service.RetrieveBatchOfRetryListenerEventV2sAsync(
                        randomTake, randomCancellationToken))
                .ReturnsAsync(randomListenerEventV2Batch)
                .ReturnsAsync(new List<ListenerEventV2>());

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(), randomCancellationToken))
                .ReturnsAsync(ranEventCallV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(randomNow);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(), randomCancellationToken))
                .ReturnsAsync((ListenerEventV2 listenerEventV2, CancellationToken _) =>
                    listenerEventV2);

            // when
            await this.retryingListenerEventV2OrchestrationService
                .RetryFailedListenerEventV2sAsync(randomCancellationToken);

            // then
            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfRetryListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Exactly(2));

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(), randomCancellationToken),
                Times.Exactly(randomTake));

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                Times.Exactly(randomTake));

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(), randomCancellationToken),
                Times.Exactly(randomTake));

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldContinueRetryingRemainingListenerEventV2sWhenCoreFailsAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(randomTake);

            List<ListenerEventV2> randomListenerEventV2Batch =
                CreateRandomListenerEventV2sWithNavProps(randomTake);

            foreach (ListenerEventV2 listenerEventV2 in randomListenerEventV2Batch)
            {
                listenerEventV2.EventListenerV2.PromotedProperties = null;
            }

            ListenerEventV2 failingListenerEventV2 = randomListenerEventV2Batch[0];

            DateTimeOffset randomNow = GetRandomDateTimeOffset();

            var ranEventCallV2 = new EventCallV2
            {
                IsSuccess = true,
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString()
            };

            var serviceException = new Exception();

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.listenerEventV2ProcessingServiceMock
                .SetupSequence(service =>
                    service.RetrieveBatchOfRetryListenerEventV2sAsync(
                        randomTake, randomCancellationToken))
                .ReturnsAsync(randomListenerEventV2Batch)
                .ReturnsAsync(new List<ListenerEventV2>());

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(), randomCancellationToken))
                .ReturnsAsync(ranEventCallV2);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(randomNow);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(lev => lev.Id == failingListenerEventV2.Id),
                    randomCancellationToken))
                .ThrowsAsync(serviceException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.Is<ListenerEventV2>(lev => lev.Id != failingListenerEventV2.Id),
                    randomCancellationToken))
                .ReturnsAsync((ListenerEventV2 listenerEventV2, CancellationToken _) =>
                    listenerEventV2);

            // when
            await this.retryingListenerEventV2OrchestrationService
                .RetryFailedListenerEventV2sAsync(randomCancellationToken);

            // then
            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(), randomCancellationToken),
                Times.Exactly(randomTake));

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(), randomCancellationToken),
                Times.Exactly(randomTake));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(serviceException),
                Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfRetryListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Exactly(2));

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
