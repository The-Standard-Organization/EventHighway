// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health check client implementation, handling health check retrieval
    /// operations while managing coordination service exceptions.
    /// </summary>
    internal class HealthStatusClientV2 : IHealthStatusClientV2
    {
        private readonly IHealthV2CoordinationService healthV2CoordinationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthStatusClientV2"/> class with the
        /// specified health coordination service.
        /// </summary>
        /// <param name="healthV2CoordinationService">The coordination service for health
        /// reports.</param>
        public HealthStatusClientV2(IHealthV2CoordinationService healthV2CoordinationService) =>
            this.healthV2CoordinationService = healthV2CoordinationService;

        public async ValueTask<IReadOnlyList<HealthCheckItemV2>> RetrieveHealthRagStatusV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            HealthReportV2 healthReport = await this.healthV2CoordinationService
                .RetrieveHealthCheckItemsReportV2Async(period, windowStart, cancellationToken);

            return healthReport.HealthCheckItems;
        }
    }
}
