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
        public async Task ShouldRemoveVolatilePathsAndReturnCanonicalContentAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2(dates: GetRandomDateTimeOffset());

            EventV2 inputEventV2 = randomEventV2;
            string someJsonContent = "{\"a\":1,\"b\":2}";
            inputEventV2.Content = someJsonContent;

            string[] someVolatileContentPaths =
                new[] { GetRandomString(), GetRandomString() };

            string someCleanedContent = "{\"a\":1}";

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

            string expectedContent = someCleanedContent;

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                .Returns(loopDetectionConfiguration);

            this.jsonBrokerMock
                .Setup(broker => broker.IsValidJson(someJsonContent))
                .Returns(true);

            this.jsonBrokerMock
                .Setup(broker => broker.RemoveNode(
                    someJsonContent,
                    someVolatileContentPaths[0]))
                .Returns(someCleanedContent);

            this.jsonBrokerMock
                .Setup(broker => broker.RemoveNode(
                    someCleanedContent,
                    someVolatileContentPaths[1]))
                .Returns(someCleanedContent);

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

            this.jsonBrokerMock.Verify(broker =>
                broker.RemoveNode(
                    someJsonContent,
                    someVolatileContentPaths[0]),
                        Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.RemoveNode(
                    someCleanedContent,
                    someVolatileContentPaths[1]),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
