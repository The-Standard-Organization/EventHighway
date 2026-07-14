// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        ValueTask<EventListenerV2> RegisterListenerAsync(
            EventListenerV2 eventListener,
            CancellationToken cancellationToken = default);
    }
}
