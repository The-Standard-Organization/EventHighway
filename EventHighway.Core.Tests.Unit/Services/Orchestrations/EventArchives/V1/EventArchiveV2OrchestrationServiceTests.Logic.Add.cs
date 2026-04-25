// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventArchiveV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldAddEventV1ArchiveWithListenerEventV1ArchivesAsync()
        {
            // given
            EventArchiveV1 randomEventV1Archive = CreateRandomEventArchiveV1();
            EventArchiveV1 inputEventV1Archive = randomEventV1Archive;

            List<ListenerEventArchiveV1> randomListenerEventV1Archives =
                randomEventV1Archive.ListenerEventArchives.ToList();

            List<ListenerEventArchiveV1> inputListenerEventV1Archives =
                randomListenerEventV1Archives;

            // when
            await this.eventArchiveV1OrchestrationService
                .AddEventArchiveWithListenerEventArchivesAsync(inputEventV1Archive);

            // then
            foreach (ListenerEventArchiveV1 listenerEventV1Archive in inputListenerEventV1Archives)
            {
                this.listenerEventArchiveV1ServiceMock.Verify(service =>
                    service.AddListenerEventArchiveAsync(listenerEventV1Archive),
                        Times.Once);
            }

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveAsync(inputEventV1Archive),
                    Times.Once);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
