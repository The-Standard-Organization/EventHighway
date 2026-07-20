// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Brokers.Jsons
{
    public partial class JsonBrokerTests
    {
        [Fact]
        public void ShouldReturnTrueWhenTopLevelPropertyExists()
        {
            // given
            string inputJson = "{\"Genre\":\"Action\"}";
            string propertyName = "Genre";

            // when
            bool actualResult =
                this.jsonBroker.CheckIfPropertyExist(inputJson, propertyName);

            // then
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueWhenNestedPropertyExistsByDotPath()
        {
            // given
            string inputJson = "{\"Content\":{\"Entity\":{\"Id\":\"some-id\"}}}";
            string propertyName = "Content.Entity.Id";

            // when
            bool actualResult =
                this.jsonBroker.CheckIfPropertyExist(inputJson, propertyName);

            // then
            actualResult.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseWhenPropertyIsAbsent()
        {
            // given
            string inputJson = "{\"Genre\":\"Action\"}";
            string absentPropertyName = GetRandomString();

            // when
            bool actualResult =
                this.jsonBroker.CheckIfPropertyExist(inputJson, absentPropertyName);

            // then
            actualResult.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnFalseWhenDotPathSegmentIsNotAnObject()
        {
            // given
            string inputJson = "{\"Content\":\"some-text\"}";
            string propertyName = "Content.Id";

            // when
            bool actualResult =
                this.jsonBroker.CheckIfPropertyExist(inputJson, propertyName);

            // then
            actualResult.Should().BeFalse();
        }
    }
}
