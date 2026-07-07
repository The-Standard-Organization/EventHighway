// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventListeners.V2
{
    internal partial class EventListenerV2Service
    {
        private delegate ValueTask<EventListenerV2> ReturningEventListenerV2Function();
        private delegate ValueTask<IQueryable<EventListenerV2>> ReturningEventListenerV2sFunction();

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

                var timeoutEventListenerV2Exception =
                    new TimeoutEventListenerV2Exception(
                        message: "Failed event listener timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventListenerV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventListenerV2Exception =
                    new FailedStorageEventListenerV2Exception(
                        message: "Failed event listener storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventListenerV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventListenerV2ServiceException =
                    new FailedEventListenerV2ServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV2ServiceException);
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
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventListenerV2Exception =
                    new TimeoutEventListenerV2Exception(
                        message: "Failed event listener timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventListenerV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidEventListenerV2Exception invalidEventListenerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV2Exception);
            }
            catch (NullEventListenerV2Exception nullEventListenerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventListenerV2Exception);
            }
            catch (NotFoundEventListenerV2Exception notFoundEventListenerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundEventListenerV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventListenerV2Exception =
                    new FailedStorageEventListenerV2Exception(
                        message: "Failed event listener storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventListenerV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventListenerV2Exception =
                    new AlreadyExistsEventListenerV2Exception(
                        message: "Event listener with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventListenerV2Exception);
            }
            catch (ForeignKeyConstraintConflictException
                foreignKeyConstraintConflictException)
            {
                var invalidReferenceEventListenerV2Exception =
                    new InvalidReferenceEventListenerV2Exception(
                        message: "Invalid event listener reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidReferenceEventListenerV2Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedEventListenerV2Exception =
                    new LockedEventListenerV2Exception(
                        message: "Event listener is locked, try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedEventListenerV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventListenerV2Exception =
                    new FailedStorageEventListenerV2Exception(
                        message: "Failed event listener storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(failedStorageEventListenerV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventListenerV2ServiceException =
                    new FailedEventListenerV2ServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV2ServiceException);
            }
        }

        private async ValueTask<EventListenerV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2ValidationException =
                new EventListenerV2ValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2ValidationException);

            return eventListenerV2ValidationException;
        }

        private async ValueTask<EventListenerV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventListenerV2DependencyValidationException =
                new EventListenerV2DependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2DependencyValidationException);

            return eventListenerV2DependencyValidationException;
        }

        private async ValueTask<EventListenerV2DependencyException> CreateAndLogTimeoutDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2DependencyException =
                new EventListenerV2DependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2DependencyException);

            return eventListenerV2DependencyException;
        }

        private async ValueTask<EventListenerV2DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2DependencyException =
                new EventListenerV2DependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2DependencyException);

            return eventListenerV2DependencyException;
        }

        private async ValueTask<EventListenerV2DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2DependencyException =
                new EventListenerV2DependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventListenerV2DependencyException);

            return eventListenerV2DependencyException;
        }

        private async ValueTask<EventListenerV2ServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventListenerV2ServiceException =
                new EventListenerV2ServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV2ServiceException);

            return eventListenerV2ServiceException;
        }
    }
}
