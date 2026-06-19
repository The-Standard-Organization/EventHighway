// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
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
        public async Task ShouldRetrieveAllDeadEventV2sWithListenersAsync()
        {
            // given
            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            BatchConfiguration retrievedBatchConfiguration = randomBatchConfiguration;
            int inputTake = retrievedBatchConfiguration.BatchSizeForBulkProcessing;

            List<EventV2> randomEventV2s = CreateRandomEventV2List();
            IQueryable<EventV2> retrievedEventV2s = randomEventV2s.AsQueryable();
            IEnumerable<EventV2> expectedEventV2s = randomEventV2s.Take(inputTake);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(retrievedBatchConfiguration);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync())
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.archivingEventV2OrchestrationService
                    .RetrieveAllDeadEventV2sWithListenersAsync();

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
