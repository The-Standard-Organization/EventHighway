// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventParticipants.V2
{
    internal partial class EventParticipantV2Service
    {
        private delegate ValueTask<EventParticipantV2> ReturningEventParticipantV2Function();
        private delegate ValueTask<IQueryable<EventParticipantV2>> ReturningEventParticipantV2sFunction();

        private async ValueTask<EventParticipantV2> TryCatch(
            ReturningEventParticipantV2Function returningEventParticipantV2Function)
        {
            try
            {
                return await returningEventParticipantV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventParticipantV2Exception =
                    new TimeoutEventParticipantV2Exception(
                        message: "Failed event participant timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventParticipantV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventParticipantV2Exception nullEventParticipantV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventParticipantV2Exception);
            }
            catch (InvalidEventParticipantV2Exception invalidEventParticipantV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventParticipantV2Exception);
            }
            catch (NotFoundEventParticipantV2Exception notFoundEventParticipantV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundEventParticipantV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventParticipantV2Exception =
                    new FailedStorageEventParticipantV2Exception(
                        message: "Failed event participant storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventParticipantV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventParticipantV2Exception =
                    new AlreadyExistsEventParticipantV2Exception(
                        message: "Event participant with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventParticipantV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventParticipantV2Exception =
                    new FailedStorageEventParticipantV2Exception(
                        message: "Failed event participant storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedStorageEventParticipantV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventParticipantV2ServiceException =
                    new FailedEventParticipantV2ServiceException(
                        message: "Failed event participant service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventParticipantV2ServiceException);
            }
        }

        private async ValueTask<EventParticipantV2ServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventParticipantV2ServiceException =
                new EventParticipantV2ServiceException(
                    message: "Event participant service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2ServiceException);

            return eventParticipantV2ServiceException;
        }

        private async ValueTask<EventParticipantV2DependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantV2DependencyException =
                new EventParticipantV2DependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2DependencyException);

            return eventParticipantV2DependencyException;
        }

        private async ValueTask<EventParticipantV2DependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantV2DependencyException =
                new EventParticipantV2DependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2DependencyException);

            return eventParticipantV2DependencyException;
        }

        private async ValueTask<EventParticipantV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantV2DependencyValidationException =
                new EventParticipantV2DependencyValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2DependencyValidationException);

            return eventParticipantV2DependencyValidationException;
        }

        private async ValueTask<IQueryable<EventParticipantV2>> TryCatch(
            ReturningEventParticipantV2sFunction returningEventParticipantV2sFunction)
        {
            try
            {
                return await returningEventParticipantV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventParticipantV2Exception =
                    new TimeoutEventParticipantV2Exception(
                        message: "Failed event participant timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventParticipantV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventParticipantV2Exception =
                    new FailedStorageEventParticipantV2Exception(
                        message: "Failed event participant storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventParticipantV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventParticipantV2ServiceException =
                    new FailedEventParticipantV2ServiceException(
                        message: "Failed event participant service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventParticipantV2ServiceException);
            }
        }

        private async ValueTask<EventParticipantV2DependencyException>
            CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantV2DependencyException =
                new EventParticipantV2DependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventParticipantV2DependencyException);

            return eventParticipantV2DependencyException;
        }

        private async ValueTask<EventParticipantV2ValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2ValidationException);

            return eventParticipantV2ValidationException;
        }
    }
}
