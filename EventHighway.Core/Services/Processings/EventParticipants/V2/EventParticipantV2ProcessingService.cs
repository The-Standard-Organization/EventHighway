// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Services.Processings.EventParticipants.V2
{
    internal partial class EventParticipantV2ProcessingService : IEventParticipantV2ProcessingService
    {
        private readonly IEventParticipantV2Service eventParticipantV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantV2ProcessingService(
            IEventParticipantV2Service eventParticipantV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventParticipantV2Service = eventParticipantV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventParticipantV2IsNotNull(eventParticipantV2);

            return await this.eventParticipantV2Service.AddEventParticipantV2Async(
                eventParticipantV2,
                cancellationToken);
        });

        public ValueTask<EventParticipantV2> RetrieveOrAddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveOrAddEventParticipantV2(eventParticipantV2);

            IQueryable<EventParticipantV2> allEventParticipantV2s =
                await this.eventParticipantV2Service.RetrieveAllEventParticipantV2sAsync(cancellationToken);

            EventParticipantV2 maybeEventParticipantV2 =
                allEventParticipantV2s.FirstOrDefault(participant => participant.Id == eventParticipantV2.Id);

            if (maybeEventParticipantV2 is not null)
                return maybeEventParticipantV2;

            return await this.eventParticipantV2Service.AddEventParticipantV2Async(
                eventParticipantV2,
                cancellationToken);
        });

        public async ValueTask<IQueryable<EventParticipantV2>> RetrieveEventParticipantV2sByQueryAsync(
            EventParticipantV2Query eventParticipantV2Query,
            CancellationToken cancellationToken = default)
        {
            IQueryable<EventParticipantV2> eventParticipantV2s =
                await this.eventParticipantV2Service.RetrieveAllEventParticipantV2sAsync(
                    cancellationToken);

            if (eventParticipantV2Query.Name is not null)
            {
                eventParticipantV2s = eventParticipantV2s.Where(eventParticipantV2 =>
                    eventParticipantV2.Name == eventParticipantV2Query.Name);
            }

            if (eventParticipantV2Query.IsActive is not null)
            {
                eventParticipantV2s = eventParticipantV2s.Where(eventParticipantV2 =>
                    eventParticipantV2.IsActive == eventParticipantV2Query.IsActive);
            }

            if (eventParticipantV2Query.IsSecretRequired is not null)
            {
                eventParticipantV2s = eventParticipantV2s.Where(eventParticipantV2 =>
                    eventParticipantV2.IsSecretRequired == eventParticipantV2Query.IsSecretRequired);
            }

            if (eventParticipantV2Query.CreatedFrom is not null)
            {
                eventParticipantV2s = eventParticipantV2s.Where(eventParticipantV2 =>
                    eventParticipantV2.CreatedDate >= eventParticipantV2Query.CreatedFrom);
            }

            if (eventParticipantV2Query.CreatedTo is not null)
            {
                eventParticipantV2s = eventParticipantV2s.Where(eventParticipantV2 =>
                    eventParticipantV2.CreatedDate <= eventParticipantV2Query.CreatedTo);
            }

            return eventParticipantV2s
                .OrderByDescending(eventParticipantV2 => eventParticipantV2.CreatedDate)
                .ThenBy(eventParticipantV2 => eventParticipantV2.Id)
                .Skip(eventParticipantV2Query.Skip)
                .Take(eventParticipantV2Query.Take);
        }

        public ValueTask<IQueryable<EventParticipantV2>> RetrieveAllEventParticipantV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventParticipantV2Service.RetrieveAllEventParticipantV2sAsync(cancellationToken);
        });

        public ValueTask<EventParticipantV2> RetrieveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventParticipantV2Id(eventParticipantV2Id);

            return await this.eventParticipantV2Service.RetrieveEventParticipantV2ByIdAsync(
                eventParticipantV2Id,
                cancellationToken);
        });

        public ValueTask<EventParticipantV2> ModifyEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventParticipantV2IsNotNull(eventParticipantV2);

            return await this.eventParticipantV2Service.ModifyEventParticipantV2Async(
                eventParticipantV2,
                cancellationToken);
        });

        public ValueTask<EventParticipantV2> RemoveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventParticipantV2Id(eventParticipantV2Id);

            return await this.eventParticipantV2Service.RemoveEventParticipantV2ByIdAsync(
                eventParticipantV2Id,
                cancellationToken);
        });
    }
}
