// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldModifyListenerEventV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 randomListenerEventV2 =
                CreateRandomListenerEventV2();

            ListenerEventV2 inputListenerEventV2 =
                randomListenerEventV2;

            ListenerEventV2 modifiedListenerEventV2 =
                inputListenerEventV2;

            ListenerEventV2 expectedListenerEventV2 =
                modifiedListenerEventV2.DeepClone();

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    inputListenerEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(modifiedListenerEventV2);

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.eventListenerV2OrchestrationService
                    .ModifyListenerEventV2Async(
                        inputListenerEventV2,
                        randomCancellationToken);

            // then
            actualListenerEventV2.Should().BeEquivalentTo(
                expectedListenerEventV2);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    inputListenerEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventListenerV2ProcessingServiceMock
                .VerifyNoOtherCalls();
                        this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
