// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.HealthArchivedEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2
{
    internal partial class HealthArchivedEventsV2OrchestrationService
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

                var timeoutHealthArchivedEventsV2OrchestrationException =
                    new TimeoutHealthArchivedEventsV2OrchestrationException(
                        message: "Health archived events orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutHealthArchivedEventsV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventArchiveV2ValidationException eventArchiveV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventArchiveV2ValidationException);
            }
            catch (EventArchiveV2DependencyValidationException eventArchiveV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2DependencyValidationException);
            }
            catch (ListenerEventArchiveV2ValidationException listenerEventArchiveV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventArchiveV2ValidationException);
            }
            catch (ListenerEventArchiveV2DependencyValidationException
                listenerEventArchiveV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventArchiveV2DependencyValidationException);
            }
            catch (EventArchiveV2DependencyException eventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventArchiveV2DependencyException);
            }
            catch (EventArchiveV2ServiceException eventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventArchiveV2ServiceException);
            }
            catch (ListenerEventArchiveV2DependencyException listenerEventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventArchiveV2DependencyException);
            }
            catch (ListenerEventArchiveV2ServiceException listenerEventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedHealthArchivedEventsV2OrchestrationServiceException =
                    new FailedHealthArchivedEventsV2OrchestrationServiceException(
                        message: "Failed health archived events service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedHealthArchivedEventsV2OrchestrationServiceException);
            }
        }

        private async ValueTask<HealthArchivedEventsV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var healthArchivedEventsV2OrchestrationDependencyException =
                new HealthArchivedEventsV2OrchestrationDependencyException(
                    message: "Health archived events dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                healthArchivedEventsV2OrchestrationDependencyException);

            return healthArchivedEventsV2OrchestrationDependencyException;
        }

        private async ValueTask<HealthArchivedEventsV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var healthArchivedEventsV2OrchestrationDependencyException =
                new HealthArchivedEventsV2OrchestrationDependencyException(
                    message: "Health archived events dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                healthArchivedEventsV2OrchestrationDependencyException);

            return healthArchivedEventsV2OrchestrationDependencyException;
        }

        private async ValueTask<HealthArchivedEventsV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var healthArchivedEventsV2OrchestrationDependencyValidationException =
                new HealthArchivedEventsV2OrchestrationDependencyValidationException(
                    message: "Health archived events validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                healthArchivedEventsV2OrchestrationDependencyValidationException);

            return healthArchivedEventsV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<HealthArchivedEventsV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var healthArchivedEventsV2OrchestrationServiceException =
                new HealthArchivedEventsV2OrchestrationServiceException(
                    message: "Health archived events service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                healthArchivedEventsV2OrchestrationServiceException);

            return healthArchivedEventsV2OrchestrationServiceException;
        }
    }
}
