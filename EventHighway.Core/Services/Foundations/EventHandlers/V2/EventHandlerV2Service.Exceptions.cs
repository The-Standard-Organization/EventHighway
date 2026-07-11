// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal partial class EventHandlerV2Service
    {
        private delegate ValueTask<IEventHandler> ReturningEventHandlerFunction();
        private delegate ValueTask<IQueryable<IEventHandler>> ReturningQueryableEventHandlersFunction();
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

                var timeoutEventHandlerV2Exception =
                    new TimeoutEventHandlerV2Exception(
                        message: "Failed event handler timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventHandlerV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidEventHandlerV2Exception invalidEventHandlerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventHandlerV2Exception);
            }
            catch (NotFoundEventHandlerV2Exception notFoundEventHandlerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundEventHandlerV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventHandlerV2Exception =
                    new FailedStorageEventHandlerV2Exception(
                        message: "Failed event handler storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventHandlerV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventHandlerV2Exception =
                    new FailedStorageEventHandlerV2Exception(
                        message: "Failed event handler storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedStorageEventHandlerV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventHandlerV2ServiceException =
                    new FailedEventHandlerV2ServiceException(
                        message: "Failed event handler service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventHandlerV2ServiceException);
            }
        }

        private async ValueTask<IQueryable<IEventHandler>> TryCatch(
            ReturningQueryableEventHandlersFunction returningQueryableEventHandlersFunction)
        {
            try
            {
                return await returningQueryableEventHandlersFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventHandlerV2Exception =
                    new TimeoutEventHandlerV2Exception(
                        message: "Failed event handler timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventHandlerV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception serviceException)
            {
                var failedEventHandlerV2ServiceException =
                    new FailedEventHandlerV2ServiceException(
                        message: "Failed event handler service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventHandlerV2ServiceException);
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

                var timeoutEventHandlerV2Exception =
                    new TimeoutEventHandlerV2Exception(
                        message: "Failed event handler timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventHandlerV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventHandlerV2Exception nullEventHandlerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventHandlerV2Exception);
            }
            catch (InvalidEventHandlerV2Exception invalidEventHandlerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventHandlerV2Exception);
            }
            catch (NotFoundEventHandlerV2Exception notFoundEventHandlerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundEventHandlerV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventHandlerV2Exception =
                    new FailedStorageEventHandlerV2Exception(
                        message: "Failed event handler storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventHandlerV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventHandlerV2Exception =
                    new AlreadyExistsEventHandlerV2Exception(
                        message: "Event handler with the same id or name already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventHandlerV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventHandlerV2Exception =
                    new FailedStorageEventHandlerV2Exception(
                        message: "Failed event handler storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedStorageEventHandlerV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventHandlerV2ServiceException =
                    new FailedEventHandlerV2ServiceException(
                        message: "Failed event handler service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventHandlerV2ServiceException);
            }
        }

        private async ValueTask<EventHandlerV2ValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventHandlerV2ValidationException =
                new EventHandlerV2ValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2ValidationException);

            return eventHandlerV2ValidationException;
        }

        private async ValueTask<EventHandlerV2DependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventHandlerV2DependencyException =
                new EventHandlerV2DependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2DependencyException);

            return eventHandlerV2DependencyException;
        }

        private async ValueTask<EventHandlerV2DependencyException>
            CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var eventHandlerV2DependencyException =
                new EventHandlerV2DependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventHandlerV2DependencyException);

            return eventHandlerV2DependencyException;
        }

        private async ValueTask<EventHandlerV2DependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventHandlerV2DependencyException =
                new EventHandlerV2DependencyException(
                    message: "Event handler dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2DependencyException);

            return eventHandlerV2DependencyException;
        }

        private async ValueTask<EventHandlerV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventHandlerV2DependencyValidationException =
                new EventHandlerV2DependencyValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2DependencyValidationException);

            return eventHandlerV2DependencyValidationException;
        }

        private async ValueTask<EventHandlerV2ServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventHandlerV2ServiceException =
                new EventHandlerV2ServiceException(
                    message: "Event handler service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2ServiceException);

            return eventHandlerV2ServiceException;
        }

    }
}
