// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Fact]
        public async Task ShouldResetRetriesForListenerEventV2ByEventListenerV2IdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventListenerV2Id = GetRandomId();
            Guid inputEventListenerV2Id = randomEventListenerV2Id;

            this.listenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.listenerEventV2Client
                .ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken);

            // then
            this.listenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
