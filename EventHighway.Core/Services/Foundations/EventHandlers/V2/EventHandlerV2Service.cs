// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal partial class EventHandlerV2Service : IEventHandlerV2Service
    {
        private readonly IEventHandlerBroker eventHandlerBroker;
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventHandlerV2Service(
            IEventHandlerBroker eventHandlerBroker,
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHandlerBroker = eventHandlerBroker;
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEventHandler> AddEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventHandlerFunction(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventHandlerV2OnAdd(eventHandler);

            var eventHandlerV2 = new EventHandlerV2
            {
                Id = eventHandler.Id,
                Name = eventHandler.Name
            };

            await this.storageBroker.InsertEventHandlerV2Async(eventHandlerV2, cancellationToken);
            this.eventHandlerBroker.Register(eventHandler);

            return eventHandler;
        }));

        public ValueTask<IQueryable<IEventHandler>> RetrieveAllEventHandlerV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningQueryableEventHandlersFunction(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await ValueTask.FromResult(this.eventHandlerBroker.GetAll().AsQueryable());
        }));

        public async ValueTask<IQueryable<EventHandlerV2>> RetrieveAllEventHandlerV2sFromStorageAsync(
            CancellationToken cancellationToken = default) =>
            await this.storageBroker.SelectAllEventHandlerV2sAsync(cancellationToken);

        public ValueTask<IEventHandler> RetrieveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventHandlerFunction(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventHandlerV2Id(eventHandlerV2Id);

            IEventHandler maybeEventHandler = this.eventHandlerBroker.GetAll()
                .FirstOrDefault(eventHandler => eventHandler.Id == eventHandlerV2Id);

            ValidateEventHandlerV2Exists(maybeEventHandler, eventHandlerV2Id);

            return await ValueTask.FromResult(maybeEventHandler);
        }));

        public ValueTask<EventHandlerV2> RemoveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventHandlerV2Function(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventHandlerV2Id(eventHandlerV2Id);

            EventHandlerV2 maybeEventHandlerV2 =
                await this.storageBroker.SelectEventHandlerV2ByIdAsync(
                    eventHandlerV2Id, cancellationToken);

            ValidateStorageEventHandlerV2Exists(maybeEventHandlerV2, eventHandlerV2Id);

            EventHandlerV2 deletedEventHandlerV2 =
                await this.storageBroker.DeleteEventHandlerV2Async(
                    maybeEventHandlerV2, cancellationToken);

            this.eventHandlerBroker.Remove(eventHandlerV2Id);

            return deletedEventHandlerV2;
        }));
    }
}
