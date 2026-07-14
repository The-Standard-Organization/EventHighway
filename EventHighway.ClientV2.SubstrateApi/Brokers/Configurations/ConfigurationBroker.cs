// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApi.Models.Brokers.Configurations;
using Microsoft.Extensions.Configuration;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.Configurations
{
    public sealed class ConfigurationBroker : IConfigurationBroker
    {
        private const string SubstrateApiSection = "SubstrateApi";

        private readonly IConfiguration configuration;

        public ConfigurationBroker(IConfiguration configuration) =>
            this.configuration = configuration;

        public SubstrateApiConfigurations GetSubstrateApiConfigurations() =>
            new SubstrateApiConfigurations
            {
                SubmitUrl = this.configuration[$"{SubstrateApiSection}:SubmitUrl"],
                ParticipantId = this.configuration[$"{SubstrateApiSection}:ParticipantId"],
                ParticipantSecret = this.configuration[$"{SubstrateApiSection}:ParticipantSecret"]
            };
    }
}
