// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfDeadEventV2sAsync()
        {
            // given
            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            BatchConfiguration retrievedBatchConfiguration = randomBatchConfiguration;
            int inputTake = retrievedBatchConfiguration.BatchSizeForBulkProcessing;

            IEnumerable<EventV2> randomEventV2s = CreateRandomEventV2s().ToList();
            IEnumerable<EventV2> retrievedEventV2s = randomEventV2s;
            IEnumerable<EventV2> expectedEventV2s = retrievedEventV2s.DeepClone();

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(retrievedBatchConfiguration);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(inputTake))
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfDeadEventV2sAsync();

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(inputTake),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
