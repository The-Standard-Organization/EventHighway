// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    internal partial class EventArchiveV1Service : IEventArchiveV1Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV1Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventArchiveV1> AddEventArchiveAsync(EventArchiveV1 eventArchive) =>
        TryCatch(async () =>
        {
            await ValidateEventArchiveOnAddAsync(eventArchive);

            return await this.storageBroker.InsertEventArchiveV1Async(eventArchive);
        });

        public ValueTask<IQueryable<EventArchiveV1>> RetrieveAllEventArchivesAsync() =>
        TryCatch(async () =>
        {
            return await this.storageBroker.SelectAllEventArchivesV1Async();
        });

        public ValueTask<EventArchiveV1> RetrieveEventArchiveByIdAsync(Guid eventArchiveId) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV1Id(eventArchiveId);

            return await this.storageBroker.SelectEventArchiveByIdV1Async(eventArchiveId);
        });
    }
}
