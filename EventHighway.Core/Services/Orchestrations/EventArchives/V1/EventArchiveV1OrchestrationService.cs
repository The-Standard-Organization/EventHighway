// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
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

        public ValueTask AddEventArchiveV1WithListenerEventArchiveV1sAsync(EventArchiveV1 eventArchiveV1) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV1(eventArchiveV1);

            foreach (ListenerEventArchiveV1 listenerEventArchiveV1 in eventArchiveV1.ListenerEventArchiveV1s)
            {
                await this.listenerEventArchiveV1Service.AddListenerEventArchiveV1Async(listenerEventArchiveV1);
            }

            await this.eventArchiveV1Service.AddEventArchiveV1Async(eventArchiveV1);
        });
    }
}
