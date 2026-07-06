// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
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

            long totalEventAddresses = eventAddresses.LongCount();
            long totalEventListeners = eventListeners.LongCount();
            long totalParticipants = eventParticipants.LongCount();

            long registeredHandlers = eventListeners
                .Select(eventListener => eventListener.HandlerId)
                .Distinct()
                .LongCount();

            return new HealthReportV2
            {
                Period = period,
                WindowStart = windowStart,

                HealthCheckItems = new List<HealthCheckItemV2>
                {
                    MapToInfrastructureItem(
                        item: "Total Event Addresses",
                        value: totalEventAddresses,
                        description: "Total number of registered event addresses."),

                    MapToInfrastructureItem(
                        item: "Total Event Listeners",
                        value: totalEventListeners,
                        description: "Total number of registered event listeners."),

                    MapToInfrastructureItem(
                        item: "Total Participants",
                        value: totalParticipants,
                        description: "Total number of registered participants."),

                    MapToInfrastructureItem(
                        item: "Registered Handlers",
                        value: registeredHandlers,
                        description: "Number of distinct registered event handlers.")
                },

                AddressUsage = eventAddresses
                    .Select(eventAddress => new EventAddressUsageV2
                    {
                        EventAddressV2Id = eventAddress.Id,
                        Name = eventAddress.Name,
                        Description = eventAddress.Description,

                        ActiveListeners = eventListeners
                            .LongCount(eventListener =>
                                eventListener.EventAddressV2Id == eventAddress.Id)
                    })
                    .ToList(),

                ParticipantUsage = eventParticipants
                    .Select(eventParticipant => new ParticipantUsageV2
                    {
                        EventParticipantV2Id = eventParticipant.Id,
                        Name = eventParticipant.Name,
                        ContactEmail = eventParticipant.ContactEmail,
                        ContactPhone = eventParticipant.ContactPhone,
                        IsActive = eventParticipant.IsActive,

                        OwnedListeners = eventListeners
                            .LongCount(eventListener =>
                                eventListener.EventParticipantV2Id == eventParticipant.Id)
                    })
                    .ToList()
            };
        });

        private static HealthCheckItemV2 MapToInfrastructureItem(
            string item,
            long value,
            string description)
        {
            return new HealthCheckItemV2
            {
                Grouping = "Infrastructure",
                Item = item,
                Value = value.ToString(CultureInfo.InvariantCulture),
                Description = description,
                StatusCode = (int)HealthStatusV2.NA,
                Status = nameof(HealthStatusV2.NA)
            };
        }
    }
}
