// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldRemoveEventAddressV1ByIdAsync()
        {
            // given
            Guid randomEventAddressV1Id = GetRandomId();
            Guid inputEventAddressV1Id = randomEventAddressV1Id;

            EventAddressV1 randomEventAddressV1 =
                CreateRandomEventAddressV1();

            EventAddressV1 removedEventAddressV1 =
                randomEventAddressV1;

            EventAddressV1 expectedEventAddressV1 =
                removedEventAddressV1.DeepClone();

            this.eventAddressV1ServiceMock.Setup(service =>
                service.RemoveEventAddressV1ByIdAsync(
                    inputEventAddressV1Id))
                        .ReturnsAsync(removedEventAddressV1);

            // when
            EventAddressV1 actualEventAddressV1 =
                await this.eventAddressV1sClient
                    .RemoveEventAddressV1ByIdAsync(
                        inputEventAddressV1Id);

            // then
            actualEventAddressV1.Should()
                .BeEquivalentTo(expectedEventAddressV1);

            this.eventAddressV1ServiceMock.Verify(service =>
                service.RemoveEventAddressV1ByIdAsync(
                    inputEventAddressV1Id),
                        Times.Once);

            this.eventAddressV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
