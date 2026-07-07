// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.HealthEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.HealthEvents.V2
{
    internal partial class HealthEventsV2OrchestrationService
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

                var timeoutHealthEventsV2OrchestrationException =
                    new TimeoutHealthEventsV2OrchestrationException(
                        message: "Health events orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutHealthEventsV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventV2ValidationException eventV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2ValidationException);
            }
            catch (EventV2DependencyValidationException eventV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2DependencyValidationException);
            }
            catch (ListenerEventV2ValidationException listenerEventV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(listenerEventV2ValidationException);
            }
            catch (ListenerEventV2DependencyValidationException listenerEventV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2DependencyValidationException);
            }
            catch (EventV2DependencyException eventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2DependencyException);
            }
            catch (EventV2ServiceException eventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ServiceException);
            }
            catch (ListenerEventV2DependencyException listenerEventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2DependencyException);
            }
            catch (ListenerEventV2ServiceException listenerEventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedHealthEventsV2OrchestrationServiceException =
                    new FailedHealthEventsV2OrchestrationServiceException(
                        message: "Failed health events service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedHealthEventsV2OrchestrationServiceException);
            }
        }

        private async ValueTask<HealthEventsV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var healthEventsV2OrchestrationDependencyException =
                new HealthEventsV2OrchestrationDependencyException(
                    message: "Health events dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                healthEventsV2OrchestrationDependencyException);

            return healthEventsV2OrchestrationDependencyException;
        }

        private async ValueTask<HealthEventsV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var healthEventsV2OrchestrationDependencyException =
                new HealthEventsV2OrchestrationDependencyException(
                    message: "Health events dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                healthEventsV2OrchestrationDependencyException);

            return healthEventsV2OrchestrationDependencyException;
        }

        private async ValueTask<HealthEventsV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var healthEventsV2OrchestrationDependencyValidationException =
                new HealthEventsV2OrchestrationDependencyValidationException(
                    message: "Health events validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                healthEventsV2OrchestrationDependencyValidationException);

            return healthEventsV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<HealthEventsV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var healthEventsV2OrchestrationServiceException =
                new HealthEventsV2OrchestrationServiceException(
                    message: "Health events service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                healthEventsV2OrchestrationServiceException);

            return healthEventsV2OrchestrationServiceException;
        }
    }
}
