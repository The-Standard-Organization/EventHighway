// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace EventHighway.ClientV2.SubstrateApi.Views.Pages
{
    /// <summary>
    /// The one page: two tabs over the same highway. Events — the default — is the live submit and
    /// receive chat; Process Flow explains the round trip the chat performs, in steps and diagrams.
    /// </summary>
    public partial class Home : ComponentBase
    {
        private const string EventsTab = "events";
        private const string ProcessFlowTab = "process-flow";

        private string activeTab = EventsTab;
    }
}
