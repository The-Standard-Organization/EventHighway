// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventHandlers
{
    public partial class EventHandlersViewService : IEventHandlersViewService
    {
        private const int RetrievalPageSize = 1000;

        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventHandlersViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<EventHandlerView>> RetrieveAllEventHandlersAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            var eventHandlerV2Query = new EventHandlerV2Query { Take = RetrievalPageSize };
            var eventHandlerV2s = new List<EventHandlerV2>();

            while (true)
            {
                IReadOnlyList<EventHandlerV2> eventHandlerV2Page =
                    await this.eventHighwayBroker.RetrieveAllEventHandlerV2sAsync(
                        eventHandlerV2Query, cancellationToken);

                eventHandlerV2s.AddRange(eventHandlerV2Page);

                if (eventHandlerV2Page.Count < eventHandlerV2Query.Take)
                {
                    break;
                }

                eventHandlerV2Query.Skip += eventHandlerV2Query.Take;
            }

            return eventHandlerV2s.Select(AsView).ToList();
        });

        private static EventHandlerView AsView(EventHandlerV2 eventHandlerV2) =>
            new EventHandlerView
            {
                Id = eventHandlerV2.Id,
                Name = eventHandlerV2.Name
            };
    }
}
