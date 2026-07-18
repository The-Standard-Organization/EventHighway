// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<IReadOnlyList<ListenerEventArchiveV2>>
            RetrieveAllListenerEventArchiveV2sAsync(
                ListenerEventArchiveV2Query listenerEventArchiveV2Query) =>
            await this.eventHighwayClient.V2.ListenerEventArchiveV2Client
                .RetrieveAllListenerEventArchiveV2sAsync(listenerEventArchiveV2Query);

        public async ValueTask<IReadOnlyList<ListenerEventArchiveV2>>
            RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                ListenerEventArchiveV2Query listenerEventArchiveV2Query) =>
            await this.eventHighwayClient.V2.ListenerEventArchiveV2Client
                .RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                    listenerEventArchiveV2Query);
    }
}
