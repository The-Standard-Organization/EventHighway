// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.RetryingListenerEvents.V2
{
    internal partial class RetryingListenerEventV2OrchestrationService
    {
        private delegate ValueTask<ListenerEventV2> ReturningListenerEventV2Function();

        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<ListenerEventV2> TryCatch(
            ReturningListenerEventV2Function returningListenerEventV2Function)
        {
            try
            {
                return await returningListenerEventV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutRetryingListenerEventV2OrchestrationException =
                    new TimeoutRetryingListenerEventV2OrchestrationException(
                        message: "Failed retrying listener event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var retryingListenerEventV2OrchestrationDependencyException =
                    new RetryingListenerEventV2OrchestrationDependencyException(
                        message: "Retrying listener event dependency error occurred, contact support.",
                        innerException: timeoutRetryingListenerEventV2OrchestrationException);

                await this.loggingBroker.LogErrorAsync(
                    retryingListenerEventV2OrchestrationDependencyException);

                throw retryingListenerEventV2OrchestrationDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullRetryingListenerEventV2OrchestrationException
                nullRetryingListenerEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullRetryingListenerEventV2OrchestrationException);
            }
            catch (EventCallV2ProcessingValidationException
                eventCallV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingValidationException);
            }
            catch (EventCallV2ProcessingDependencyValidationException
                eventCallV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingDependencyValidationException);
            }
            catch (ListenerEventV2ProcessingValidationException
                listenerEventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingValidationException);
            }
            catch (ListenerEventV2ProcessingDependencyValidationException
                listenerEventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingDependencyValidationException);
            }
            catch (EventCallV2ProcessingDependencyException
                eventCallV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventCallV2ProcessingDependencyException);
            }
            catch (EventCallV2ProcessingServiceException
                eventCallV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventCallV2ProcessingServiceException);
            }
            catch (ListenerEventV2ProcessingDependencyException
                listenerEventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingDependencyException);
            }
            catch (ListenerEventV2ProcessingServiceException
                listenerEventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedRetryingListenerEventV2OrchestrationServiceException =
                    new FailedRetryingListenerEventV2OrchestrationServiceException(
                        message: "Failed retrying listener event orchestration service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedRetryingListenerEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (EventCallV2ProcessingValidationException
                eventCallV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingValidationException);
            }
            catch (EventCallV2ProcessingDependencyValidationException
                eventCallV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingDependencyValidationException);
            }
            catch (ListenerEventV2ProcessingValidationException
                listenerEventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingValidationException);
            }
            catch (ListenerEventV2ProcessingDependencyValidationException
                listenerEventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingDependencyValidationException);
            }
            catch (Exception exception)
            {
                var failedRetryingListenerEventV2OrchestrationServiceException =
                    new FailedRetryingListenerEventV2OrchestrationServiceException(
                        message: "Failed retrying listener event orchestration service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedRetryingListenerEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<RetryingListenerEventV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var retryingListenerEventV2OrchestrationValidationException =
                new RetryingListenerEventV2OrchestrationValidationException(
                    message: "Retrying listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                retryingListenerEventV2OrchestrationValidationException);

            return retryingListenerEventV2OrchestrationValidationException;
        }

        private async ValueTask<RetryingListenerEventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var retryingListenerEventV2OrchestrationDependencyValidationException =
                new RetryingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Retrying listener event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                retryingListenerEventV2OrchestrationDependencyValidationException);

            return retryingListenerEventV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<RetryingListenerEventV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var retryingListenerEventV2OrchestrationDependencyException =
                new RetryingListenerEventV2OrchestrationDependencyException(
                    message: "Retrying listener event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                retryingListenerEventV2OrchestrationDependencyException);

            return retryingListenerEventV2OrchestrationDependencyException;
        }

        private async ValueTask<RetryingListenerEventV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var retryingListenerEventV2OrchestrationServiceException =
                new RetryingListenerEventV2OrchestrationServiceException(
                    message: "Retrying listener event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                retryingListenerEventV2OrchestrationServiceException);

            return retryingListenerEventV2OrchestrationServiceException;
        }
    }
}
