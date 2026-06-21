// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Brokers.Hashings
{
    public partial class HashBrokerTests
    {
        [Fact]
        public void ShouldGenerateSameHashForIdenticalContent()
        {
            // given
            string randomContent = GetRandomString();
            string inputContent = randomContent;

            // when
            string firstHash = this.hashBroker.GenerateSha256Hash(inputContent);
            string secondHash = this.hashBroker.GenerateSha256Hash(inputContent);

            // then
            firstHash.Should().Be(secondHash);
        }

        [Fact]
        public void ShouldGenerateDifferentHashForDifferentContent()
        {
            // given
            string firstContent = GetRandomString();
            string secondContent = GetRandomString();

            // when
            string firstHash = this.hashBroker.GenerateSha256Hash(firstContent);
            string secondHash = this.hashBroker.GenerateSha256Hash(secondContent);

            // then
            firstHash.Should().NotBe(secondHash);
        }

        [Fact]
        public void ShouldGenerateKnownSha256HashForKnownInput()
        {
            // given
            string knownInput = "hello";
            string expectedHash = "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824";

            // when
            string actualHash = this.hashBroker.GenerateSha256Hash(knownInput);

            // then
            actualHash.Should().Be(expectedHash);
        }
    }
}
