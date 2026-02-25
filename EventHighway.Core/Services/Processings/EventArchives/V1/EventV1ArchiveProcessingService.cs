// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Services.Foundations.EventArchives.V1;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    internal partial class EventV1ArchiveProcessingService : IEventV1ArchiveProcessingService
    {
        private readonly IEventV1ArchiveService eventV1ArchiveService;
        private readonly ILoggingBroker loggingBroker;

        public EventV1ArchiveProcessingService(
            IEventV1ArchiveService eventV1ArchiveService,
            ILoggingBroker loggingBroker)
        {
            this.eventV1ArchiveService = eventV1ArchiveService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV1Archive> AddEventV1ArchiveAsync(EventV1Archive eventV1Archive) =>
        TryCatch(async () =>
        {
            ValidateEventV1ArchiveIsNotNull(eventV1Archive);

            return await this.eventV1ArchiveService.AddEventV1ArchiveAsync(eventV1Archive);
        });
    }
}
