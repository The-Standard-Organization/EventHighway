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

        public ConfigurationBroker(IConfiguration configuration) =>
            this.configuration = configuration;

        public JoesRestApiConfigurations GetJoesRestApiConfigurations() =>
            new JoesRestApiConfigurations
            {
                Url = this.configuration["JoesRestApi:Url"],
                Secret = this.configuration["JoesRestApi:Secret"]
            };
    }
}
