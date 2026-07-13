// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.EventListeners.V1
{
    public partial class EventListenerV1sClientTests
    {
        [Fact]
        public async Task ShouldRemoveEventListenerV1ByIdAsync()
        {
            // given
            Guid randomEventListenerV1Id = GetRandomId();
            Guid inputEventListenerV1Id = randomEventListenerV1Id;

            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1();

            EventListenerV1 removedEventListenerV1 =
                randomEventListenerV1;

            EventListenerV1 expectedEventListenerV1 =
                removedEventListenerV1.DeepClone();

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RemoveEventListenerV1ByIdAsync(
                    inputEventListenerV1Id))
                        .ReturnsAsync(removedEventListenerV1);

            // when
            EventListenerV1 actualEventListenerV1 =
                await this.eventListenerV1SClient
                    .RemoveEventListenerV1ByIdAsync(
                        inputEventListenerV1Id);

            // then
            actualEventListenerV1.Should()
                .BeEquivalentTo(expectedEventListenerV1);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RemoveEventListenerV1ByIdAsync(
                    inputEventListenerV1Id),
                        Times.Once);

            this.eventListenerV1OrchestrationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
