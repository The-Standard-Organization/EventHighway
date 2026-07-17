// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventArchives.V2
{
    internal partial class EventArchiveV2Service
    {
        private delegate ValueTask<EventArchiveV2> ReturningEventArchiveV2Function();
        private delegate ValueTask<IQueryable<EventArchiveV2>> ReturningEventArchiveV2sFunction();
        private delegate ValueTask<IReadOnlyList<EventArchiveV2>> ReturningEventArchiveV2ListFunction();
        private delegate ValueTask<IEnumerable<EventArchiveV2>> ReturningEnumerableEventArchiveV2sFunction();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<EventArchiveV2> TryCatch(
            ReturningEventArchiveV2Function returningEventArchiveV2Function)
        {
            try
            {
                return await returningEventArchiveV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventArchiveV2Exception =
                    new TimeoutEventArchiveV2Exception(
                        message: "Failed event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventArchiveV2Exception nullEventArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV2Exception);
            }
            catch (InvalidEventArchiveV2Exception invalidEventArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventArchiveV2Exception);
            }
            catch (NotFoundEventArchiveV2Exception notFoundEventArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundEventArchiveV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventArchiveV2Exception =
                    new FailedStorageEventArchiveV2Exception(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventArchiveV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventArchiveV2Exception =
                    new AlreadyExistsEventArchiveV2Exception(
                        message: "Event archive with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventArchiveV2Exception);
            }
            catch (ForeignKeyConstraintConflictException
                foreignKeyConstraintConflictException)
            {
                var invalidReferenceEventArchiveV2Exception =
                    new InvalidReferenceEventArchiveV2Exception(
                        message: "Invalid event archive reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidReferenceEventArchiveV2Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedEventArchiveV2Exception =
                    new LockedEventArchiveV2Exception(
                        message: "Event archive is locked, try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedEventArchiveV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventArchiveV2Exception =
                    new FailedStorageEventArchiveV2Exception(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(failedStorageEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventArchiveV2ServiceException =
                    new FailedEventArchiveV2ServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2ServiceException);
            }
        }

        private async ValueTask<IQueryable<EventArchiveV2>> TryCatch(
            ReturningEventArchiveV2sFunction returningEventArchiveV2sFunction)
        {
            try
            {
                return await returningEventArchiveV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventArchiveV2Exception =
                    new TimeoutEventArchiveV2Exception(
                        message: "Failed event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventArchiveV2Exception =
                    new FailedStorageEventArchiveV2Exception(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventArchiveV2ServiceException =
                    new FailedEventArchiveV2ServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2ServiceException);
            }
        }

        private async ValueTask<IReadOnlyList<EventArchiveV2>> TryCatch(
            ReturningEventArchiveV2ListFunction returningEventArchiveV2ListFunction)
        {
            try
            {
                return await returningEventArchiveV2ListFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventArchiveV2Exception =
                    new TimeoutEventArchiveV2Exception(
                        message: "Failed event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventArchiveV2Exception =
                    new FailedStorageEventArchiveV2Exception(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventArchiveV2ServiceException =
                    new FailedEventArchiveV2ServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2ServiceException);
            }
        }

        private async ValueTask<IEnumerable<EventArchiveV2>> TryCatch(
            ReturningEnumerableEventArchiveV2sFunction returningEnumrableEventArchiveV2sFunction)
        {
            try
            {
                return await returningEnumrableEventArchiveV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventArchiveV2Exception =
                    new TimeoutEventArchiveV2Exception(
                        message: "Failed event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventArchiveV2Exception nullEventArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventArchiveV2Exception =
                    new FailedStorageEventArchiveV2Exception(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventArchiveV2ServiceException =
                    new FailedEventArchiveV2ServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2ServiceException);
            }
        }

        private async ValueTask TryCatch(
            ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventArchiveV2Exception =
                    new TimeoutEventArchiveV2Exception(
                        message: "Failed event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventArchiveV2Exception nullEventArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventArchiveV2Exception =
                    new FailedStorageEventArchiveV2Exception(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventArchiveV2ServiceException =
                    new FailedEventArchiveV2ServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2ServiceException);
            }
        }

        private async ValueTask<EventArchiveV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV2ValidationException =
                new EventArchiveV2ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2ValidationException);

            return eventArchiveV2ValidationException;
        }

        private async ValueTask<EventArchiveV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventArchiveV2DependencyValidationException =
                new EventArchiveV2DependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2DependencyValidationException);

            return eventArchiveV2DependencyValidationException;
        }

        private async ValueTask<EventArchiveV2DependencyException> CreateAndLogTimeoutDependencyExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2DependencyException);

            return eventArchiveV2DependencyException;
        }

        private async ValueTask<EventArchiveV2DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2DependencyException);

            return eventArchiveV2DependencyException;
        }

        private async ValueTask<EventArchiveV2DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventArchiveV2DependencyException);

            return eventArchiveV2DependencyException;
        }

        private async ValueTask<EventArchiveV2ServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV2ServiceException =
                new EventArchiveV2ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2ServiceException);

            return eventArchiveV2ServiceException;
        }
    }
}
