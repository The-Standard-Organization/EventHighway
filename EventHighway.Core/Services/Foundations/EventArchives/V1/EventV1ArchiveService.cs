// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    internal partial class EventV1ArchiveService : IEventV1ArchiveService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV1ArchiveService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV1Archive> AddEventV1ArchiveAsync(
            EventV1Archive eventV1Archive) => TryCatch(async () =>
        {
            await ValidateEventV1ArchiveOnAddAsync(eventV1Archive);

            return await this.storageBroker.InsertEventV1ArchiveAsync(eventV1Archive);
        });

        public ValueTask<IQueryable<EventV1Archive>> RetrieveAllEventV1ArchivesAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllEventV1ArchivesAsync());

        public async ValueTask<EventV1Archive> RetrieveEventV1ArchiveByIdAsync(Guid eventArchiveV1Id)
        {
            return await this.storageBroker.SelectEventV1ArchiveByIdAsync(eventArchiveV1Id);
        }

        public ValueTask<int> RemoveEventV1ArchivesAsync(
            DateTimeOffset cutOffDate) => TryCatch(async () =>
        {
            await ValidateCutOffDate(cutOffDate);

            return await this.storageBroker.DeleteEventV1ArchivesAsync(cutOffDate);
        });
    }
}
