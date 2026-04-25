// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;

namespace EventHighway.Core.Services.Foundations.ListenerEvents
{
    internal interface IListenerEventService
    {
        ValueTask<ListenerEvent> AddListenerEventAsync(ListenerEvent listenerEvent);
        ValueTask<ListenerEvent> ModifyListenerEventAsync(ListenerEvent listenerEvent);
    }
}
