// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;

namespace EventHighway.Core.Services.Processings.EventAddresses.V2
{
    internal interface IEventAddressV2ProcessingService
    {
        ValueTask<IReadOnlyList<EventAddressV2>> RetrieveEventAddressV2sByQueryAsync(
            EventAddressV2Query eventAddressV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventAddressV2> RetrieveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventAddressV2> RetrieveOrRegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default);

        ValueTask<EventAddressV2> RegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default);

        ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default);
    }
}
