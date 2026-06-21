// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Configurations.LoopDetections;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Unit.Brokers.Configurations
{
    public partial class ConfigurationBrokerTests
    {
        [Fact]
        public void ShouldGetLoopDetectionConfiguration()
        {
            // given
            LoopDetection randomLoopDetection = CreateRandomLoopDetection();
            LoopDetection expectedLoopDetection = randomLoopDetection.DeepClone();

            EventHighwayConfiguration eventHighwayConfiguration =
                new EventHighwayConfiguration
                {
                    LoopDetection = randomLoopDetection
                };

            var configurationBroker =
                new ConfigurationBroker(eventHighwayConfiguration);

            // when
            LoopDetection actualLoopDetection =
                configurationBroker.GetLoopDetectionConfiguration();

            // then
            actualLoopDetection.Should().BeEquivalentTo(expectedLoopDetection);
        }
    }
}
