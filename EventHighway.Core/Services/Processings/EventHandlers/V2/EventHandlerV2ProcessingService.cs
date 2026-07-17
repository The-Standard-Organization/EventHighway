// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
using EventHighway.Core.Services.Foundations.EventHandlers.V2;

namespace EventHighway.Core.Services.Processings.EventHandlers.V2
{
    internal partial class EventHandlerV2ProcessingService : IEventHandlerV2ProcessingService
    {
        private readonly IEventHandlerV2Service eventHandlerV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventHandlerV2ProcessingService(
            IEventHandlerV2Service eventHandlerV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventHandlerV2Service = eventHandlerV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEventHandler> RegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventHandlerFunction(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRegisterEventHandlerV2(eventHandler);

            return await this.eventHandlerV2Service.AddEventHandlerV2Async(
                eventHandler,
                cancellationToken);
        }));

        public ValueTask<EventHandlerV2> RemoveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventHandlerV2Function(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRemoveEventHandlerV2ById(eventHandlerV2Id);

            return await this.eventHandlerV2Service.RemoveEventHandlerV2ByIdAsync(
                eventHandlerV2Id,
                cancellationToken);
        }));

        public ValueTask<IQueryable<EventHandlerV2>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventHandlerV2sFunction(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<EventHandlerV2> unionedEventHandlerV2s =
                await RetrieveUnionedEventHandlerV2sAsync(cancellationToken);

            return unionedEventHandlerV2s.AsQueryable();
        }));

        public ValueTask<IReadOnlyList<EventHandlerV2>> RetrieveEventHandlerV2sByQueryAsync(
            EventHandlerV2Query eventHandlerV2Query,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventHandlerV2ListFunction(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<EventHandlerV2> unionedEventHandlerV2s =
                await RetrieveUnionedEventHandlerV2sAsync(cancellationToken);

            return ApplyEventHandlerV2Query(unionedEventHandlerV2s, eventHandlerV2Query);
        }));

        private async ValueTask<List<EventHandlerV2>> RetrieveUnionedEventHandlerV2sAsync(
            CancellationToken cancellationToken)
        {
            IQueryable<IEventHandler> registeredEventHandlers =
                await this.eventHandlerV2Service.RetrieveAllEventHandlerV2sAsync(cancellationToken);

            List<EventHandlerV2> registeredEventHandlerV2s =
                registeredEventHandlers.Select(eventHandler => new EventHandlerV2
                {
                    Id = eventHandler.Id,
                    Name = eventHandler.Name
                }).ToList();

            List<EventHandlerV2> storageEventHandlerV2s =
                (await this.eventHandlerV2Service.RetrieveAllEventHandlerV2sFromStorageAsync(
                    cancellationToken)).ToList();

            // Registered handlers take precedence on id conflicts — they hold the live delegate.
            return registeredEventHandlerV2s.Concat(
                storageEventHandlerV2s.Where(storageEventHandlerV2 =>
                    registeredEventHandlerV2s.All(registeredEventHandlerV2 =>
                        registeredEventHandlerV2.Id != storageEventHandlerV2.Id)))
                .ToList();
        }

        private static IReadOnlyList<EventHandlerV2> ApplyEventHandlerV2Query(
            IEnumerable<EventHandlerV2> eventHandlerV2s,
            EventHandlerV2Query eventHandlerV2Query)
        {
            if (eventHandlerV2Query.Name is not null)
            {
                eventHandlerV2s = eventHandlerV2s.Where(eventHandlerV2 =>
                    eventHandlerV2.Name == eventHandlerV2Query.Name);
            }

            return eventHandlerV2s
                .OrderBy(eventHandlerV2 => eventHandlerV2.Name)
                .ThenBy(eventHandlerV2 => eventHandlerV2.Id)
                .Skip(eventHandlerV2Query.Skip)
                .Take(eventHandlerV2Query.Take)
                .ToList();
        }

        public ValueTask<IEventHandler> RetrieveOrRegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventHandlerFunction(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRetrieveOrRegisterEventHandlerV2(eventHandler);

            IQueryable<IEventHandler> allEventHandlers =
                await this.eventHandlerV2Service.RetrieveAllEventHandlerV2sAsync(cancellationToken);

            IEventHandler maybeEventHandler =
                allEventHandlers.FirstOrDefault(handler => handler.Id == eventHandler.Id);

            if (maybeEventHandler is not null)
                return maybeEventHandler;

            IQueryable<EventHandlerV2> storageEventHandlerV2s =
                await this.eventHandlerV2Service.RetrieveAllEventHandlerV2sFromStorageAsync(
                    cancellationToken);

            EventHandlerV2 maybeStorageEventHandlerV2 =
                storageEventHandlerV2s.FirstOrDefault(storageEventHandlerV2 =>
                    storageEventHandlerV2.Id == eventHandler.Id);

            // Already persisted by a previous process run — only the in-memory delegate
            // registration is needed; inserting again would violate the stable-id key.
            if (maybeStorageEventHandlerV2 is not null)
            {
                return await this.eventHandlerV2Service.RegisterEventHandlerV2Async(
                    eventHandler,
                    cancellationToken);
            }

            return await this.eventHandlerV2Service.AddEventHandlerV2Async(
                eventHandler,
                cancellationToken);
        }));
    }
}
