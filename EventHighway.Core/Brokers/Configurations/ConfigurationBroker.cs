// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Configurations.LoopDetections;

namespace EventHighway.Core.Brokers.Configurations
{
    internal class ConfigurationBroker : IConfigurationBroker
    {
        private readonly EventHighwayConfiguration configuration;

        public ConfigurationBroker(EventHighwayConfiguration configuration) =>
            this.configuration = configuration;

        public HealthConfiguration GetHealthConfiguration() =>
            this.configuration.Health;

        public BatchConfiguration GetBatchConfiguration() =>
            this.configuration.BatchProcessing;

        public LoopDetection GetLoopDetectionConfiguration() =>
            this.configuration.LoopDetection;
    }
}
