// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventListeners.V2
{
    public partial class EventListenerV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveServiceInNewScopePerOperationAsync()
        {
            // given
            EventListenerV2 randomEventListenerV2 = CreateRandomEventListenerV2();
            EventListenerV2 inputEventListenerV2 = randomEventListenerV2;
            int expectedResolutionCount = 2;

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.AddEventListenerV2Async(
                    inputEventListenerV2, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomEventListenerV2);

            // when
            await this.eventListenerV2Client.RegisterEventListenerV2Async(
                inputEventListenerV2);

            await this.eventListenerV2Client.RegisterEventListenerV2Async(
                inputEventListenerV2);

            // then
            this.eventListenerOrchestrationServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.AddEventListenerV2Async(
                    inputEventListenerV2, It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
