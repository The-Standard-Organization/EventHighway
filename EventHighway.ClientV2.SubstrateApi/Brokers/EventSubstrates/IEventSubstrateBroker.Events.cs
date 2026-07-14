// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        ValueTask<EventV2> SubmitEventAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);

        ValueTask FirePendingEventsAsync(CancellationToken cancellationToken = default);
    }
}
