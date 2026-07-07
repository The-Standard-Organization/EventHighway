// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2;

namespace EventHighway.Core.Services.Coordinations.HealthChecks.V2
{
    internal partial class HealthV2CoordinationService : IHealthV2CoordinationService
    {
        private readonly IHealthInfrastructureV2OrchestrationService healthInfrastructureV2OrchestrationService;
        private readonly IHealthEventsV2OrchestrationService healthEventsV2OrchestrationService;
        private readonly IHealthArchivedEventsV2OrchestrationService healthArchivedEventsV2OrchestrationService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public HealthV2CoordinationService(
            IHealthInfrastructureV2OrchestrationService healthInfrastructureV2OrchestrationService,
            IHealthEventsV2OrchestrationService healthEventsV2OrchestrationService,
            IHealthArchivedEventsV2OrchestrationService healthArchivedEventsV2OrchestrationService,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.healthInfrastructureV2OrchestrationService = healthInfrastructureV2OrchestrationService;
            this.healthEventsV2OrchestrationService = healthEventsV2OrchestrationService;
            this.healthArchivedEventsV2OrchestrationService = healthArchivedEventsV2OrchestrationService;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<HealthReportV2> RetrieveHealthCheckItemsReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
