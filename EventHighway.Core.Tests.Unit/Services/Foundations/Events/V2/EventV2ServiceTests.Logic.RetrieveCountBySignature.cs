// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventV2CountBySignatureAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            TimeSpan randomWindow = TimeSpan.FromSeconds(GetRandomNumber());
            DateTimeOffset createdAfter = randomDateTimeOffset - randomWindow;

            EventV2 randomEventV2 =
                CreateRandomEventV2(dates: GetRandomDateTimeOffset());

            EventV2 inputEventV2 = randomEventV2;

            EventV2 matchingEventV2 = new EventV2
            {
                EventAddressId = inputEventV2.EventAddressId,
                EventName = inputEventV2.EventName,
                ContentHash = inputEventV2.ContentHash,
                CreatedDate = createdAfter.AddSeconds(1)
            };

            EventV2 nonMatchingEventV2 = CreateRandomEventV2();

            IQueryable<EventV2> storedEventV2s =
                new[] { matchingEventV2, nonMatchingEventV2 }.AsQueryable();

            int expectedCount = 1;

            var loopDetectionConfig = new LoopDetection
            {
                Window = randomWindow
            };

            this.storageBrokerMock
                .Setup(broker => broker.SelectAllEventV2sAsync(
                    randomCancellationToken))
                        .ReturnsAsync(storedEventV2s);

            this.dateTimeBrokerMock
                .Setup(broker => broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetectionConfig);

            // when
            int actualCount =
                await this.eventV2Service.RetrieveEventV2CountBySignatureAsync(
                    inputEventV2,
                    randomCancellationToken);

            // then
            actualCount.Should().Be(expectedCount);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldReturnZeroOnRetrieveEventV2CountBySignatureWhenLoopDetectionIsDisabledAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2(dates: GetRandomDateTimeOffset());

            EventV2 inputEventV2 = randomEventV2;
            int expectedCount = 0;

            var loopDetectionConfig = new LoopDetection
            {
                Enabled = false,
                Window = TimeSpan.FromSeconds(GetRandomNumber())
            };

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetectionConfig);

            // when
            int actualCount =
                await this.eventV2Service.RetrieveEventV2CountBySignatureAsync(
                    inputEventV2,
                    randomCancellationToken);

            // then
            actualCount.Should().Be(expectedCount);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
