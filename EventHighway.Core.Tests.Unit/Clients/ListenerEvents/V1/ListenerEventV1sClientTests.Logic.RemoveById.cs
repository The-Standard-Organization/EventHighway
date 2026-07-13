// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V1
{
    public partial class ListenerEventV1sClientTests
    {
        [Fact]
        public async Task ShouldRemoveListenerEventV1ByIdAsync()
        {
            // given
            Guid randomListenerEventV1Id = GetRandomId();
            Guid inputListenerEventV1Id = randomListenerEventV1Id;

            ListenerEventV1 randomListenerEventV1 =
                CreateRandomListenerEventV1();

            ListenerEventV1 removedListenerEventV1 =
                randomListenerEventV1;

            ListenerEventV1 expectedListenerEventV1 =
                removedListenerEventV1.DeepClone();

            this.eventListenerV1OrchestrationServiceMock.Setup(service =>
                service.RemoveListenerEventV1ByIdAsync(
                    inputListenerEventV1Id))
                        .ReturnsAsync(removedListenerEventV1);

            // when
            ListenerEventV1 actualListenerEventV1 =
                await this.listenerEventV1sClient
                    .RemoveListenerEventV1ByIdAsync(
                        inputListenerEventV1Id);

            // then
            actualListenerEventV1.Should()
                .BeEquivalentTo(expectedListenerEventV1);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RemoveListenerEventV1ByIdAsync(
                    inputListenerEventV1Id),
                        Times.Once);

            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
