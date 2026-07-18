// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ReplayingEvents.V2
{
    public partial class ReplayingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveServiceInNewScopePerOperationAsync()
        {
            // given
            int expectedResolutionCount = 2;

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ProcessReplayedListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.replayingEventV2Client.ProcessReplayedListenerEventV2sAsync();
            await this.replayingEventV2Client.ProcessReplayedListenerEventV2sAsync();

            // then
            this.replayingEventCoordinationServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ProcessReplayedListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Exactly(2));

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
