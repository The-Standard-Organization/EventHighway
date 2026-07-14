// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Brokers.Configurations;
using Microsoft.Extensions.Configuration;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Brokers.Configurations
{
    internal class ConfigurationBroker : IConfigurationBroker
    {
        private readonly IConfiguration configuration;
        private readonly string sectionName;

        // The section is chosen by the consumer, not fixed by the broker: one host can hold several
        // clients of this library, each delivering to a different downstream (its own url + secret).
        public ConfigurationBroker(IConfiguration configuration, string sectionName)
        {
            this.configuration = configuration;
            this.sectionName = sectionName;
        }

        public JoesRestApiConfigurations GetJoesRestApiConfigurations() =>
            new JoesRestApiConfigurations
            {
                SectionName = this.sectionName,
                Url = this.configuration[$"{this.sectionName}:Url"],
                Secret = this.configuration[$"{this.sectionName}:Secret"]
            };
    }
}
