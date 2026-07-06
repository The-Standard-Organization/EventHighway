// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2
{
    internal partial class HealthInfrastructureV2OrchestrationService : IHealthInfrastructureV2OrchestrationService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly IEventListenerV2Service eventListenerV2Service;
        private readonly IEventParticipantV2Service eventParticipantV2Service;
        private readonly ILoggingBroker loggingBroker;

        public HealthInfrastructureV2OrchestrationService(
            IEventAddressV2Service eventAddressV2Service,
            IEventListenerV2Service eventListenerV2Service,
            IEventParticipantV2Service eventParticipantV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.eventListenerV2Service = eventListenerV2Service;
            this.eventParticipantV2Service = eventParticipantV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<HealthReportV2> RetrieveHealthReportV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<EventAddressV2> eventAddresses =
                await this.eventAddressV2Service.RetrieveAllEventAddressV2sAsync(cancellationToken);

            IQueryable<EventListenerV2> eventListeners =
                await this.eventListenerV2Service.RetrieveAllEventListenerV2sAsync(cancellationToken);

            IQueryable<EventParticipantV2> eventParticipants =
                await this.eventParticipantV2Service.RetrieveAllEventParticipantV2sAsync(cancellationToken);

            return new HealthReportV2
            {
                Period = period,
                WindowStart = windowStart
            };
        });
    }
}
