// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats;
using EventHighway.ClientV2.SubstrateApi.Services.Views.EventChats;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EventHighway.ClientV2.SubstrateApi.Views.Components
{
    /// <summary>
    /// The chat. It shows what the highway delivered to /receive, and it can put something new onto
    /// the highway — but it never takes a short cut to do it: Send posts to the public /submit
    /// endpoint, the same one Postman would use, and then waits like everyone else for the item to
    /// come back around as a delivery.
    /// </summary>
    public partial class EventChat : IDisposable
    {
        [Inject]
        private IEventChatsViewService EventChatsViewService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private List<ReceivedEventView> receivedEvents = new();
        private MediaSubmissionView submission;
        private ElementReference logElement;
        private string composerContent = string.Empty;
        private bool isSending;
        private bool shouldScrollToLatest;

        protected override async Task OnInitializedAsync()
        {
            this.EventChatsViewService.ReceivedEventsChanged += HandleReceivedEventsChanged;

            this.receivedEvents =
                await this.EventChatsViewService.RetrieveReceivedEventsAsync();

            this.composerContent =
                await this.EventChatsViewService.GenerateSampleMediaItemAsync();

            this.shouldScrollToLatest = true;
        }

        private async Task SendAsync()
        {
            this.isSending = true;
            this.submission = null;

            try
            {
                this.submission =
                    await this.EventChatsViewService.SubmitMediaItemAsync(this.composerContent);

                // A submitted item is spent: the substrate quarantines a second copy of the same
                // content as a loop. Re-arming the box with a fresh id means Send can be pressed
                // again straight away and mean it.
                if (this.submission.IsAccepted)
                {
                    this.composerContent =
                        await this.EventChatsViewService.GenerateSampleMediaItemAsync();
                }
            }
            catch (Exception exception)
            {
                this.submission = new MediaSubmissionView
                {
                    IsAccepted = false,
                    Message = $"The submission could not be sent: {exception.Message}"
                };
            }
            finally
            {
                this.isSending = false;
            }
        }

        // Deliveries arrive on a web request, not on this circuit, so the re-render has to be
        // marshalled back onto the component's own context before it can touch the UI.
        private async void HandleReceivedEventsChanged()
        {
            try
            {
                await InvokeAsync(async () =>
                {
                    this.receivedEvents =
                        await this.EventChatsViewService.RetrieveReceivedEventsAsync();

                    this.shouldScrollToLatest = true;

                    StateHasChanged();
                });
            }
            catch (ObjectDisposedException)
            {
                // The circuit went away mid-delivery — there is no longer a chat to tell.
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (this.shouldScrollToLatest is false)
            {
                return;
            }

            this.shouldScrollToLatest = false;

            await this.JSRuntime.InvokeVoidAsync(
                "eventChat.scrollToLatest",
                this.logElement);
        }

        public void Dispose() =>
            this.EventChatsViewService.ReceivedEventsChanged -= HandleReceivedEventsChanged;
    }
}
