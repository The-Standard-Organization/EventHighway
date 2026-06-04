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
    public partial class EventArchiveV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldAddEventArchiveV1WithListenerEventArchiveV1sAsync()
        {
            // given
            EventArchiveV1 randomEventArchiveV1 = CreateRandomEventArchiveV1();
            EventArchiveV1 inputEventArchiveV1 = randomEventArchiveV1;

            List<ListenerEventArchiveV1> randomListenerEventArchiveV1s =
                randomEventArchiveV1.ListenerEventArchiveV1s.ToList();

            List<ListenerEventArchiveV1> inputListenerEventArchiveV1s =
                randomListenerEventArchiveV1s;

            // when
            await this.eventArchiveV1OrchestrationService
                .AddEventArchiveV1WithListenerEventArchiveV1sAsync(inputEventArchiveV1);

            // then
            foreach (ListenerEventArchiveV1 listenerEventArchiveV1 in inputListenerEventArchiveV1s)
            {
                this.listenerEventArchiveV1ServiceMock.Verify(service =>
                    service.AddListenerEventArchiveV1Async(listenerEventArchiveV1),
                        Times.Once);
            }

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(inputEventArchiveV1),
                    Times.Once);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
