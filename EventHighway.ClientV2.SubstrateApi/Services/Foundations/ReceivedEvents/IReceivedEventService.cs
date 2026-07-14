// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.ReceivedEvents
{
    public interface IReceivedEventService
    {
        ValueTask<ReceivedEvent> AddReceivedEventAsync(string content);
        ValueTask<IQueryable<ReceivedEvent>> RetrieveAllReceivedEventsAsync();

        event Action ReceivedEventsChanged;
    }
}
