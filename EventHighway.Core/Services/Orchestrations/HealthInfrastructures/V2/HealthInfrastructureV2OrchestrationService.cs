// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
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

        public ValueTask<InfrastructureHealthV2> RetrieveInfrastructureHealthV2Async(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
