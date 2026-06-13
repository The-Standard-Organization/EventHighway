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

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRemoveListenerEventV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomListenerEventV2Id = GetRandomId();
            Guid inputListenerEventV2Id = randomListenerEventV2Id;

            ListenerEventV2 randomListenerEventV2 =
                CreateRandomListenerEventV2();

            ListenerEventV2 removedListenerEventV2 =
                randomListenerEventV2;

            ListenerEventV2 expectedListenerEventV2 =
                removedListenerEventV2.DeepClone();

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(removedListenerEventV2);

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.eventListenerV2OrchestrationService
                    .RemoveListenerEventV2ByIdAsync(
                        inputListenerEventV2Id,
                        randomCancellationToken);

            // then
            actualListenerEventV2.Should().BeEquivalentTo(
                expectedListenerEventV2);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
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
