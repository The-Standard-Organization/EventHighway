// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    /// <summary>
    /// Abstracts the EventHighway dependency behind a single broker so the application never
    /// talks to <c>EventHighwayClient</c> directly. Swap this implementation to retarget the app
    /// at a different event substrate without touching the application code.
    /// </summary>
    public partial interface IEventSubstrateBroker
    {
    }
}
