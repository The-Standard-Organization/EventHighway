// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.EventListeners.V2
{
    internal partial class EventListenerV2OrchestrationService
    {
        private delegate ValueTask<EventListenerV2> ReturningEventListenerV2Function();
        private delegate ValueTask<IQueryable<EventListenerV2>> ReturningEventListenerV2sFunction();
        private delegate ValueTask<IReadOnlyList<EventListenerV2>> ReturningEventListenerV2ListFunction();

        private async ValueTask<IReadOnlyList<EventListenerV2>> TryCatch(
            ReturningEventListenerV2ListFunction returningEventListenerV2ListFunction)
        {
            try
            {
                return await returningEventListenerV2ListFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutEventListenerV2OrchestrationException =
                    new TimeoutEventListenerV2OrchestrationException(
                        message: "Failed event listener orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventListenerV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventListenerV2ProcessingValidationException
                eventListenerV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingValidationException);
            }
            catch (EventListenerV2ProcessingDependencyValidationException
                eventListenerV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingDependencyValidationException);
            }
            catch (EventListenerV2ProcessingDependencyException
                eventListenerV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingDependencyException);
            }
            catch (EventListenerV2ProcessingServiceException
                eventListenerV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV2OrchestrationServiceException =
                    new FailedEventListenerV2OrchestrationServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV2OrchestrationServiceException);
            }
        }

        private async ValueTask<EventListenerV2> TryCatch(
            ReturningEventListenerV2Function returningEventListenerV2Function)
        {
            try
            {
                return await returningEventListenerV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutEventListenerV2OrchestrationException =
                    new TimeoutEventListenerV2OrchestrationException(
                        message: "Failed event listener orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventListenerV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidEventListenerV2OrchestrationException
                invalidEventListenerV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV2OrchestrationException);
            }
            catch (NullEventListenerV2OrchestrationException
                nullEventListenerV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventListenerV2OrchestrationException);
            }
            catch (EventListenerV2ProcessingValidationException
                eventListenerV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingValidationException);
            }
            catch (EventListenerV2ProcessingDependencyValidationException
                eventListenerV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingDependencyValidationException);
            }
            catch (EventListenerV2ProcessingDependencyException
                eventListenerV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingDependencyException);
            }
            catch (EventListenerV2ProcessingServiceException
                eventListenerV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV2OrchestrationServiceException =
                    new FailedEventListenerV2OrchestrationServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV2OrchestrationServiceException);
            }
        }

        private async ValueTask<IQueryable<EventListenerV2>> TryCatch(
            ReturningEventListenerV2sFunction returningEventListenerV2sFunction)
        {
            try
            {
                return await returningEventListenerV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutEventListenerV2OrchestrationException =
                    new TimeoutEventListenerV2OrchestrationException(
                        message: "Failed event listener orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventListenerV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidEventListenerV2OrchestrationException
                invalidEventListenerV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV2OrchestrationException);
            }
            catch (EventListenerV2ProcessingValidationException
                eventListenerV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingValidationException);
            }
            catch (EventListenerV2ProcessingDependencyValidationException
                eventListenerV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingDependencyValidationException);
            }
            catch (EventListenerV2ProcessingDependencyException
                eventListenerV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingDependencyException);
            }
            catch (EventListenerV2ProcessingServiceException
                eventListenerV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV2OrchestrationServiceException =
                    new FailedEventListenerV2OrchestrationServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV2OrchestrationServiceException);
            }
        }

        private async ValueTask<EventListenerV2OrchestrationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2OrchestrationValidationException =
                new EventListenerV2OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2OrchestrationValidationException);

            return eventListenerV2OrchestrationValidationException;
        }

        private async ValueTask<EventListenerV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventListenerV2OrchestrationDependencyValidationException =
                new EventListenerV2OrchestrationDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventListenerV2OrchestrationDependencyValidationException);

            return eventListenerV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<EventListenerV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                eventListenerV2OrchestrationDependencyException);

            return eventListenerV2OrchestrationDependencyException;
        }

        private async ValueTask<EventListenerV2OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventListenerV2OrchestrationDependencyException);

            return eventListenerV2OrchestrationDependencyException;
        }

        private async ValueTask<EventListenerV2OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2OrchestrationServiceException =
                new EventListenerV2OrchestrationServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2OrchestrationServiceException);

            return eventListenerV2OrchestrationServiceException;
        }
    }
}
