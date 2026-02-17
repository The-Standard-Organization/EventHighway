// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Services.Processings.Events.V1;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace EventHighway.Core.Services.Orchestrations.Events.V1
{
    internal partial class EventV1OrchestrationServiceV1 : IEventV1OrchestrationServiceV1
    {
        private readonly IEventV1ProcessingService eventV1ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public EventV1OrchestrationServiceV1(
            IEventV1ProcessingService eventV1ProcessingService, 
            ILoggingBroker loggingBroker)
        {
            this.eventV1ProcessingService = eventV1ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventV1>> RetrieveAllDeadEventV1sWithListenersAsync() =>
        TryCatch(async () => await this.eventV1ProcessingService.RetrieveAllDeadEventV1sWithListenersAsync());
    }
}
