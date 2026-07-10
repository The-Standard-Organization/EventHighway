// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApp.Models.Events;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        // Numbers are written as JSON strings so they can be used as promoted properties
        // (promotion reads JSON values as strings) and read back by handlers.
        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            NumberHandling =
                JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString
        };

        public async ValueTask<EventV2> EmitAsync<TContent>(
            EventEnvelope<TContent> envelope,
            CancellationToken cancellationToken = default)
        {
            var eventV2 = new EventV2
            {
                Id = envelope.EventId,
                Content = JsonSerializer.Serialize(envelope.Content, SerializationOptions),
                EventName = envelope.EventName,
                EventAddressV2Id = envelope.EventAddressId,
                EventParticipantV2Id = envelope.ParticipantId,
                EventParticipantV2Secret = envelope.Secret,
                ScheduledDate = envelope.ScheduledDate,
                CreatedDate = envelope.OccurredAt,
                UpdatedDate = envelope.OccurredAt
            };

            return await SubmitEventAsync(eventV2, cancellationToken);
        }
    }
}
