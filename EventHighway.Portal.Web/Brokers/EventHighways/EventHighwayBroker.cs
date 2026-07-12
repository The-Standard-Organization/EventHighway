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
        // client once (retrying after a failed cold-start) and then serializes all access to its single
        // non-thread-safe EF DbContext — the dashboard fans its panels out concurrently, so without this
        // gate they collide on the shared context. A database outage still surfaces as an exception the
        // calling view service can catch and report, rather than crashing the app during DI resolution.
    }
}
