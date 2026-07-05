// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfDeadEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            BatchConfiguration retrievedBatchConfiguration = randomBatchConfiguration;
            int inputTake = retrievedBatchConfiguration.BatchSizeForBulkProcessing;

            List<EventV2> randomEventV2s = CreateRandomEventV2List();
            IQueryable<EventV2> retrievedEventV2s = randomEventV2s.AsQueryable();
            IEnumerable<EventV2> expectedEventV2s = retrievedEventV2s.Take(inputTake);

            var mockSequence = new MockSequence();

            this.configurationBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetBatchConfiguration())
                    .Returns(retrievedBatchConfiguration);

            this.eventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RetrieveAllDeadEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveAllDeadEventV2sWhenBatchSizeIsZeroAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var batchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = 0
            };

            List<EventV2> randomEventV2s = CreateRandomEventV2List();
            IQueryable<EventV2> retrievedEventV2s = randomEventV2s.AsQueryable();
            IEnumerable<EventV2> expectedEventV2s = retrievedEventV2s.AsEnumerable();

            var mockSequence = new MockSequence();

            this.configurationBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RetrieveAllDeadEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
