// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Clients.EventHighways;

namespace EventHighway.ClientV2.App.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker : IEventSubstrateBroker
    {
        private readonly EventHighwayClient eventHighwayClient;

        public EventSubstrateBroker(EventHighwayClient eventHighwayClient) =>
            this.eventHighwayClient = eventHighwayClient;

        public async ValueTask FirePendingEventsAsync(CancellationToken cancellationToken = default) =>
            await this.eventHighwayClient.V2.EventV2Client.FireScheduledPendingEventV2sAsync();
    }
}
