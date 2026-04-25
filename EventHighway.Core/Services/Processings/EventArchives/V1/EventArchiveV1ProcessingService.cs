// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Services.Foundations.EventArchives.V1;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    internal partial class EventArchiveV1ProcessingService : IEventArchiveV1ProcessingService
    {
        private readonly IEventArchiveV1Service eventArchiveV1Service;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV1ProcessingService(
            IEventArchiveV1Service eventArchiveV1Service,
            ILoggingBroker loggingBroker)
        {
            this.eventArchiveV1Service = eventArchiveV1Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventArchiveV1> AddEventArchiveAsync(EventArchiveV1 eventArchive) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveIsNotNull(eventArchive);

            return await this.eventArchiveV1Service.AddEventArchiveAsync(eventArchive);
        });
    }
}
