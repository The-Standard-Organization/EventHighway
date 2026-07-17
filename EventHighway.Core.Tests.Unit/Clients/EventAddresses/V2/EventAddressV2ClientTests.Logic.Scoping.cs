// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V2
{
    public partial class EventAddressV2ClientTests
    {
        [Fact]
        public async Task ShouldResolveProcessingServiceInNewScopePerOperationAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 = CreateRandomEventAddressV2();
            Guid inputEventAddressV2Id = randomEventAddressV2.Id;
            int expectedResolutionCount = 2;

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    inputEventAddressV2Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomEventAddressV2);

            // when
            await this.eventAddressV2Client.RemoveEventAddressV2ByIdAsync(inputEventAddressV2Id);
            await this.eventAddressV2Client.RemoveEventAddressV2ByIdAsync(inputEventAddressV2Id);

            // then
            this.processingServiceResolutionCount.Should()
                .Be(expectedResolutionCount);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    inputEventAddressV2Id, It.IsAny<CancellationToken>()),
                        Times.Exactly(2));

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
