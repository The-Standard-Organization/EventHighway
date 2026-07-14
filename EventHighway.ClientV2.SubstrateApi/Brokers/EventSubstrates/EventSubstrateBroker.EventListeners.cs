// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        // Idempotent on the listener's (stable) Id: RetrieveOrRegister reuses an existing row with
        // the same Id, so re-running the seed does not insert duplicate listeners.
        public ValueTask<EventListenerV2> RegisterListenerAsync(
            EventListenerV2 eventListener,
            CancellationToken cancellationToken = default) =>
            this.databaseGate.ExecuteAsync(
                () => this.eventHighwayClient.V2.EventListenerV2Client
                    .RetrieveOrRegisterEventListenerV2Async(eventListener, cancellationToken),
                cancellationToken);
    }
}
