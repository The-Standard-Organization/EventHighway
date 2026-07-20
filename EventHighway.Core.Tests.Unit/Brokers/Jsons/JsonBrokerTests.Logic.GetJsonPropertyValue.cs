// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Brokers.Jsons
{
    public partial class JsonBrokerTests
    {
        [Fact]
        public void ShouldGetTopLevelJsonPropertyValue()
        {
            // given
            string inputJson = "{\"Genre\":\"Action\",\"Rating\":\"8.6\"}";
            string propertyName = "Genre";

            // when
            string actualValue =
                this.jsonBroker.GetJsonPropertyValue(inputJson, propertyName);

            // then
            actualValue.Should().Be("Action");
        }

        [Fact]
        public void ShouldGetNestedJsonPropertyValueByDotPath()
        {
            // given
            string inputJson = "{\"Content\":{\"Entity\":{\"Id\":\"some-id\"}}}";
            string propertyName = "Content.Entity.Id";

            // when
            string actualValue =
                this.jsonBroker.GetJsonPropertyValue(inputJson, propertyName);

            // then
            actualValue.Should().Be("some-id");
        }

        [Fact]
        public void ShouldPreferExactPropertyNameOverDotPath()
        {
            // given
            string inputJson = "{\"Content.Id\":\"exact\",\"Content\":{\"Id\":\"nested\"}}";
            string propertyName = "Content.Id";

            // when
            string actualValue =
                this.jsonBroker.GetJsonPropertyValue(inputJson, propertyName);

            // then
            actualValue.Should().Be("exact");
        }

        [Fact]
        public void ShouldGetNumberJsonPropertyValueAsRawText()
        {
            // given
            string inputJson = "{\"Metadata\":{\"Version\":2}}";
            string propertyName = "Metadata.Version";

            // when
            string actualValue =
                this.jsonBroker.GetJsonPropertyValue(inputJson, propertyName);

            // then
            actualValue.Should().Be("2");
        }

        [Fact]
        public void ShouldGetBooleanJsonPropertyValueAsRawText()
        {
            // given
            string inputJson = "{\"Metadata\":{\"IsActive\":true}}";
            string propertyName = "Metadata.IsActive";

            // when
            string actualValue =
                this.jsonBroker.GetJsonPropertyValue(inputJson, propertyName);

            // then
            actualValue.Should().Be("true");
        }

        [Fact]
        public void ShouldReturnNullWhenJsonPropertyIsAbsent()
        {
            // given
            string inputJson = "{\"Genre\":\"Action\"}";
            string absentPropertyName = GetRandomString();

            // when
            string actualValue =
                this.jsonBroker.GetJsonPropertyValue(inputJson, absentPropertyName);

            // then
            actualValue.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnNullWhenDotPathSegmentIsNotAnObject()
        {
            // given
            string inputJson = "{\"Content\":\"some-text\"}";
            string propertyName = "Content.Id";

            // when
            string actualValue =
                this.jsonBroker.GetJsonPropertyValue(inputJson, propertyName);

            // then
            actualValue.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnNullWhenJsonPropertyValueIsNull()
        {
            // given
            string inputJson = "{\"Content\":null}";
            string propertyName = "Content";

            // when
            string actualValue =
                this.jsonBroker.GetJsonPropertyValue(inputJson, propertyName);

            // then
            actualValue.Should().BeNull();
        }
    }
}
