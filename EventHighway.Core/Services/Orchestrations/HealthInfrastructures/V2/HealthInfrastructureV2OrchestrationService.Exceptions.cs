// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.HealthInfrastructures.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2
{
    internal partial class HealthInfrastructureV2OrchestrationService
    {
        private delegate ValueTask<InfrastructureHealthV2> ReturningInfrastructureHealthV2Function();

        private async ValueTask<InfrastructureHealthV2> TryCatch(
            ReturningInfrastructureHealthV2Function returningInfrastructureHealthV2Function)
        {
            try
            {
                return await returningInfrastructureHealthV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutHealthInfrastructureV2OrchestrationException =
                    new TimeoutHealthInfrastructureV2OrchestrationException(
                        message: "Health infrastructure orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutHealthInfrastructureV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }

        private async ValueTask<HealthInfrastructureV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var healthInfrastructureV2OrchestrationDependencyException =
                new HealthInfrastructureV2OrchestrationDependencyException(
                    message: "Health infrastructure dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                healthInfrastructureV2OrchestrationDependencyException);

            return healthInfrastructureV2OrchestrationDependencyException;
        }
    }
}
