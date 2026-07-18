// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Configurations.Dispatch;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Unit.Brokers.Configurations
{
    public partial class ConfigurationBrokerTests
    {
        [Fact]
        public void ShouldGetDispatchConfiguration()
        {
            // given
            DispatchConfiguration randomDispatchConfiguration = CreateRandomDispatchConfiguration();
            DispatchConfiguration expectedDispatchConfiguration = randomDispatchConfiguration.DeepClone();

            EventHighwayConfiguration eventHighwayConfiguration =
                new EventHighwayConfiguration
                {
                    Dispatch = randomDispatchConfiguration
                };

            var configurationBroker =
                new ConfigurationBroker(eventHighwayConfiguration);

            // when
            DispatchConfiguration actualDispatchConfiguration =
                configurationBroker.GetDispatchConfiguration();

            // then
            actualDispatchConfiguration.Should().BeEquivalentTo(expectedDispatchConfiguration);
        }

        private static DispatchConfiguration CreateRandomDispatchConfiguration()
        {
            return new DispatchConfiguration
            {
                HandlerTimeout = TimeSpan.FromSeconds(GetRandomNumber())
            };
        }
    }
}
