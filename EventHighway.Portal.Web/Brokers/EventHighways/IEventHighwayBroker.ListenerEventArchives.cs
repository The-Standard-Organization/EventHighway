// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public partial interface IEventHighwayBroker
    {
        ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default);

        ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default);
    }
}
