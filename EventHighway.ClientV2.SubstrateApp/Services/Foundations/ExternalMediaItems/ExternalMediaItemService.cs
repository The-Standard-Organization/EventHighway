// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.ClientV2.Seed;
using EventHighway.ClientV2.SubstrateApp.Brokers.DateTimes;
using EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApp.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApp.Models.Events;
using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems
{
    // The authenticated public intake. An external media item arrives with the contributing
    // participant's id and secret; once validated as present, it is published onto the
    // "NFlix-ExternalContributions" address as an ExternalMediaItemAdded event carrying those
    // credentials (which EventHighway.Core verifies). The participant attribution lives on that
    // EventV2 record — MediaItemService's substrate handler listening on the address receives
    // only the content and funnels it into the media catalogue. Both addresses are provisioned at seed
    // time (see SubstrateDemo.Setup), so this service only emits.
    public partial class ExternalMediaItemService : IExternalMediaItemService
    {
        private const string ExternalMediaItemAddedEventName = "ExternalMediaItemAdded";

        private readonly IEventSubstrateBroker eventSubstrateBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ExternalMediaItemService(
            IEventSubstrateBroker eventSubstrateBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventSubstrateBroker = eventSubstrateBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask AddExternalMediaItemAsync(ExternalMediaItem externalMediaItem) =>
        await TryCatch(async () =>
        {
            ValidateExternalMediaItemOnAdd(externalMediaItem);

            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            await this.eventSubstrateBroker.EmitAsync(
                new EventEnvelope<MediaItem>
                {
                    EventName = ExternalMediaItemAddedEventName,
                    Content = externalMediaItem.MediaItem,
                    EventAddressId = SeedIdentifiers.NFlixExternalContributionsAddress,
                    ParticipantId = externalMediaItem.ParticipantId,
                    Secret = externalMediaItem.Secret,
                    OccurredAt = now
                });

            return externalMediaItem.MediaItem;
        });
    }
}
