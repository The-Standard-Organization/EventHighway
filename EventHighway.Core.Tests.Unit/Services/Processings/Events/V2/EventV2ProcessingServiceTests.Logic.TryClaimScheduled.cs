// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldTryClaimScheduledEventV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventV2Id = Guid.NewGuid();
            Guid inputEventV2Id = randomEventV2Id;
            int randomRowsClaimed = GetRandomNumber();
            int expectedRowsClaimed = randomRowsClaimed;

            this.eventV2ServiceMock.Setup(service =>
                service.TryClaimScheduledEventV2Async(
                    inputEventV2Id, randomCancellationToken))
                        .ReturnsAsync(randomRowsClaimed);

            // when
            int actualRowsClaimed =
                await this.eventV2ProcessingService.TryClaimScheduledEventV2Async(
                    inputEventV2Id, randomCancellationToken);

            // then
            actualRowsClaimed.Should().Be(expectedRowsClaimed);

            this.eventV2ServiceMock.Verify(service =>
                service.TryClaimScheduledEventV2Async(
                    inputEventV2Id, randomCancellationToken),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
