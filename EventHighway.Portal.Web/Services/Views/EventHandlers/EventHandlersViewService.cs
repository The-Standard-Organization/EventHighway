// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.EventHandlers;

namespace EventHighway.Portal.Web.Services.Views.EventHandlers
{
    public partial class EventHandlersViewService : IEventHandlersViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventHandlersViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<List<EventHandlerView>> RetrieveAllEventHandlersAsync(
            CancellationToken cancellationToken = default)
        {
            IEnumerable<EventHandlerV2> eventHandlerV2s =
                await this.eventHighwayBroker.RetrieveAllEventHandlerV2sAsync(
                    cancellationToken);

            return eventHandlerV2s.Select(AsView).ToList();
        }

        private static EventHandlerView AsView(EventHandlerV2 eventHandlerV2) =>
            new EventHandlerView
            {
                Id = eventHandlerV2.Id,
                Name = eventHandlerV2.Name
            };
    }
}
