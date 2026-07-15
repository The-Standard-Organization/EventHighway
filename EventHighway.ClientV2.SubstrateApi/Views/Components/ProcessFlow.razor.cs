// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EventHighway.ClientV2.SubstrateApi.Views.Components
{
    /// <summary>
    /// The Process Flow tab: the eight-step round trip a media item makes, drawn twice — once as a
    /// flow of the pieces it passes through, once as the sequence of calls between them. Both
    /// diagrams are Mermaid definitions rendered by processFlow.js into empty, JS-owned containers.
    /// The addresses named here (NFlix-ExternalContributions and NFlix-NewReleases) are the same
    /// SeedIdentifiers the substrate is wired against, so the picture cannot drift from the wiring.
    /// </summary>
    public partial class ProcessFlow : ComponentBase
    {
        private const string FlowElementId = "process-flow-graph";
        private const string SequenceElementId = "process-flow-sequence";

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private ElementReference flowElement;
        private ElementReference sequenceElement;

        // The diagrams are static, so they are drawn exactly once — on the first render, when the
        // containers exist and (in interactive server mode) JS interop is available. Later parent
        // re-renders, such as switching back to this tab, leave the injected SVG untouched.
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender is false)
            {
                return;
            }

            await this.JSRuntime.InvokeVoidAsync(
                "processFlow.render", this.flowElement, FlowElementId, FlowDefinition);

            await this.JSRuntime.InvokeVoidAsync(
                "processFlow.render", this.sequenceElement, SequenceElementId, SequenceDefinition);
        }

        private const string FlowDefinition =
            """
            flowchart TD
                Client(["Postman / cURL / UI"])
                Submit["Submit API<br/>POST /submit"]
                Ext["ExternalMediaItemService<br/>validates credentials (mandatory)"]
                Contrib["NFlix-ExternalContributions"]:::address
                EH1{{"EventHighway"}}:::highway
                Listener1["Event Listener"]
                Handler1["Delegate Event Handler"]
                Media["InternalMediaService<br/>validate + store"]
                DB[("SQL")]
                Releases["NFlix-NewReleases"]:::address
                EH2{{"EventHighway"}}:::highway
                Others["Other Event Listeners<br/>SofaBox · Joe · Ann · FlakyBox"]
                Listener2["Event Listener"]
                Handler2["Delegate Event Handler"]
                Rest["REST API Delegate"]
                Receive["Receive API<br/>POST /receive"]
                UI(["Chat UI"])

                Client -->|"1 submit"| Submit
                Submit -->|"2 validate"| Ext
                Ext -->|"3 ExternalMediaItemAdded"| Contrib
                Contrib --> EH1
                EH1 -->|"4 dispatch"| Listener1
                Listener1 --> Handler1
                Handler1 --> Media
                Media -->|"5 store"| DB
                Media -->|"6 MediaItemAdded"| Releases
                Releases --> EH2
                EH2 --> Others
                EH2 -->|"7 dispatch"| Listener2
                Listener2 --> Handler2
                Handler2 --> Rest
                Rest -->|"POST"| Receive
                Receive -->|"8 surface"| UI

                classDef address fill:#fff3cd,stroke:#f0c000,color:#664d03;
                classDef highway fill:#cfe2ff,stroke:#0d6efd,color:#052c65;
            """;

        private const string SequenceDefinition =
            """
            sequenceDiagram
                autonumber
                actor Client as Postman / cURL / UI
                participant Submit as Submit API
                participant Ext as ExternalMediaItemService
                participant EH as EventHighway
                participant Media as InternalMediaService
                participant DB as SQL
                participant Recv as Receive API
                participant UI as Chat UI

                Client->>Submit: POST /submit - item plus credentials in headers
                Submit->>Ext: AddExternalMediaItem
                Note over Ext: credentials mandatory
                Ext->>EH: emit ExternalMediaItemAdded to NFlix-ExternalContributions
                EH-->>Media: dispatch via Delegate Event Handler (content only)
                Note over Media: validate content
                Media->>DB: store media item
                Media->>EH: emit MediaItemAdded to NFlix-NewReleases
                EH-->>Recv: SubstrateApi listener's REST delegate posts to /receive
                Recv->>UI: surface delivery on chat
            """;
    }
}
