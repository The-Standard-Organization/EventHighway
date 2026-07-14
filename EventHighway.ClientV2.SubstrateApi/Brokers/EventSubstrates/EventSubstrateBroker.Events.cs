// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        // An immediate event is also dispatched inline: by the time this returns, every listener on
        // the address has had its turn — including the one that posts back to this app's /receive.
        // The gate is held for that whole dispatch, which is why it has to tolerate the re-entry
        // that dispatch causes.
        public ValueTask<EventV2> SubmitEventAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
            this.databaseGate.ExecuteAsync(
                () => this.eventHighwayClient.V2.EventV2Client
                    .SubmitEventV2Async(eventV2, cancellationToken),
                cancellationToken);

        public ValueTask FirePendingEventsAsync(CancellationToken cancellationToken = default) =>
            this.databaseGate.ExecuteAsync(
                () => this.eventHighwayClient.V2.EventV2Client
                    .FireScheduledPendingEventV2sAsync(cancellationToken),
                cancellationToken);
    }
}
