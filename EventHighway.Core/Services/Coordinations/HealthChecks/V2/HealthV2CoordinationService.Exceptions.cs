// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.HealthArchivedEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.HealthEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.HealthInfrastructures.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Coordinations.HealthChecks.V2
{
    internal partial class HealthV2CoordinationService
    {
        private delegate ValueTask<HealthReportV2> ReturningHealthReportV2Function();

        private async ValueTask<HealthReportV2> TryCatch(
            ReturningHealthReportV2Function returningHealthReportV2Function)
        {
            try
            {
                return await returningHealthReportV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutHealthV2CoordinationException =
                    new TimeoutHealthV2CoordinationException(
                        message: "Health coordination timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutHealthV2CoordinationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidHealthV2CoordinationException invalidHealthV2CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidHealthV2CoordinationException);
            }
            catch (HealthInfrastructureV2OrchestrationDependencyValidationException
                healthInfrastructureV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    healthInfrastructureV2OrchestrationDependencyValidationException);
            }
            catch (HealthEventsV2OrchestrationDependencyValidationException
                healthEventsV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    healthEventsV2OrchestrationDependencyValidationException);
            }
            catch (HealthArchivedEventsV2OrchestrationDependencyValidationException
                healthArchivedEventsV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    healthArchivedEventsV2OrchestrationDependencyValidationException);
            }
            catch (HealthInfrastructureV2OrchestrationDependencyException
                healthInfrastructureV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    healthInfrastructureV2OrchestrationDependencyException);
            }
            catch (HealthInfrastructureV2OrchestrationServiceException
                healthInfrastructureV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    healthInfrastructureV2OrchestrationServiceException);
            }
            catch (HealthEventsV2OrchestrationDependencyException
                healthEventsV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    healthEventsV2OrchestrationDependencyException);
            }
            catch (HealthEventsV2OrchestrationServiceException
                healthEventsV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    healthEventsV2OrchestrationServiceException);
            }
            catch (HealthArchivedEventsV2OrchestrationDependencyException
                healthArchivedEventsV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    healthArchivedEventsV2OrchestrationDependencyException);
            }
            catch (HealthArchivedEventsV2OrchestrationServiceException
                healthArchivedEventsV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    healthArchivedEventsV2OrchestrationServiceException);
            }
            catch (Exception exception)
            {
                var failedHealthV2CoordinationServiceException =
                    new FailedHealthV2CoordinationServiceException(
                        message: "Failed health coordination service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedHealthV2CoordinationServiceException);
            }
        }

        private async ValueTask<HealthV2CoordinationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var healthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    message: "Health coordination dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(healthV2CoordinationDependencyException);

            return healthV2CoordinationDependencyException;
        }

        private async ValueTask<HealthV2CoordinationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var healthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    message: "Health coordination dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(healthV2CoordinationDependencyException);

            return healthV2CoordinationDependencyException;
        }

        private async ValueTask<HealthV2CoordinationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var healthV2CoordinationDependencyValidationException =
                new HealthV2CoordinationDependencyValidationException(
                    message: "Health coordination validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(healthV2CoordinationDependencyValidationException);

            return healthV2CoordinationDependencyValidationException;
        }

        private async ValueTask<HealthV2CoordinationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var healthV2CoordinationValidationException =
                new HealthV2CoordinationValidationException(
                    message: "Health coordination validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(healthV2CoordinationValidationException);

            return healthV2CoordinationValidationException;
        }

        private async ValueTask<HealthV2CoordinationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var healthV2CoordinationServiceException =
                new HealthV2CoordinationServiceException(
                    message: "Health coordination service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(healthV2CoordinationServiceException);

            return healthV2CoordinationServiceException;
        }
    }
}
