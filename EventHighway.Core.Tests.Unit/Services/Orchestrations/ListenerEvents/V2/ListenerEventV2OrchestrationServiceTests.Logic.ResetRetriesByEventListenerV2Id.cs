// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ListenerEvents.V2
{
    public partial class ListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldResetRetriesForListenerEventV2ByEventListenerV2IdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventListenerV2Id = GetRandomId();
            Guid inputEventListenerV2Id = randomEventListenerV2Id;

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.listenerEventV2OrchestrationService
                .ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken);

            // then
            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    inputEventListenerV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
