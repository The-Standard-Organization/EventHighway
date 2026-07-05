// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Fact]
        public async Task ShouldResetRetriesForListenerEventV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomListenerEventV2Id = GetRandomId();
            Guid inputListenerEventV2Id = randomListenerEventV2Id;

            ListenerEventV2 randomListenerEventV2 =
                CreateRandomListenerEventV2();

            ListenerEventV2 resetListenerEventV2 =
                randomListenerEventV2;

            ListenerEventV2 expectedListenerEventV2 =
                resetListenerEventV2.DeepClone();

            this.listenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ResetRetriesForListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(resetListenerEventV2);

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.listenerEventV2Client
                    .ResetRetriesForListenerEventV2ByIdAsync(
                        inputListenerEventV2Id,
                        randomCancellationToken);

            // then
            actualListenerEventV2.Should()
                .BeEquivalentTo(expectedListenerEventV2);

            this.listenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ResetRetriesForListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
