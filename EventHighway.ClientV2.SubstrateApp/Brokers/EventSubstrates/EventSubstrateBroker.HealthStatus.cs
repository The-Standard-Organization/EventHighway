// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        public async ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusAsync(
            CancellationToken cancellationToken = default) =>
            await this.eventHighwayClient.V2.HealthClientV2.HealthStatusClientV2
                .RetrieveHealthRagStatusV2Async(
                    TrafficPeriodV2.Day, DateTimeOffset.UtcNow, cancellationToken);
    }
}
