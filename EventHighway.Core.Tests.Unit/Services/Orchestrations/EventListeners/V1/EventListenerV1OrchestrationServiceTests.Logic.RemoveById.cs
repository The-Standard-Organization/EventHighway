// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRemoveEventListenerV1ByIdAsync()
        {
            // given
            Guid randomListenerEventId = GetRandomId();
            Guid inputListenerEventId = randomListenerEventId;

            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1();

            EventListenerV1 removedEventListenerV1 =
                randomEventListenerV1;

            EventListenerV1 expectedEventListenerV1 =
                removedEventListenerV1.DeepClone();

            this.eventListenerV1ProcessingServiceMock.Setup(service =>
                service.RemoveEventListenerByIdAsync(
                    inputListenerEventId))
                        .ReturnsAsync(removedEventListenerV1);

            // when
            EventListenerV1 actualEventListenerV1 =
                await this.eventListenerV1OrchestrationService
                    .RemoveEventListenerByIdAsync(
                        inputListenerEventId);

            // then
            actualEventListenerV1.Should()
                .BeEquivalentTo(expectedEventListenerV1);

            this.eventListenerV1ProcessingServiceMock.Verify(service =>
                service.RemoveEventListenerByIdAsync(
                    inputListenerEventId),
                        Times.Once);

            this.eventListenerV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.listenerEventV1ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock
                .VerifyNoOtherCalls();
        }
    }
}
