// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldAddListenerEventV1Async()
        {
            // given
            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1();

            ListenerEventV1 inputListenerEventV1 =
                randomListenerEventV1;

            ListenerEventV1 addedListenerEventV1 =
                inputListenerEventV1;

            ListenerEventV1 expectedListenerEventV1 =
                addedListenerEventV1.DeepClone();

            this.listenerEventV1ProcessingServiceMock.Setup(service =>
                service.AddListenerEventV1Async(
                    inputListenerEventV1))
                        .ReturnsAsync(addedListenerEventV1);

            // when
            ListenerEventV1 actualListenerEventV1 =
                await this.eventListenerV1OrchestrationService
                    .AddListenerEventAsync(
                        inputListenerEventV1);

            // then
            actualListenerEventV1.Should().BeEquivalentTo(
                expectedListenerEventV1);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.AddListenerEventV1Async(
                    inputListenerEventV1),
                        Times.Once);

            this.listenerEventV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventListenerV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
