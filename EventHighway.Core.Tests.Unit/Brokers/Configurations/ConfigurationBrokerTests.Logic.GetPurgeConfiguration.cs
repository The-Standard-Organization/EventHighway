// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Configurations.Purging;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Unit.Brokers.Configurations
{
    public partial class ConfigurationBrokerTests
    {
        [Fact]
        public void ShouldGetPurgeConfiguration()
        {
            // given
            PurgeConfiguration randomPurgeConfiguration = CreateRandomPurgeConfiguration();
            PurgeConfiguration expectedPurgeConfiguration = randomPurgeConfiguration.DeepClone();

            EventHighwayConfiguration eventHighwayConfiguration =
                new EventHighwayConfiguration
                {
                    Purging = randomPurgeConfiguration
                };

            var configurationBroker =
                new ConfigurationBroker(eventHighwayConfiguration);

            // when
            PurgeConfiguration actualPurgeConfiguration =
                configurationBroker.GetPurgeConfiguration();

            // then
            actualPurgeConfiguration.Should().BeEquivalentTo(expectedPurgeConfiguration);
        }
    }
}
