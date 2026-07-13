// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldRegisterEventListenerV1Async()
        {
            // given
            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1();

            EventListenerV1 inputEventListenerV1 =
                randomEventListenerV1;

            EventListenerV1 registeredEventListenerV1 =
                inputEventListenerV1;

            EventListenerV1 expectedEventListenerV1 =
                registeredEventListenerV1.DeepClone();

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.AddEventListenerV1Async(
                    inputEventListenerV1))
                        .ReturnsAsync(registeredEventListenerV1);

            // when
            EventListenerV1 actualEventListenerV1 =
                await this.eventListenerV1SClient
                    .RegisterEventListenerV1Async(
                        inputEventListenerV1);

            // then
            actualEventListenerV1.Should()
                .BeEquivalentTo(expectedEventListenerV1);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.AddEventListenerV1Async(
                    inputEventListenerV1),
                        Times.Once);

            this.eventListenerV1OrchestrationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
