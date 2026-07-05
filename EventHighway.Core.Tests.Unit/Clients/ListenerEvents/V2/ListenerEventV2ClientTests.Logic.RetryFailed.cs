// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Fact]
        public async Task ShouldRetryFailedListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            this.retryingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetryFailedListenerEventV2sAsync(randomCancellationToken))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.listenerEventV2Client
                .RetryFailedListenerEventV2sAsync(randomCancellationToken);

            // then
            this.retryingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetryFailedListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.retryingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
