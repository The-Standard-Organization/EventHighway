// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldRemoveEventAddressV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventAddressV2Id = GetRandomId();
            Guid inputEventAddressV2Id = randomEventAddressV2Id;

            EventAddressV2 randomEventAddressV2 =
                CreateRandomEventAddressV2();

            EventAddressV2 removedEventAddressV2 =
                randomEventAddressV2;

            EventAddressV2 expectedEventAddressV2 =
                removedEventAddressV2.DeepClone();

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    inputEventAddressV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(removedEventAddressV2);

            // when
            EventAddressV2 actualEventAddressV2 =
                await this.eventAddressV2ProcessingService
                    .RemoveEventAddressV2ByIdAsync(
                        inputEventAddressV2Id,
                        randomCancellationToken);

            // then
            actualEventAddressV2.Should().BeEquivalentTo(
                expectedEventAddressV2);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RemoveEventAddressV2ByIdAsync(
                    inputEventAddressV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
