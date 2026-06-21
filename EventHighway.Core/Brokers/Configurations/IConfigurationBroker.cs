// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Configurations.LoopDetections;

namespace EventHighway.Core.Brokers.Configurations
{
    internal interface IConfigurationBroker
    {
        HealthConfiguration GetHealthConfiguration();
        BatchConfiguration GetBatchConfiguration();
        LoopDetection GetLoopDetectionConfiguration();
    }
}
