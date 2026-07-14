// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
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
        // body included — can go across in one paste rather than four.
        private async Task CopyAsCurlAsync()
        {
            string headers = string.Join(
                separator: " ",
                values: this.submitEndpoint.Headers.Select(header =>
                    $"-H \"{header.Name}: {header.Value}\""));

            string body = this.submitEndpoint.SampleBody
                .Replace("\r\n", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("  ", string.Empty);

            string curl =
                $"curl -X {this.submitEndpoint.Method} {this.submitEndpoint.Url} " +
                $"{headers} -d '{body}'";

            await CopyAsync(CurlField, curl);
        }

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
