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
        public async Task ShouldRetrieveEventAddressV1ByIdAsync()
        {
            // given
            Guid randomEventAddressV1Id = GetRandomId();
            Guid inputEventAddressV1Id = randomEventAddressV1Id;

            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1();

            EventAddressV1 selectedEventAddressV1 =
                randomEventAddressV1;

            EventAddressV1 expectedEventAddressV1 =
                selectedEventAddressV1.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressByIdV1Async(
                    inputEventAddressV1Id))
                        .ReturnsAsync(selectedEventAddressV1);

            // when
            EventAddressV1 actualEventAddressV1 =
                await this.eventAddressV1Service
                    .RetrieveEventAddressByIdAsync(
                        inputEventAddressV1Id);

            // then
            actualEventAddressV1.Should()
                .BeEquivalentTo(expectedEventAddressV1);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressByIdV1Async(
                    inputEventAddressV1Id),
                        Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
