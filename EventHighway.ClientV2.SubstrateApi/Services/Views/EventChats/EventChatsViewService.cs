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

            return new SubmitEndpointView
            {
                Method = mediaSubmissionEndpoint.Method,
                Url = mediaSubmissionEndpoint.Url,
                Headers = mediaSubmissionEndpoint.Headers.Select(AsView).ToList(),
                SampleBody = await GenerateSampleMediaItemAsync()
            };
        });

        public ValueTask<string> GenerateSampleMediaItemAsync() =>
        TryCatch(async () =>
        {
            var sampleMediaItem = new MediaItem
            {
                Id = Guid.NewGuid(),
                Title = "Yellowstone",
                Type = "Series",
                Genres = new List<string> { "Drama", "Western" },
                Rating = 8.6
            };

            string serializedMediaItem =
                await this.jsonSerializationBroker.SerializeAsync(sampleMediaItem);

            return await this.jsonSerializationBroker.PrettifyAsync(serializedMediaItem);
        });

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
