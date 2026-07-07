// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.ListenerEvents.V2
{
    internal partial class ListenerEventV2OrchestrationService
    {
        private delegate ValueTask<IQueryable<ListenerEventV2>> ReturningListenerEventV2sQueryableFunction();
        private delegate ValueTask<IEnumerable<ListenerEventV2>> ReturningListenerEventV2sFunction();
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
                    new TimeoutException(
                        "The dependency operation timed out.",
                        operationCanceledException);

                var timeoutListenerEventV2OrchestrationException =
                    new TimeoutListenerEventV2OrchestrationException(
                        message: "Failed listener event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutListenerEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidListenerEventV2OrchestrationException
                invalidListenerEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV2OrchestrationException);
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
                var failedListenerEventV2OrchestrationServiceException =
                    new FailedListenerEventV2OrchestrationServiceException(
                        message: "Failed listener event orchestration service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<IQueryable<ListenerEventV2>> TryCatch(
            ReturningListenerEventV2sQueryableFunction returningListenerEventV2sQueryableFunction)
        {
            try
            {
                return await returningListenerEventV2sQueryableFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException(
                        "The dependency operation timed out.",
                        operationCanceledException);

                var timeoutListenerEventV2OrchestrationException =
                    new TimeoutListenerEventV2OrchestrationException(
                        message: "Failed listener event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutListenerEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidListenerEventV2OrchestrationException
                invalidListenerEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV2OrchestrationException);
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
                var failedListenerEventV2OrchestrationServiceException =
                    new FailedListenerEventV2OrchestrationServiceException(
                        message: "Failed listener event orchestration service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<IEnumerable<ListenerEventV2>> TryCatch(
            ReturningListenerEventV2sFunction returningListenerEventV2sFunction)
        {
            try
            {
                return await returningListenerEventV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException(
                        "The dependency operation timed out.",
                        operationCanceledException);

                var timeoutListenerEventV2OrchestrationException =
                    new TimeoutListenerEventV2OrchestrationException(
                        message: "Failed listener event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutListenerEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidListenerEventV2OrchestrationException
                invalidListenerEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV2OrchestrationException);
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
                var failedListenerEventV2OrchestrationServiceException =
                    new FailedListenerEventV2OrchestrationServiceException(
                        message: "Failed listener event orchestration service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException(
                        "The dependency operation timed out.",
                        operationCanceledException);

                var timeoutListenerEventV2OrchestrationException =
                    new TimeoutListenerEventV2OrchestrationException(
                        message: "Failed listener event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutListenerEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidListenerEventV2OrchestrationException
                invalidListenerEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV2OrchestrationException);
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
                var failedListenerEventV2OrchestrationServiceException =
                    new FailedListenerEventV2OrchestrationServiceException(
                        message: "Failed listener event orchestration service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<ListenerEventV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var listenerEventV2OrchestrationValidationException =
                new ListenerEventV2OrchestrationValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                listenerEventV2OrchestrationValidationException);

            return listenerEventV2OrchestrationValidationException;
        }

        private async ValueTask<ListenerEventV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var listenerEventV2OrchestrationServiceException =
                new ListenerEventV2OrchestrationServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                listenerEventV2OrchestrationServiceException);

            return listenerEventV2OrchestrationServiceException;
        }

        private async ValueTask<ListenerEventV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var listenerEventV2OrchestrationDependencyException =
                new ListenerEventV2OrchestrationDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                listenerEventV2OrchestrationDependencyException);

            return listenerEventV2OrchestrationDependencyException;
        }

        private async ValueTask<ListenerEventV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var listenerEventV2OrchestrationDependencyException =
                new ListenerEventV2OrchestrationDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                listenerEventV2OrchestrationDependencyException);

            return listenerEventV2OrchestrationDependencyException;
        }

        private async ValueTask<ListenerEventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var listenerEventV2OrchestrationDependencyValidationException =
                new ListenerEventV2OrchestrationDependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                listenerEventV2OrchestrationDependencyValidationException);

            return listenerEventV2OrchestrationDependencyValidationException;
        }
    }
}
