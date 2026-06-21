// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Brokers.Jsons
{
    public partial class JsonBrokerTests
    {
        [Fact]
        public void ShouldRemoveNodeAtGivenPath()
        {
            // given
            string inputJson = "{\"a\":1,\"b\":2}";
            string pathToRemove = "b";
            string expectedJson = "{\"a\":1}";

            // when
            string actualJson = this.jsonBroker.RemoveNode(inputJson, pathToRemove);

            // then
            actualJson.Should().Be(expectedJson);
        }

        [Fact]
        public void ShouldReturnUnchangedJsonWhenPathIsAbsent()
        {
            // given
            string inputJson = "{\"a\":1}";
            string absentPath = GetRandomString();

            // when
            string actualJson = this.jsonBroker.RemoveNode(inputJson, absentPath);

            // then
            actualJson.Should().Be(inputJson);
        }
    }
}
