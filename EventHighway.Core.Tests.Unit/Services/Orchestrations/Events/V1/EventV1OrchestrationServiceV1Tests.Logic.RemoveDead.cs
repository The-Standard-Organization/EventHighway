// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceV1Tests
    {
        [Fact]
        public async Task ShouldRemoveEventV1AndListenerEventV1sAsync()
        {
            // given
            var mockSequence = new MockSequence();
            EventV1 randomEventV1 = CreateRandomEventV1();

            var randomListenerEventV1s =
                new List<ListenerEventV1>(randomEventV1.ListenerEvents);

            EventV1 inputEventV1 = randomEventV1;

            // when
            await this.eventV1OrchestrationServiceV1
                .RemoveEventV1AndListenerEventV1sAsync(inputEventV1);

            // then
            foreach (ListenerEventV1 listenerEventV1 in randomListenerEventV1s)
            {
                this.listenerEventV1ProcessingServiceMock.Verify(service =>
                    service.RemoveListenerEventV1ByIdAsync(listenerEventV1.Id),
                        Times.Once);
            }

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(inputEventV1.Id),
                    Times.Once);

            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
