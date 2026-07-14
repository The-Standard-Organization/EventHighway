// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApi.Brokers.Serializations;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;
using EventHighway.ClientV2.SubstrateApi.Models.MediaSubmissions;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents;
using EventHighway.ClientV2.SubstrateApi.Models.Services.Views.EventChats;
using EventHighway.ClientV2.SubstrateApi.Services.Foundations.MediaSubmissions;
using EventHighway.ClientV2.SubstrateApi.Services.Foundations.ReceivedEvents;

namespace EventHighway.ClientV2.SubstrateApi.Services.Views.EventChats
{
    public partial class EventChatsViewService : IEventChatsViewService
    {
        private readonly IReceivedEventService receivedEventService;
        private readonly IMediaSubmissionService mediaSubmissionService;
        private readonly IJsonSerializationBroker jsonSerializationBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventChatsViewService(
            IReceivedEventService receivedEventService,
            IMediaSubmissionService mediaSubmissionService,
            IJsonSerializationBroker jsonSerializationBroker,
            ILoggingBroker loggingBroker)
        {
            this.receivedEventService = receivedEventService;
            this.mediaSubmissionService = mediaSubmissionService;
            this.jsonSerializationBroker = jsonSerializationBroker;
            this.loggingBroker = loggingBroker;
        }

        public event Action ReceivedEventsChanged
        {
            add => this.receivedEventService.ReceivedEventsChanged += value;
            remove => this.receivedEventService.ReceivedEventsChanged -= value;
        }

        public ValueTask<List<ReceivedEventView>> RetrieveReceivedEventsAsync() =>
        TryCatch(async () =>
        {
            IQueryable<ReceivedEvent> receivedEvents =
                await this.receivedEventService.RetrieveAllReceivedEventsAsync();

            return receivedEvents
                .OrderBy(receivedEvent => receivedEvent.ReceivedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<MediaSubmissionView> SubmitMediaItemAsync(
            string content,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            MediaSubmission mediaSubmission =
                await this.mediaSubmissionService.SubmitMediaItemAsync(
                    content,
                    cancellationToken);

            return AsView(mediaSubmission);
        });

        public ValueTask<SubmitEndpointView> RetrieveSubmitEndpointAsync() =>
        TryCatch(async () =>
        {
            MediaSubmissionEndpoint mediaSubmissionEndpoint =
                await this.mediaSubmissionService.RetrieveMediaSubmissionEndpointAsync();

            // One sample item, shown two ways: indented to be read, and on one line to be pasted
            // into a command. Both carry the same id, so copying the body and copying the command
            // do not disagree about which item they submit.
            string sampleMediaItem =
                await this.jsonSerializationBroker.SerializeAsync(CreateSampleMediaItem());

            return new SubmitEndpointView
            {
                Method = mediaSubmissionEndpoint.Method,
                Url = mediaSubmissionEndpoint.Url,
                Headers = mediaSubmissionEndpoint.Headers.Select(AsView).ToList(),
                SampleBody = await this.jsonSerializationBroker.PrettifyAsync(sampleMediaItem),
                CurlCommand = ComposeCurlCommand(mediaSubmissionEndpoint, sampleMediaItem)
            };
        });

        public ValueTask<string> GenerateSampleMediaItemAsync() =>
        TryCatch(async () =>
        {
            string serializedMediaItem =
                await this.jsonSerializationBroker.SerializeAsync(CreateSampleMediaItem());

            return await this.jsonSerializationBroker.PrettifyAsync(serializedMediaItem);
        });

        private static MediaItem CreateSampleMediaItem() =>
            new MediaItem
            {
                Id = Guid.NewGuid(),
                Title = "Yellowstone",
                Type = "Series",
                Genres = new List<string> { "Drama", "Western" },
                Rating = 8.6
            };

        // Quoted for the shell this app is most likely to be run from. Windows' Command Prompt does
        // not treat a single quote as a string delimiter at all, so the bash-style
        // -d '{"Id":"…"}' that every curl example on the internet uses arrives at curl as a
        // handful of broken arguments — it posts a body starting with a stray quote and then tries
        // to read the rest of the JSON as more URLs.
        //
        // Double quotes with the inner quotes escaped are understood by cmd, by bash, and by
        // Postman's cURL importer, so this one string works wherever it is pasted.
        private static string ComposeCurlCommand(
            MediaSubmissionEndpoint mediaSubmissionEndpoint,
            string body)
        {
            string headers = string.Join(
                separator: " ",
                values: mediaSubmissionEndpoint.Headers.Select(header =>
                    $"-H \"{header.Name}: {header.Value}\""));

            string escapedBody = body.Replace("\"", "\\\"");

            return $"curl -X {mediaSubmissionEndpoint.Method} {mediaSubmissionEndpoint.Url} " +
                $"{headers} -d \"{escapedBody}\"";
        }

        private static ReceivedEventView AsView(ReceivedEvent receivedEvent) =>
            new ReceivedEventView
            {
                Id = receivedEvent.Id,
                ReceivedDate = receivedEvent.ReceivedDate,
                Content = receivedEvent.FormattedContent
            };

        private static SubmitHeaderView AsView(MediaSubmissionHeader mediaSubmissionHeader) =>
            new SubmitHeaderView
            {
                Name = mediaSubmissionHeader.Name,
                Value = mediaSubmissionHeader.Value,
                IsCredential = mediaSubmissionHeader.IsCredential
            };

        // The intake answers in HTTP; the chat answers in English. A refusal is normal traffic
        // here (an unknown participant, a bad secret, a media item missing a title), so it is
        // reported as a result and not raised as a failure.
        private static MediaSubmissionView AsView(MediaSubmission mediaSubmission) =>
            new MediaSubmissionView
            {
                IsAccepted = mediaSubmission.IsAccepted,

                Message = mediaSubmission.IsAccepted
                    ? "Submitted. It will appear above once the highway delivers it back."
                    : $"The /submit endpoint refused this item ({mediaSubmission.ResponseCode}): " +
                        $"{mediaSubmission.Response}"
            };
    }
}
