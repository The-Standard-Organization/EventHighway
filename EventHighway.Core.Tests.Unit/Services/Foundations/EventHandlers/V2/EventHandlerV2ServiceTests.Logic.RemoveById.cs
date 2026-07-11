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

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public async Task ShouldRemoveEventHandlerV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventHandlerV2 randomEventHandlerV2 = CreateRandomEventHandlerV2();
            Guid inputEventHandlerV2Id = randomEventHandlerV2.Id;
            EventHandlerV2 storageEventHandlerV2 = randomEventHandlerV2;
            EventHandlerV2 deletedEventHandlerV2 = storageEventHandlerV2;
            EventHandlerV2 expectedEventHandlerV2 = deletedEventHandlerV2.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventHandlerV2ByIdAsync(
                    inputEventHandlerV2Id, randomCancellationToken))
                        .ReturnsAsync(storageEventHandlerV2);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventHandlerV2Async(
                    storageEventHandlerV2, randomCancellationToken))
                        .ReturnsAsync(deletedEventHandlerV2);

            // when
            EventHandlerV2 actualEventHandlerV2 =
                await this.eventHandlerV2Service.RemoveEventHandlerV2ByIdAsync(
                    inputEventHandlerV2Id, randomCancellationToken);

            // then
            actualEventHandlerV2.Should().BeEquivalentTo(expectedEventHandlerV2);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventHandlerV2ByIdAsync(
                    inputEventHandlerV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventHandlerV2Async(
                    storageEventHandlerV2, randomCancellationToken),
                        Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Remove(inputEventHandlerV2Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
