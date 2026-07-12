// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;

#pragma warning disable CS0618 // Testing deprecated V1 exposure surface
namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventV1s() =>
            await this.eventHighwayClient.ListenerEventV1s.RetrieveAllListenerEventV1sAsync();

        public async ValueTask<ListenerEventV1> RemoveListenerEventV1ByIdAsync(Guid listenerEventV1Id) =>
            await this.eventHighwayClient.ListenerEventV1s.RemoveListenerEventV1ByIdAsync(listenerEventV1Id);
    }
}
