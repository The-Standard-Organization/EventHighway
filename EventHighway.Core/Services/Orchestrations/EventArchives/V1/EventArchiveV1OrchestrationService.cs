// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventArchiveV1OrchestrationService : IEventArchiveV1OrchestrationService
    {
        private readonly IListenerEventArchiveV1Service listenerEventArchiveV1Service;
        private readonly IEventArchiveV1Service eventArchiveV1Service;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV1OrchestrationService(
            IListenerEventArchiveV1Service listenerEventArchiveV1Service,
            IEventArchiveV1Service eventArchiveV1Service,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventArchiveV1Service = listenerEventArchiveV1Service;
            this.eventArchiveV1Service = eventArchiveV1Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask AddEventArchiveWithListenerEventArchivesAsync(EventArchiveV1 eventArchive) =>
        TryCatch(async () =>
        {
            ValidateEventArchive(eventArchive);

            foreach (ListenerEventArchiveV1 listenerEventArchive in eventArchive.ListenerEventArchives)
            {
                await this.listenerEventArchiveV1Service.AddListenerEventArchiveAsync(listenerEventArchive);
            }

            await this.eventArchiveV1Service.AddEventArchiveAsync(eventArchive);
        });
    }
}
