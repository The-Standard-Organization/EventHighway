// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.ClientV2.App.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        ValueTask RegisterCourseCompletedAddressAsync(
            EventAddressV2 address,
            CancellationToken cancellationToken = default);

        ValueTask RegisterCourseCompletedListenerAsync(
            EventListenerV2 listener,
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> RaiseCourseCompletedAsync(
            string content,
            CancellationToken cancellationToken = default);
    }
}
