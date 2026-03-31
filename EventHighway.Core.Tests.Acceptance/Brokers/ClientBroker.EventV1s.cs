// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<EventV1> SubmitEventV1Async(EventV1 eventV1) =>
            await this.eventHighwayClient.EventV1s.SubmitEventV1Async(eventV1);

        public async ValueTask<EventV1> SubmitEventV1AsyncV1(EventV1 eventV1) =>
            await this.eventHighwayClient.EventV1s.SubmitEventV1AsyncV1(eventV1);

        public async ValueTask FireScheduledPendingEventV1sAsync() =>
            await this.eventHighwayClient.EventV1s.FireScheduledPendingEventV1sAsync();

        public async ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id) =>
            await this.eventHighwayClient.EventV1s.RemoveEventV1ByIdAsync(eventV1Id);
    }
}
