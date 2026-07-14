// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats
{
    /// <summary>
    /// What to tell the person who pressed Send: whether the intake took the item, and — when it
    /// did not — the reason in words they can act on.
    /// </summary>
    public sealed class MediaSubmissionView
    {
        public bool IsAccepted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
