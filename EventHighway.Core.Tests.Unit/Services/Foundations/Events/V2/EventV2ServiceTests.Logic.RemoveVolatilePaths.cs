// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldCanonicalizeOnlyWhenNoVolatilePathsConfiguredForEventAddressOnRemoveVolatilePathsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2(dates: GetRandomDateTimeOffset());

            EventV2 inputEventV2 = randomEventV2;
            string someJsonContent = "{\"a\":1}";
            inputEventV2.Content = someJsonContent;

            var loopDetectionConfiguration = new LoopDetection
            {
                VolatilePaths = new List<VolatilePaths>()
            };

            string expectedContent = someJsonContent;

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                .Returns(loopDetectionConfiguration);

            this.jsonBrokerMock
                .Setup(broker => broker.IsValidJson(someJsonContent))
                .Returns(true);

            // when
            string actualContent =
                await this.eventV2Service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken);

            // then
            actualContent.Should().Be(expectedContent);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.IsValidJson(someJsonContent),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnContentAsIsWhenContentIsNotValidJsonOnRemoveVolatilePathsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2(dates: GetRandomDateTimeOffset());

            EventV2 inputEventV2 = randomEventV2;
            string someNonJsonContent = GetRandomString();
            inputEventV2.Content = someNonJsonContent;

            string[] someVolatileContentPaths =
                new[] { GetRandomString(), GetRandomString() };

            var loopDetectionConfiguration = new LoopDetection
            {
                VolatilePaths = new List<VolatilePaths>
                {
                    new VolatilePaths
                    {
                        EventAddressId = inputEventV2.EventAddressId,
                        VolatileContentPaths = someVolatileContentPaths
                    }
                }
            };

            string expectedContent = someNonJsonContent;

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                .Returns(loopDetectionConfiguration);

            this.jsonBrokerMock
                .Setup(broker => broker.IsValidJson(someNonJsonContent))
                .Returns(false);

            // when
            string actualContent =
                await this.eventV2Service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken);

            // then
            actualContent.Should().Be(expectedContent);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.IsValidJson(someNonJsonContent),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
