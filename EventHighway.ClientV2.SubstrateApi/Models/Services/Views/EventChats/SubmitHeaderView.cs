// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats
{
    public sealed class SubmitHeaderView
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsCredential { get; set; }
    }
}
