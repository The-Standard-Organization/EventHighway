// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<IReadOnlyList<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            ListenerEventV2Query listenerEventV2Query) =>
            await this.eventHighwayClient.V2.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync(
                listenerEventV2Query);

        public async ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(Guid listenerEventV2Id) =>
            await this.eventHighwayClient.V2.ListenerEventV2Client.RemoveListenerEventV2ByIdAsync(listenerEventV2Id);
    }
}
