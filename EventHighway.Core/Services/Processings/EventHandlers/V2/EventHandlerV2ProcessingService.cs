// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
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

        public async ValueTask<IQueryable<EventHandlerV2>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default)
        {
            IQueryable<IEventHandler> registeredEventHandlers =
                await this.eventHandlerV2Service.RetrieveAllEventHandlerV2sAsync(cancellationToken);

            if (registeredEventHandlers.Any())
            {
                return registeredEventHandlers.Select(eventHandler => new EventHandlerV2
                {
                    Id = eventHandler.Id,
                    Name = eventHandler.Name
                });
            }

            return await this.eventHandlerV2Service.RetrieveAllEventHandlerV2sFromStorageAsync(
                cancellationToken);
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

            return await this.eventHandlerV2Service.AddEventHandlerV2Async(
                eventHandler,
                cancellationToken);
        }));
    }
}
