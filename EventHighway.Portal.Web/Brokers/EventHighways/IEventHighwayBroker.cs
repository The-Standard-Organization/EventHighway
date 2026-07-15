// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    // The EventHighway V2 client (IClientV2) is an external component, so it is wrapped
    // by this broker. The contract is split into partial files per sub-domain, mirroring
    // the EventHighway.ClientV2.SubstrateApi broker organization. Domain methods are added
    // alongside the feature (view service) that consumes them.
    public partial interface IEventHighwayBroker
    { }
}
