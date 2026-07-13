// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events;

namespace EventHighway.Core.Clients.Events
{
    public interface IEventsClient
    {
        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask<Event> SubmitEventAsync(Event @event);
    }
}
