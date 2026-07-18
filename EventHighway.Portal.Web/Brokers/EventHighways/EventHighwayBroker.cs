// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------


namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker : IEventHighwayBroker
    {
        private readonly ClientV2Provider clientV2Provider;

        public EventHighwayBroker(ClientV2Provider clientV2Provider) =>
            this.clientV2Provider = clientV2Provider;

        // Every database call is routed through clientV2Provider.ExecuteAsync, which builds the V2
        // client once (retrying after a failed cold-start). The V2 client is thread-safe — each
        // operation opens its own DI scope with a fresh EF DbContext — so ExecuteAsync no longer
        // serializes access; the dashboard panels run concurrently without colliding on a shared
        // context. A database outage still surfaces as an exception the calling view service can
        // catch and report, rather than crashing the app during DI resolution.
    }
}
