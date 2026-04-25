// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V1
{
    public partial class EventAddressV1ServiceTests
    {
        [Fact]
        private async Task ShouldRemoveEventAddressV1ByIdAsync()
        {
            // given
            Guid randomEventAddressV1Id = GetRandomId();
            Guid inputEventAddressV1Id = randomEventAddressV1Id;

            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1();

            EventAddressV1 retrievedEventAddressV1 =
                randomEventAddressV1;

            EventAddressV1 deletedEventAddressV1 =
                retrievedEventAddressV1;

            EventAddressV1 expectedEventAddressV1 =
                deletedEventAddressV1.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressByIdV1Async(
                    inputEventAddressV1Id))
                        .ReturnsAsync(retrievedEventAddressV1);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventAddressV1Async(
                    retrievedEventAddressV1))
                        .ReturnsAsync(deletedEventAddressV1);

            // when
            EventAddressV1 actualEventAddressV1 =
                await this.eventAddressV1Service
                    .RemoveEventAddressByIdAsync(
                        inputEventAddressV1Id);

            // then
            actualEventAddressV1.Should().BeEquivalentTo(
                expectedEventAddressV1);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressByIdV1Async(
                    inputEventAddressV1Id),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventAddressV1Async(
                    retrievedEventAddressV1),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
