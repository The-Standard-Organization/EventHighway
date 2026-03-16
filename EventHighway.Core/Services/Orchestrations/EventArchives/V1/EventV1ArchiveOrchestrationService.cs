// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventV1ArchiveOrchestrationService : IEventV1ArchiveOrchestrationService
    {
        private readonly IListenerEventV1ArchiveService listenerEventV1ArchiveService;
        private readonly IEventV1ArchiveService eventV1ArchiveService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV1ArchiveOrchestrationService(
            IListenerEventV1ArchiveService listenerEventV1ArchiveService,
            IEventV1ArchiveService eventV1ArchiveService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventV1ArchiveService = listenerEventV1ArchiveService;
            this.eventV1ArchiveService = eventV1ArchiveService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask AddEventV1ArchiveWithListenerEventV1ArchivesAsync(EventV1Archive eventV1Archive) =>
        TryCatch(async () =>
        {
            ValidateEventV1Arhive(eventV1Archive);

            foreach (ListenerEventV1Archive listenerEventV1Archive in eventV1Archive.ListenerEventV1Archives)
            {
                await this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(listenerEventV1Archive);
            }

            await this.eventV1ArchiveService.AddEventV1ArchiveAsync(eventV1Archive);
        });

        public ValueTask RemoveEventV1ArchivesAsync(ArchiveDeletionPolicy policy, int duration = 0) =>
        TryCatch(async () =>
        {
            await ValidateArchiveDeletionType(policy);

            DateTimeOffset currentTime =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            DateTimeOffset cutOffDate =
                (DateTimeOffset)GetCutoffDate(policy, currentTime, duration);

            await this.eventV1ArchiveService.RemoveEventV1ArchivesAsync(cutOffDate);
        });

        private static DateTimeOffset? GetCutoffDate(
            ArchiveDeletionPolicy policy,
            DateTimeOffset currentTime,
            int customDays = 0)
        {
            return policy switch
            {
                ArchiveDeletionPolicy.Daily => currentTime.AddDays(-1),
                ArchiveDeletionPolicy.Weekly => currentTime.AddDays(-7),
                ArchiveDeletionPolicy.Monthly => currentTime.AddMonths(-1),
                ArchiveDeletionPolicy.Quarterly => currentTime.AddMonths(-3),
                ArchiveDeletionPolicy.Yearly => currentTime.AddYears(-1),
                ArchiveDeletionPolicy.Duration => currentTime.AddDays(-customDays),
                _ => null
            };
        }
    }
}
