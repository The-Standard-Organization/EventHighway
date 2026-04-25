// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V1
{
    public partial class EventV1ServiceTests
    {
        [Fact]
        private async Task ShouldRemoveEventV1ByIdAsync()
        {
            // given
            Guid randomEventV1Id = GetRandomId();
            Guid inputEventV1Id = randomEventV1Id;
            EventV1 randomEventV1 = CreateRandomEventV1();
            EventV1 retrievedEventV1 = randomEventV1;
            EventV1 deletedEventV1 = retrievedEventV1;

            EventV1 expectedEventV1 =
                deletedEventV1.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventByIdV1Async(
                    inputEventV1Id))
                        .ReturnsAsync(retrievedEventV1);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventV1Async(
                    retrievedEventV1))
                        .ReturnsAsync(deletedEventV1);

            // when
            EventV1 actualEventV1 =
                await this.eventV1Service
                    .RemoveEventV1ByIdAsync(
                        inputEventV1Id);

            // then
            actualEventV1.Should().BeEquivalentTo(
                expectedEventV1);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventByIdV1Async(
                    inputEventV1Id),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventV1Async(
                    retrievedEventV1),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
