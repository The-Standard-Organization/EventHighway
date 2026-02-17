// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Services.Processings.Events.V1;
using EventHighway.Core.Services.Processings.ListenerEvents.V1;

namespace EventHighway.Core.Services.Orchestrations.Events.V1
{
    internal partial class EventV1OrchestrationServiceV1 : IEventV1OrchestrationServiceV1
    {
        private readonly IEventV1ProcessingService eventV1ProcessingService;
        private readonly IListenerEventV1ProcessingService listenerEventV1ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public EventV1OrchestrationServiceV1(
            IEventV1ProcessingService eventV1ProcessingService,
            IListenerEventV1ProcessingService listenerEventV1ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.eventV1ProcessingService = eventV1ProcessingService;
            this.listenerEventV1ProcessingService = listenerEventV1ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask RemoveEventV1AndListenerEventV1sAsync(EventV1 eventV1)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<IQueryable<EventV1>> RetrieveAllDeadEventV1sWithListenersAsync() =>
        TryCatch(async () => await this.eventV1ProcessingService.RetrieveAllDeadEventV1sWithListenersAsync());
    }
}
