// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventAddresses.V2
{
    public partial class EventAddressV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRegisterEventAddressV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 randomEventAddressV2 =
                CreateRandomEventAddressV2();

            EventAddressV2 inputEventAddressV2 =
                randomEventAddressV2;

            EventAddressV2 addedEventAddressV2 =
                inputEventAddressV2;

            EventAddressV2 expectedEventAddressV2 =
                addedEventAddressV2.DeepClone();

            this.eventAddressV2ServiceMock.Setup(service =>
                service.AddEventAddressV2Async(
                    inputEventAddressV2,
                    randomCancellationToken))
                        .ReturnsAsync(addedEventAddressV2);

            // when
            EventAddressV2 actualEventAddressV2 =
                await this.eventAddressV2ProcessingService
                    .RegisterEventAddressV2Async(
                        inputEventAddressV2,
                        randomCancellationToken);

            // then
            actualEventAddressV2.Should().BeEquivalentTo(
                expectedEventAddressV2);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.AddEventAddressV2Async(
                    inputEventAddressV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
