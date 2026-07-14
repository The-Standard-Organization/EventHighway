// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Brokers.Configurations
{
    public class JoesRestApiConfigurations
    {
        /// <summary>
        /// The configuration section these values were read from, so a missing url or secret is
        /// reported against the key the host actually has to fix.
        /// </summary>
        public string SectionName { get; set; }

        public string Url { get; set; }
        public string Secret { get; set; }
    }
}
