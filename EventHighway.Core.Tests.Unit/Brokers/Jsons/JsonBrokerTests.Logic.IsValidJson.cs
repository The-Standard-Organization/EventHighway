// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Brokers.Jsons
{
    public partial class JsonBrokerTests
    {
        [Fact]
        public void ShouldReturnTrueForValidJson()
        {
            // given
            string validJson = "{\"key\":\"value\"}";

            // when
            bool actualResult = this.jsonBroker.IsValidJson(validJson);

            // then
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseForInvalidJson()
        {
            // given
            string invalidContent = GetRandomString();

            // when
            bool actualResult = this.jsonBroker.IsValidJson(invalidContent);

            // then
            actualResult.Should().BeFalse();
        }
    }
}
