// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<IReadOnlyList<EventHandlerV2>> RetrieveAllEventHandlerV2sAsync(
            EventHandlerV2Query eventHandlerV2Query) =>
            await this.eventHighwayClient.V2.EventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                eventHandlerV2Query);

        public async ValueTask<EventHandlerV2> RemoveEventHandlerV2ByIdAsync(Guid eventHandlerV2Id) =>
            await this.eventHighwayClient.V2.EventHandlerV2Client.RemoveEventHandlerV2ByIdAsync(
                eventHandlerV2Id);
    }
}
