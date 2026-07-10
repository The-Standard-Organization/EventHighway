// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems
{
    // An external contribution: a media item accompanied by the contributing
    // participant's credentials. The credentials are mandatory here; once present,
    // EventHighway.Core verifies they are valid when the event reaches the substrate.
    public class ExternalMediaItem
    {
        public MediaItem MediaItem { get; set; } = new MediaItem();
        public Guid ParticipantId { get; set; }
        public string Secret { get; set; } = string.Empty;
    }
}
