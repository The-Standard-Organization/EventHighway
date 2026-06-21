// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.VolatilePaths
{
    public partial class VolatilePathServiceTests
    {
        [Fact]
        public async Task ShouldRemoveVolatilePathsFromValidJsonContentAsync()
        {
            // given
            string inputContent = "{\"z\":3,\"a\":1,\"m\":2}";
            string[] inputPaths = new[] { "z", "m" };

            string afterFirstRemoval = "{\"a\":1,\"m\":2}";
            string afterSecondRemoval = "{\"a\":1}";
            string expectedContent = "{\"a\":1}";

            var mockSequence = new MockSequence();

            this.jsonBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.IsValidJson(inputContent))
                .Returns(true);

            this.jsonBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.RemoveNode(inputContent, inputPaths[0]))
                .Returns(afterFirstRemoval);

            this.jsonBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.RemoveNode(afterFirstRemoval, inputPaths[1]))
                .Returns(afterSecondRemoval);

            // when
            string actualContent =
                await this.volatilePathService.RemoveVolatilePathsAsync(
                    inputContent,
                    inputPaths);

            // then
            actualContent.Should().Be(expectedContent);

            this.jsonBrokerMock.Verify(
                broker => broker.IsValidJson(inputContent),
                Times.Once);

            this.jsonBrokerMock.Verify(
                broker => broker.RemoveNode(inputContent, inputPaths[0]),
                Times.Once);

            this.jsonBrokerMock.Verify(
                broker => broker.RemoveNode(afterFirstRemoval, inputPaths[1]),
                Times.Once);

            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
