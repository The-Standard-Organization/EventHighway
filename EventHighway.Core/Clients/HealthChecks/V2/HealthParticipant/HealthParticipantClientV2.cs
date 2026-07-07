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
    /// Represents the V2 health participant client implementation, handling per-participant
    /// summary retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthParticipantClientV2 : IHealthParticipantClientV2
    {
        private readonly IHealthV2CoordinationService healthV2CoordinationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthParticipantClientV2"/> class with the
        /// specified health coordination service.
        /// </summary>
        /// <param name="healthV2CoordinationService">The coordination service for health
        /// reports.</param>
        public HealthParticipantClientV2(IHealthV2CoordinationService healthV2CoordinationService) =>
            this.healthV2CoordinationService = healthV2CoordinationService;

        public ValueTask<IReadOnlyList<ParticipantUsageV2>> RetrieveParticipantSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
