// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventListeners.V2
{
    internal partial class EventListenerV2ProcessingService
    {
        private delegate ValueTask<EventListenerV2> ReturningEventListenerV2Function();
        private delegate ValueTask<IQueryable<EventListenerV2>> ReturningEventListenerV2sFunction();

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
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventListenerV2ProcessingException =
                    new TimeoutEventListenerV2ProcessingException(
                        message: "Failed event listener processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventListenerV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventListenerV2ProcessingException
                nullEventListenerV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventListenerV2ProcessingException);
            }
            catch (InvalidEventListenerV2ProcessingException
                invalidEventListenerV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV2ProcessingException);
            }
            catch (EventListenerV2ValidationException
                eventListenerV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ValidationException);
            }
            catch (EventListenerV2DependencyValidationException
                eventListenerV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2DependencyValidationException);
            }
            catch (EventListenerV2DependencyException
                eventListenerV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2DependencyException);
            }
            catch (EventListenerV2ServiceException
                eventListenerV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV2ProcessingServiceException =
                    new FailedEventListenerV2ProcessingServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV2ProcessingServiceException);
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
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventListenerV2ProcessingException =
                    new TimeoutEventListenerV2ProcessingException(
                        message: "Failed event listener processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventListenerV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidEventListenerV2ProcessingException
                invalidEventListenerV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV2ProcessingException);
            }
            catch (EventListenerV2DependencyException
                eventListenerV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2DependencyException);
            }
            catch (EventListenerV2ServiceException
                eventListenerV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV2ProcessingServiceException =
                    new FailedEventListenerV2ProcessingServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventListenerV2ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2ProcessingValidationException =
                new EventListenerV2ProcessingValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2ProcessingValidationException);

            return eventListenerV2ProcessingValidationException;
        }

        private async ValueTask<EventListenerV2ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventListenerV2ProcessingDependencyValidationException =
                new EventListenerV2ProcessingDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventListenerV2ProcessingDependencyValidationException);

            return eventListenerV2ProcessingDependencyValidationException;
        }

        private async ValueTask<EventListenerV2ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2ProcessingDependencyException =
                new EventListenerV2ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventListenerV2ProcessingDependencyException);

            return eventListenerV2ProcessingDependencyException;
        }

        private async ValueTask<EventListenerV2ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2ProcessingServiceException =
                new EventListenerV2ProcessingServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2ProcessingServiceException);

            return eventListenerV2ProcessingServiceException;
        }

        private async ValueTask<EventListenerV2ProcessingDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventListenerV2ProcessingDependencyException =
                new EventListenerV2ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2ProcessingDependencyException);

            return eventListenerV2ProcessingDependencyException;
        }

    }
}
