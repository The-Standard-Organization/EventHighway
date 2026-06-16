// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V2
{
    public partial class EventAddressV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveOrRegisterEventAddressV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 randomEventAddressV2 =
                CreateRandomEventAddressV2();

            EventAddressV2 inputEventAddressV2 =
                randomEventAddressV2;

            EventAddressV2 retrievedOrRegisteredEventAddressV2 =
                inputEventAddressV2;

            EventAddressV2 expectedEventAddressV2 =
                retrievedOrRegisteredEventAddressV2.DeepClone();

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveOrRegisterEventAddressV2Async(
                    inputEventAddressV2,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedOrRegisteredEventAddressV2);

            // when
            EventAddressV2 actualEventAddressV2 =
                await this.eventAddressV2Client
                    .RetrieveOrRegisterEventAddressV2Async(
                        inputEventAddressV2,
                        randomCancellationToken);

            // then
            actualEventAddressV2.Should().BeEquivalentTo(
                expectedEventAddressV2);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveOrRegisterEventAddressV2Async(
                    inputEventAddressV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
