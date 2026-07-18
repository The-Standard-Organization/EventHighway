// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveOrchestrationServiceInNewScopePerOperationAsync()
        {
            // given
            ListenerEventV2 randomListenerEventV2 = CreateRandomListenerEventV2();
            Guid inputListenerEventV2Id = randomListenerEventV2.Id;
            int expectedResolutionCount = 2;

            this.listenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    inputListenerEventV2Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomListenerEventV2);

            // when
            await this.listenerEventV2Client.RemoveListenerEventV2ByIdAsync(
                inputListenerEventV2Id);

            await this.listenerEventV2Client.RemoveListenerEventV2ByIdAsync(
                inputListenerEventV2Id);

            // then
            this.orchestrationServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.listenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    inputListenerEventV2Id, It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.retryingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
