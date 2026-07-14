// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats;
using EventHighway.ClientV2.SubstrateApi.Services.Views.EventChats;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EventHighway.ClientV2.SubstrateApi.Views.Components
{
    /// <summary>
    /// The call the Send button makes, published so a reader can make it themselves. Every value
    /// shown comes from the app's own configuration, so what is on screen is what the app would
    /// actually send — a documented example would drift; this cannot.
    /// </summary>
    public partial class SubmitEndpoint : ComponentBase
    {
        private const string UrlField = "url";
        private const string BodyField = "body";
        private const string CurlField = "curl";
        private const int CopiedNoticeMilliseconds = 1500;

        [Inject]
        private IEventChatsViewService EventChatsViewService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private SubmitEndpointView submitEndpoint;
        private string copiedField;

        protected override async Task OnInitializedAsync() =>
            this.submitEndpoint =
                await this.EventChatsViewService.RetrieveSubmitEndpointAsync();

        private async Task CopyAsync(string field, string value)
        {
            await this.JSRuntime.InvokeVoidAsync("eventChat.copyToClipboard", value);

            await ShowCopiedNoticeAsync(field);
        }

        // Postman imports cURL directly (Import → Raw text), so the whole request — headers and
        // body included — goes across in one paste rather than four. The command is composed by
        // the view service, quoting and all; the component only carries it to the clipboard.
        private async Task CopyAsCurlAsync()
        {
            await RefreshSampleMediaItemAsync();

            await CopyAsync(CurlField, this.submitEndpoint.CurlCommand);
        }

        private async Task CopyBodyAsync()
        {
            await RefreshSampleMediaItemAsync();

            await CopyAsync(BodyField, this.submitEndpoint.SampleBody);
        }

        // Anything carrying a body is re-minted before it is handed over, so every copy is of an
        // item the highway has not seen. Hand out the same one twice and the second submission is
        // identical content inside the loop-detection window — the substrate quarantines it, which
        // is exactly right of it and thoroughly baffling when all you did was press Copy again.
        private async Task RefreshSampleMediaItemAsync() =>
            this.submitEndpoint =
                await this.EventChatsViewService.RetrieveSubmitEndpointAsync();

        // The button says "Copied", then goes back to saying what it does. Without the reset the
        // panel would keep claiming a copy that happened a minute ago.
        private async Task ShowCopiedNoticeAsync(string field)
        {
            this.copiedField = field;
            StateHasChanged();

            await Task.Delay(TimeSpan.FromMilliseconds(CopiedNoticeMilliseconds));

            if (this.copiedField == field)
            {
                this.copiedField = null;
                StateHasChanged();
            }
        }
    }
}
