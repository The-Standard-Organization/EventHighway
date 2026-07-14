// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Brokers.DateTimes;
using EventHighway.ClientV2.SubstrateApi.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApi.Brokers.ReceivedEvents;
using EventHighway.ClientV2.SubstrateApi.Brokers.Serializations;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.ReceivedEvents
{
    // The chat log: everything the highway delivers to /receive is stamped with the moment it
    // landed and kept for the UI to render. Deliveries are recorded, never rejected on shape —
    // an event that does not parse as JSON is still an event that arrived, and the UI shows it
    // as it came.
    public partial class ReceivedEventService : IReceivedEventService
    {
        private readonly IReceivedEventBroker receivedEventBroker;
        private readonly IJsonSerializationBroker jsonSerializationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ReceivedEventService(
            IReceivedEventBroker receivedEventBroker,
            IJsonSerializationBroker jsonSerializationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.receivedEventBroker = receivedEventBroker;
            this.jsonSerializationBroker = jsonSerializationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public event Action ReceivedEventsChanged
        {
            add => this.receivedEventBroker.ReceivedEventsChanged += value;
            remove => this.receivedEventBroker.ReceivedEventsChanged -= value;
        }

        public ValueTask<ReceivedEvent> AddReceivedEventAsync(string content) =>
        TryCatch(async () =>
        {
            ValidateContent(content);

            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            var receivedEvent = new ReceivedEvent
            {
                Id = Guid.NewGuid(),
                ReceivedDate = now,
                Content = content,
                FormattedContent = await FormatContentAsync(content)
            };

            this.loggingBroker.LogInformation(
                $"Received an event on /receive at {now:HH:mm:ss} ({content.Length} characters).");

            return await this.receivedEventBroker.InsertReceivedEventAsync(receivedEvent);
        });

        public ValueTask<IQueryable<ReceivedEvent>> RetrieveAllReceivedEventsAsync() =>
        TryCatch(async () =>
            await this.receivedEventBroker.SelectAllReceivedEventsAsync());

        // Malformed JSON is a display problem, not a delivery failure — show it raw rather than
        // dropping a delivery the highway considers made.
        private async ValueTask<string> FormatContentAsync(string content)
        {
            try
            {
                return await this.jsonSerializationBroker.PrettifyAsync(content);
            }
            catch (JsonException)
            {
                return content;
            }
        }
    }
}
