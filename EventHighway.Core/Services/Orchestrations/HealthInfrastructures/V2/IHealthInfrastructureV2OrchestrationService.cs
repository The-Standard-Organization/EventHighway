// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2
{
    internal interface IHealthInfrastructureV2OrchestrationService
    {
        ValueTask<InfrastructureHealthV2> RetrieveInfrastructureHealthV2Async(
            CancellationToken cancellationToken = default);
    }
}
