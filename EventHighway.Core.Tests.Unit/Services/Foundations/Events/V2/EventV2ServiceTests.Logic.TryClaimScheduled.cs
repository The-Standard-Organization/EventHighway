// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
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

            this.storageBrokerMock.Setup(broker =>
                broker.ClaimScheduledEventV2Async(
                    inputEventV2Id, randomCancellationToken))
                        .ReturnsAsync(randomRowsClaimed);

            // when
            int actualRowsClaimed =
                await this.eventV2Service.TryClaimScheduledEventV2Async(
                    inputEventV2Id, randomCancellationToken);

            // then
            actualRowsClaimed.Should().Be(expectedRowsClaimed);

            this.storageBrokerMock.Verify(broker =>
                broker.ClaimScheduledEventV2Async(
                    inputEventV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
