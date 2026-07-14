// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.ReceivedEvents
{
    // In-memory on purpose: the chat is a live window onto the highway, not a second archive of it.
    // EventHighway already keeps the durable record of every delivery.
    public sealed class ReceivedEventBroker : IReceivedEventBroker
    {
        private const int MaximumRetainedEvents = 200;

        private readonly ConcurrentQueue<ReceivedEvent> receivedEvents = new();

        public event Action ReceivedEventsChanged;

        public ValueTask<ReceivedEvent> InsertReceivedEventAsync(ReceivedEvent receivedEvent)
        {
            this.receivedEvents.Enqueue(receivedEvent);

            while (this.receivedEvents.Count > MaximumRetainedEvents)
                this.receivedEvents.TryDequeue(out _);

            ReceivedEventsChanged?.Invoke();

            return ValueTask.FromResult(receivedEvent);
        }

        public ValueTask<IQueryable<ReceivedEvent>> SelectAllReceivedEventsAsync() =>
            ValueTask.FromResult(this.receivedEvents.ToArray().AsQueryable());
    }
}
