// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V1
{
    public partial class EventAddressesV1ClientTests
    {
        [Fact]
        public async Task ShouldRegisterEventAddressV1Async()
        {
            // given
            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1();

            EventAddressV1 inputEventAddressV1 =
                randomEventAddressV1;

            EventAddressV1 registeredEventAddressV1 =
                inputEventAddressV1;

            EventAddressV1 expectedEventAddressV1 =
                registeredEventAddressV1.DeepClone();

            this.eventAddressV1ServiceMock.Setup(service =>
                service.AddEventAddressV1Async(
                    inputEventAddressV1))
                        .ReturnsAsync(registeredEventAddressV1);

            // when
            EventAddressV1 actualEventAddressV1 =
                await this.eventAddressV1sClient
                    .RegisterEventAddressV1Async(
                        inputEventAddressV1);

            // then
            actualEventAddressV1.Should()
                .BeEquivalentTo(expectedEventAddressV1);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.AddEventAddressV1Async(
                    inputEventAddressV1),
                        Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
