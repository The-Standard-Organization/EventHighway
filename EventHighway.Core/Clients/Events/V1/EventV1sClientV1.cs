// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Services.Coordinations.Events.V1;

namespace EventHighway.Core.Clients.Events.V1
{
    internal class EventV1sClientV1 : IEventV1sClientV1
    {
        private readonly IEventV1CoordinationServiceV1 eventV1CoordinationServiceV1;

        public EventV1sClientV1(IEventV1CoordinationServiceV1 eventV1CoordinationServiceV1) =>
            this.eventV1CoordinationServiceV1 = eventV1CoordinationServiceV1;

        public ValueTask ArchiveDeadEventV1sAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
