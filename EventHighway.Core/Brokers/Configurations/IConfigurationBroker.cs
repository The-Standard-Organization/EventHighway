// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Dispatch;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Configurations.Purging;
using EventHighway.Core.Models.Configurations.Retries;

namespace EventHighway.Core.Brokers.Configurations
{
    internal interface IConfigurationBroker
    {
        HealthConfiguration GetHealthConfiguration();
        BatchConfiguration GetBatchConfiguration();
        LoopDetection GetLoopDetectionConfiguration();
        RetryConfiguration GetRetryConfiguration();
        PurgeConfiguration GetPurgeConfiguration();
        DispatchConfiguration GetDispatchConfiguration();
    }
}
