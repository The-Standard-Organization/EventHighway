// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventHandlers.V2
{
    internal partial class EventHandlerV2ProcessingService
    {
        private delegate ValueTask<IEventHandler> ReturningEventHandlerFunction();
        private delegate ValueTask<EventHandlerV2> ReturningEventHandlerV2Function();

        private async ValueTask<EventHandlerV2> TryCatch(
            ReturningEventHandlerV2Function returningEventHandlerV2Function)
        {
            try
            {
                return await returningEventHandlerV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventHandlerV2ProcessingException =
                    new TimeoutEventHandlerV2ProcessingException(
                        message: "Failed event handler processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventHandlerV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidEventHandlerV2ProcessingException invalidEventHandlerV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventHandlerV2ProcessingException);
            }
            catch (EventHandlerV2ValidationException eventHandlerV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventHandlerV2ValidationException);
            }
            catch (EventHandlerV2DependencyValidationException eventHandlerV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventHandlerV2DependencyValidationException);
            }
            catch (EventHandlerV2DependencyException eventHandlerV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventHandlerV2DependencyException);
            }
            catch (EventHandlerV2ServiceException eventHandlerV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventHandlerV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventHandlerV2ProcessingServiceException =
                    new FailedEventHandlerV2ProcessingServiceException(
                        message: "Failed event handler service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventHandlerV2ProcessingServiceException);
            }
        }

        private async ValueTask<IEventHandler> TryCatch(
            ReturningEventHandlerFunction returningEventHandlerFunction)
        {
            try
            {
                return await returningEventHandlerFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventHandlerV2ProcessingException =
                    new TimeoutEventHandlerV2ProcessingException(
                        message: "Failed event handler processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventHandlerV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventHandlerV2ProcessingException nullEventHandlerV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventHandlerV2ProcessingException);
            }
            catch (InvalidEventHandlerV2ProcessingException invalidEventHandlerV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventHandlerV2ProcessingException);
            }
            catch (EventHandlerV2ValidationException eventHandlerV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventHandlerV2ValidationException);
            }
            catch (EventHandlerV2DependencyValidationException eventHandlerV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventHandlerV2DependencyValidationException);
            }
            catch (EventHandlerV2DependencyException eventHandlerV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventHandlerV2DependencyException);
            }
            catch (EventHandlerV2ServiceException eventHandlerV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventHandlerV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventHandlerV2ProcessingServiceException =
                    new FailedEventHandlerV2ProcessingServiceException(
                        message: "Failed event handler service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventHandlerV2ProcessingServiceException);
            }
        }

        private async ValueTask<EventHandlerV2ProcessingValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventHandlerV2ProcessingValidationException =
                new EventHandlerV2ProcessingValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2ProcessingValidationException);

            return eventHandlerV2ProcessingValidationException;
        }

        private async ValueTask<EventHandlerV2ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventHandlerV2ProcessingDependencyValidationException =
                new EventHandlerV2ProcessingDependencyValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2ProcessingDependencyValidationException);

            return eventHandlerV2ProcessingDependencyValidationException;
        }

        private async ValueTask<EventHandlerV2ProcessingDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventHandlerV2ProcessingDependencyException =
                new EventHandlerV2ProcessingDependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2ProcessingDependencyException);

            return eventHandlerV2ProcessingDependencyException;
        }

        private async ValueTask<EventHandlerV2ProcessingDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventHandlerV2ProcessingDependencyException =
                new EventHandlerV2ProcessingDependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2ProcessingDependencyException);

            return eventHandlerV2ProcessingDependencyException;
        }

        private async ValueTask<EventHandlerV2ProcessingServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventHandlerV2ProcessingServiceException =
                new EventHandlerV2ProcessingServiceException(
                    message: "Event handler service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2ProcessingServiceException);

            return eventHandlerV2ProcessingServiceException;
        }
    }
}
