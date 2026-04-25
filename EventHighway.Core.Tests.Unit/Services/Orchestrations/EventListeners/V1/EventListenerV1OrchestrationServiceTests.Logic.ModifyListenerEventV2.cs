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
        public async Task ShouldModifyListenerEventV1Async()
        {
            // given
            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1();

            ListenerEventV1 inputListenerEventV1 =
                randomListenerEventV1;

            ListenerEventV1 modifiedListenerEventV1 =
                inputListenerEventV1;

            ListenerEventV1 expectedListenerEventV1 =
                modifiedListenerEventV1.DeepClone();

            this.listenerEventV1ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventAsync(
                    inputListenerEventV1))
                        .ReturnsAsync(modifiedListenerEventV1);

            // when
            ListenerEventV1 actualListenerEventV1 =
                await this.eventListenerV1OrchestrationService
                    .ModifyListenerEventAsync(
                        inputListenerEventV1);

            // then
            actualListenerEventV1.Should().BeEquivalentTo(
                expectedListenerEventV1);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventAsync(
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
