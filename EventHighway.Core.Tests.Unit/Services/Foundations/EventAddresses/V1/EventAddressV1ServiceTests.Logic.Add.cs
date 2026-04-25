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
        public async Task ShouldAddEventAddressV1Async()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1(
                    dates: randomDateTimeOffset);

            EventAddressV1 inputEventAddressV1 =
                randomEventAddressV1;

            EventAddressV1 insertedEventAddressV1 =
                inputEventAddressV1;

            EventAddressV1 expectedEventAddressV1 =
                insertedEventAddressV1.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEventAddressV1Async(
                    inputEventAddressV1))
                        .ReturnsAsync(insertedEventAddressV1);

            // when
            EventAddressV1 actualEventAddressV1 =
                await this.eventAddressV1Service
                    .AddEventAddressAsync(
                        inputEventAddressV1);

            // then
            actualEventAddressV1.Should().BeEquivalentTo(
                expectedEventAddressV1);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV1Async(
                    inputEventAddressV1),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
