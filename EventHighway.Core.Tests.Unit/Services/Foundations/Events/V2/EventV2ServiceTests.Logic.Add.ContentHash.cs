// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
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
            string someJsonContent = "{\"a\":1,\"b\":2}";
            string[] someVolatileContentPaths = new[] { GetRandomString(), GetRandomString() };
            string someCleanedContent = "{\"a\":1}";

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

            string expectedContent = someCleanedContent;

            // when
            string actualContent =
                await this.eventV2Service.RemoveVolatilePathsAsync(
                    someJsonContent,
                    someVolatileContentPaths);

            // then
            actualContent.Should().Be(expectedContent);

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

            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
