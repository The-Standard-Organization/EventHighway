// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask ArchiveDeadEventV2sAsync() =>
            await this.eventHighwayClient.V2.ArchivingEventV2Client.ArchiveEventV2sAsync();

        public async ValueTask PurgeEventArchiveV2sAsync(DateTimeOffset olderThan) =>
            await this.eventHighwayClient.V2.ArchivingEventV2Client
                .PurgeEventArchiveV2sAsync(olderThan);

        public async ValueTask PurgeEventArchiveV2sAsync() =>
            await this.eventHighwayClient.V2.ArchivingEventV2Client
                .PurgeEventArchiveV2sAsync();
    }
}
