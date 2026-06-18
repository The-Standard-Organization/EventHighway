// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V2
{
    internal partial class EventArchiveV2OrchestrationService : IEventArchiveV2OrchestrationService
    {
        private readonly IListenerEventArchiveV2Service listenerEventArchiveV2Service;
        private readonly IEventArchiveV2Service eventArchiveV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV2OrchestrationService(
            IListenerEventArchiveV2Service listenerEventArchiveV2Service,
            IEventArchiveV2Service eventArchiveV2Service,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventArchiveV2Service = listenerEventArchiveV2Service;
            this.eventArchiveV2Service = eventArchiveV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventArchiveV2Service.RetrieveAllEventArchiveV2sAsync());

        public ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>

            await this.listenerEventArchiveV2Service.RetrieveAllListenerEventArchiveV2sAsync());

        public ValueTask AddEventArchiveV2WithListenerEventArchiveV2sAsync(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2(eventArchiveV2);
            
            await this.eventArchiveV2Service.AddEventArchiveV2Async(eventArchiveV2, cancellationToken);

            foreach (ListenerEventArchiveV2 listenerEventArchiveV2 in eventArchiveV2.ListenerEventArchiveV2s)
            {
                await this.listenerEventArchiveV2Service
                    .AddListenerEventArchiveV2Async(listenerEventArchiveV2, cancellationToken);
            }
        });
    }
}
