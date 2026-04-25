// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V1
{
    public partial class EventListenerV1ServiceTests
    {
        [Fact]
        private async Task ShouldRemoveEventListenerV1ByIdAsync()
        {
            // given
            Guid randomEventListenerV1Id = GetRandomId();
            Guid inputEventListenerV1Id = randomEventListenerV1Id;

            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1();

            EventListenerV1 retrievedEventListenerV1 =
                randomEventListenerV1;

            EventListenerV1 deletedEventListenerV1 =
                retrievedEventListenerV1;

            EventListenerV1 expectedEventListenerV1 =
                deletedEventListenerV1.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventListenerByIdV1Async(
                    inputEventListenerV1Id))
                        .ReturnsAsync(retrievedEventListenerV1);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventListenerV1Async(
                    retrievedEventListenerV1))
                        .ReturnsAsync(deletedEventListenerV1);

            // when
            EventListenerV1 actualEventListenerV1 =
                await this.eventListenerV1Service
                    .RemoveEventListenerV1ByIdAsync(
                        inputEventListenerV1Id);

            // then
            actualEventListenerV1.Should().BeEquivalentTo(
                expectedEventListenerV1);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventListenerByIdV1Async(
                    inputEventListenerV1Id),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventListenerV1Async(
                    retrievedEventListenerV1),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
