// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventV1ArchiveOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldAddEventV1ArchiveWithListenerEventV1ArchivesAsync()
        {
            // given
            EventV1Archive randomEventV1Archive = CreateRandomEventV1Archive();
            EventV1Archive inputEventV1Archive = randomEventV1Archive;

            List<ListenerEventV1Archive> randomListenerEventV1Archives =
                randomEventV1Archive.ListenerEventV1Archives.ToList();

            List<ListenerEventV1Archive> inputListenerEventV1Archives =
                randomListenerEventV1Archives;

            // when
            await this.eventV1ArchiveOrchestrationService
                .AddEventV1ArchiveWithListenerEventV1ArchivesAsync(inputEventV1Archive);

            // then
            foreach (ListenerEventV1Archive listenerEventV1Archive in inputListenerEventV1Archives)
            {
                this.listenerEventV1ArchiveServiceMock.Verify(service => 
                    service.AddListenerEventV1ArchiveAsync(listenerEventV1Archive),
                        Times.Once);
            }

            this.eventV1ArchiveServiceMock.Verify(service => 
                service.AddEventV1ArchiveAsync(inputEventV1Archive),
                    Times.Once);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
