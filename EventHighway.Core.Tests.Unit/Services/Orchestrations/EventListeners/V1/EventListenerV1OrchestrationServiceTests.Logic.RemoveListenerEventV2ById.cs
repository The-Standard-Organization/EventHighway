// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldRemoveListenerEventV1ByIdAsync()
        {
            // given
            Guid randomListenerEventId = GetRandomId();
            Guid inputListenerEventId = randomListenerEventId;

            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1();

            ListenerEventV1 removedListenerEventV1 =
                randomListenerEventV1;

            ListenerEventV1 expectedListenerEventV1 =
                removedListenerEventV1.DeepClone();

            this.listenerEventV1ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventByIdAsync(
                    inputListenerEventId))
                        .ReturnsAsync(removedListenerEventV1);

            // when
            ListenerEventV1 actualListenerEventV1 =
                await this.eventListenerV1OrchestrationService
                    .RemoveListenerEventByIdAsync(
                        inputListenerEventId);

            // then
            actualListenerEventV1.Should()
                .BeEquivalentTo(expectedListenerEventV1);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventByIdAsync(
                    inputListenerEventId),
                        Times.Once);

            this.listenerEventV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventListenerV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock
                .VerifyNoOtherCalls();
        }
    }
}
