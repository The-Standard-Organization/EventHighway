// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventCalls.V2
{
    internal partial class EventCallV2Service
    {
        private delegate ValueTask<EventCallV2> ReturningEventCallV2Function();

        private async ValueTask<EventCallV2> TryCatch(ReturningEventCallV2Function returningEventCallV2Function)
        {
            try
            {
                return await returningEventCallV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventCallV2Exception =
                    new TimeoutEventCallV2Exception(
                        message: "Failed event call timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventCallV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventCallV2Exception nullEventCallV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventCallV2Exception);
            }
            catch (InvalidEventCallV2Exception invalidEventCallV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventCallV2Exception);
            }
            catch (HandlerNotFoundEventCallV2Exception handlerNotFoundEventCallV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(handlerNotFoundEventCallV2Exception);
            }
            catch (Exception exception)
                when (exception is IEventHandlerValidationException)
            {
                var failedEventCallV2DependencyValidationException =
                    new FailedEventCallV2DependencyValidationException(
                        message: "Failed event call dependency validation error occurred, " +
                            "fix the errors and try again.",

                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    failedEventCallV2DependencyValidationException);
            }
            catch (Exception exception)
                when (exception is IEventHandlerDependencyException)
            {
                var failedEventCallV2DependencyException =
                    new FailedEventCallV2DependencyException(
                        message: "Failed event call dependency error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventCallV2DependencyException);
            }
            catch (Exception exception)
                when (exception is IEventHandlerServiceException)
            {
                var failedEventCallV2DependencyException =
                    new FailedEventCallV2DependencyException(
                        message: "Failed event call dependency error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedEventCallV2DependencyException);
            }
            catch (Exception serviceException)
            {
                var failedEventCallV2ServiceException =
                    new FailedEventCallV2ServiceException(
                        message: "Failed event call service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventCallV2ServiceException);
            }
        }

        private async ValueTask<EventCallV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2ValidationException);

            return eventCallV2ValidationException;
        }

        private async ValueTask<EventCallV2DependencyException> CreateAndLogTimeoutDependencyExceptionAsync(
            Xeption exception)
        {
            var eventCallV2DependencyException =
                new EventCallV2DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2DependencyException);

            return eventCallV2DependencyException;
        }

        private async ValueTask<EventCallV2DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventCallV2DependencyException =
                new EventCallV2DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2DependencyException);

            return eventCallV2DependencyException;
        }

        private async ValueTask<EventCallV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventCallV2DependencyValidationException =
                new EventCallV2DependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2DependencyValidationException);

            return eventCallV2DependencyValidationException;
        }

        private async ValueTask<EventCallV2DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventCallV2DependencyException =
                new EventCallV2DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventCallV2DependencyException);

            return eventCallV2DependencyException;
        }

        private async ValueTask<EventCallV2ServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventCallV2ServiceException =
                new EventCallV2ServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2ServiceException);

            return eventCallV2ServiceException;
        }
    }
}
