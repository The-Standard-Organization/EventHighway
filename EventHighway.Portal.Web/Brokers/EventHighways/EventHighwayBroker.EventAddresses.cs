// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask<EventAddressV2> RegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventAddressV2Client
                    .RegisterEventAddressV2Async(eventAddressV2, cancellationToken),
                cancellationToken);

        public ValueTask<IReadOnlyList<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            EventAddressV2Query eventAddressV2Query,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                await client.EventAddressV2Client.RetrieveAllEventAddressV2sAsync(
                    eventAddressV2Query, cancellationToken),
                cancellationToken);

        public ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.EventAddressV2Client
                    .RemoveEventAddressV2ByIdAsync(eventAddressV2Id, cancellationToken),
                cancellationToken);
    }
}
