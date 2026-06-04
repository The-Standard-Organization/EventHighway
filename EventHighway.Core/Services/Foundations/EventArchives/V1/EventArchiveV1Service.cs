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

        public ValueTask<EventArchiveV1> AddEventArchiveV1Async(EventArchiveV1 eventArchiveV1) =>
        TryCatch(async () =>
        {
            await ValidateEventArchiveV1OnAddAsync(eventArchiveV1);

            return await this.storageBroker.InsertEventArchiveV1Async(eventArchiveV1);
        });

        public ValueTask<IQueryable<EventArchiveV1>> RetrieveAllEventArchiveV1sAsync() =>
        TryCatch(async () =>
        {
            return await this.storageBroker.SelectAllEventArchiveV1sAsync();
        });

        public ValueTask<EventArchiveV1> RetrieveEventArchiveV1ByIdAsync(Guid eventArchiveV1Id) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV1Id(eventArchiveV1Id);

            return await this.storageBroker.SelectEventArchiveV1ByIdAsync(eventArchiveV1Id);
        });
    }
}
