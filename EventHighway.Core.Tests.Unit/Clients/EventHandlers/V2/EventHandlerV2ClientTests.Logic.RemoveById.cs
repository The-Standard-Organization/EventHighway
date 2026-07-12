// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventHandlers.V2
{
    public partial class EventHandlerV2ClientTests
    {
        [Fact]
        public async Task ShouldRemoveEventHandlerV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventHandlerV2 randomEventHandlerV2 = CreateRandomEventHandlerV2();
            Guid inputEventHandlerV2Id = randomEventHandlerV2.Id;
            EventHandlerV2 removedEventHandlerV2 = randomEventHandlerV2;
            EventHandlerV2 expectedEventHandlerV2 = removedEventHandlerV2.DeepClone();

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    inputEventHandlerV2Id, randomCancellationToken))
                        .ReturnsAsync(removedEventHandlerV2);

            // when
            EventHandlerV2 actualEventHandlerV2 =
                await this.eventHandlerV2Client.RemoveEventHandlerV2ByIdAsync(
                    inputEventHandlerV2Id, randomCancellationToken);

            // then
            actualEventHandlerV2.Should().BeEquivalentTo(expectedEventHandlerV2);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventHandlerV2ByIdAsync(
                    inputEventHandlerV2Id, randomCancellationToken),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
