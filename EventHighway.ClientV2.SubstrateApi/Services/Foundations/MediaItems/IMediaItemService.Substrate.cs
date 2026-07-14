// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaItems
{
    // The substrate face of the service: every event subscription MediaItemService owns is
    // exposed here as an IEventHandler, ready to be registered and wired to a listener.
    public partial interface IMediaItemService
    {
        IEventHandler ExternalMediaItemAddedEventHandler { get; }
    }
}
