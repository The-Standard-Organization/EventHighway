// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<EventAddressV2> RegisterEventAddressV2Async(EventAddressV2 eventAddressV2) =>
            await this.eventHighwayClient.V2.EventAddressV2Client.RegisterEventAddressV2Async(eventAddressV2);

        public async ValueTask<IReadOnlyList<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            EventAddressV2Query eventAddressV2Query) =>
            await this.eventHighwayClient.V2.EventAddressV2Client.RetrieveAllEventAddressV2sAsync(
                eventAddressV2Query);

        public async ValueTask<EventAddressV2> RetrieveOrRegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2) =>
                await this.eventHighwayClient.V2.EventAddressV2Client
                    .RetrieveOrRegisterEventAddressV2Async(eventAddressV2);

        public async ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(Guid eventAddressV2Id) =>
            await this.eventHighwayClient.V2.EventAddressV2Client.RemoveEventAddressV2ByIdAsync(eventAddressV2Id);
    }
}
