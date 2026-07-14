// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.ClientV2.SubstrateApi.Models.Brokers.Configurations
{
    /// <summary>
    /// The app's own intake, as it is configured: where /submit lives, and the participant
    /// credentials it presents when it calls itself. The chat UI publishes these so anyone with
    /// Postman can make the same call by hand.
    /// </summary>
    public class SubstrateApiConfigurations
    {
        public string SubmitUrl { get; set; }
        public string ParticipantId { get; set; }
        public string ParticipantSecret { get; set; }
    }
}
