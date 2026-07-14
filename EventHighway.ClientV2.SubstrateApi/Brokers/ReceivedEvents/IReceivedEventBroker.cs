// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.ReceivedEvents
{
    /// <summary>
    /// The chat log's storage. Deliveries land on a web request and are read back on a Blazor
    /// circuit — two different threads — so the store also announces its own changes, letting the
    /// UI re-render the moment something arrives instead of polling for it.
    /// </summary>
    public interface IReceivedEventBroker
    {
        ValueTask<ReceivedEvent> InsertReceivedEventAsync(ReceivedEvent receivedEvent);
        ValueTask<IQueryable<ReceivedEvent>> SelectAllReceivedEventsAsync();

        event Action ReceivedEventsChanged;
    }
}
