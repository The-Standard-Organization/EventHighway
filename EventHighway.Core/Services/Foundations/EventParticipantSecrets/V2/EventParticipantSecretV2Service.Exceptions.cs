// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2
{
    internal partial class EventParticipantSecretV2Service
    {
        private delegate ValueTask<EventParticipantSecretV2> ReturningEventParticipantSecretV2Function();
        private delegate ValueTask<IQueryable<EventParticipantSecretV2>> ReturningEventParticipantSecretV2sFunction();
        private delegate ValueTask<IReadOnlyList<EventParticipantSecretV2>> ReturningEventParticipantSecretV2ListFunction();

        private async ValueTask<IReadOnlyList<EventParticipantSecretV2>> TryCatch(
            ReturningEventParticipantSecretV2ListFunction returningEventParticipantSecretV2ListFunction)
        {
            try
            {
                return await returningEventParticipantSecretV2ListFunction();
            }
            catch (NullEventParticipantSecretV2QueryException
                nullEventParticipantSecretV2QueryException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventParticipantSecretV2QueryException);
            }
            catch (InvalidEventParticipantSecretV2QueryException
                invalidEventParticipantSecretV2QueryException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventParticipantSecretV2QueryException);
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventParticipantSecretV2Exception =
                    new TimeoutEventParticipantSecretV2Exception(
                        message: "Failed event participant secret timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventParticipantSecretV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventParticipantSecretV2Exception =
                    new FailedStorageEventParticipantSecretV2Exception(
                        message: "Failed event participant secret storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventParticipantSecretV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventParticipantSecretV2ServiceException =
                    new FailedEventParticipantSecretV2ServiceException(
                        message: "Failed event participant secret service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventParticipantSecretV2ServiceException);
            }
        }

        private async ValueTask<EventParticipantSecretV2> TryCatch(
            ReturningEventParticipantSecretV2Function returningEventParticipantSecretV2Function)
        {
            try
            {
                return await returningEventParticipantSecretV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventParticipantSecretV2Exception =
                    new TimeoutEventParticipantSecretV2Exception(
                        message: "Failed event participant secret timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventParticipantSecretV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventParticipantSecretV2Exception nullEventParticipantSecretV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventParticipantSecretV2Exception);
            }
            catch (InvalidEventParticipantSecretV2Exception invalidEventParticipantSecretV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventParticipantSecretV2Exception);
            }
            catch (NotFoundEventParticipantSecretV2Exception notFoundEventParticipantSecretV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundEventParticipantSecretV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventParticipantSecretV2Exception =
                    new FailedStorageEventParticipantSecretV2Exception(
                        message: "Failed event participant secret storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventParticipantSecretV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventParticipantSecretV2Exception =
                    new AlreadyExistsEventParticipantSecretV2Exception(
                        message: "Event participant secret with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventParticipantSecretV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventParticipantSecretV2Exception =
                    new FailedStorageEventParticipantSecretV2Exception(
                        message: "Failed event participant secret storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedStorageEventParticipantSecretV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventParticipantSecretV2ServiceException =
                    new FailedEventParticipantSecretV2ServiceException(
                        message: "Failed event participant secret service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventParticipantSecretV2ServiceException);
            }
        }

        private async ValueTask<IQueryable<EventParticipantSecretV2>> TryCatch(
            ReturningEventParticipantSecretV2sFunction returningEventParticipantSecretV2sFunction)
        {
            try
            {
                return await returningEventParticipantSecretV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventParticipantSecretV2Exception =
                    new TimeoutEventParticipantSecretV2Exception(
                        message: "Failed event participant secret timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventParticipantSecretV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventParticipantSecretV2Exception =
                    new FailedStorageEventParticipantSecretV2Exception(
                        message: "Failed event participant secret storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventParticipantSecretV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventParticipantSecretV2ServiceException =
                    new FailedEventParticipantSecretV2ServiceException(
                        message: "Failed event participant secret service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventParticipantSecretV2ServiceException);
            }
        }

        private async ValueTask<EventParticipantSecretV2ServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventParticipantSecretV2ServiceException =
                new EventParticipantSecretV2ServiceException(
                    message: "Event participant secret service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantSecretV2ServiceException);

            return eventParticipantSecretV2ServiceException;
        }

        private async ValueTask<EventParticipantSecretV2DependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantSecretV2DependencyException =
                new EventParticipantSecretV2DependencyException(
                    message: "Event participant secret dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantSecretV2DependencyException);

            return eventParticipantSecretV2DependencyException;
        }

        private async ValueTask<EventParticipantSecretV2DependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantSecretV2DependencyException =
                new EventParticipantSecretV2DependencyException(
                    message: "Event participant secret dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantSecretV2DependencyException);

            return eventParticipantSecretV2DependencyException;
        }

        private async ValueTask<EventParticipantSecretV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantSecretV2DependencyValidationException =
                new EventParticipantSecretV2DependencyValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantSecretV2DependencyValidationException);

            return eventParticipantSecretV2DependencyValidationException;
        }

        private async ValueTask<EventParticipantSecretV2DependencyException>
            CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantSecretV2DependencyException =
                new EventParticipantSecretV2DependencyException(
                    message: "Event participant secret dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventParticipantSecretV2DependencyException);

            return eventParticipantSecretV2DependencyException;
        }

        private async ValueTask<EventParticipantSecretV2ValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantSecretV2ValidationException);

            return eventParticipantSecretV2ValidationException;
        }
    }
}
