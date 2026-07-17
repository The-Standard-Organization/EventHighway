// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V2
{
    internal partial class ListenerEventArchiveV2Service
    {
        private delegate ValueTask<IQueryable<ListenerEventArchiveV2>> ReturningListenerEventArchiveV2sFunction();
        private delegate ValueTask<IReadOnlyList<ListenerEventArchiveV2>> ReturningListenerEventArchiveV2ListFunction();
        private delegate ValueTask<ListenerEventArchiveV2> ReturningListenerEventArchiveV2Function();
        private delegate ValueTask<IEnumerable<ListenerEventArchiveV2>> ReturningEnumerableListenerEventArchiveV2sFunction();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<IReadOnlyList<ListenerEventArchiveV2>> TryCatch(
            ReturningListenerEventArchiveV2ListFunction returningListenerEventArchiveV2ListFunction)
        {
            try
            {
                return await returningListenerEventArchiveV2ListFunction();
            }
            catch (NullListenerEventArchiveV2QueryException nullListenerEventArchiveV2QueryException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventArchiveV2QueryException);
            }
            catch (InvalidListenerEventArchiveV2QueryException invalidListenerEventArchiveV2QueryException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventArchiveV2QueryException);
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventArchiveV2Exception =
                    new TimeoutListenerEventArchiveV2Exception(
                        message: "Failed listener event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutListenerEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventArchiveV2Exception =
                    new FailedStorageListenerEventArchiveV2Exception(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventArchiveV2ServiceException =
                    new FailedListenerEventArchiveV2ServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventArchiveV2ServiceException);
            }
        }

        private async ValueTask<IQueryable<ListenerEventArchiveV2>> TryCatch(
            ReturningListenerEventArchiveV2sFunction returningListenerEventArchiveV2sFunction)
        {
            try
            {
                return await returningListenerEventArchiveV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventArchiveV2Exception =
                    new TimeoutListenerEventArchiveV2Exception(
                        message: "Failed listener event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutListenerEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventArchiveV2Exception =
                    new FailedStorageListenerEventArchiveV2Exception(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventArchiveV2ServiceException =
                    new FailedListenerEventArchiveV2ServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventArchiveV2ServiceException);
            }
        }

        private async ValueTask<ListenerEventArchiveV2> TryCatch(
            ReturningListenerEventArchiveV2Function returningListenerEventArchiveV2Function)
        {
            try
            {
                return await returningListenerEventArchiveV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventArchiveV2Exception =
                    new TimeoutListenerEventArchiveV2Exception(
                        message: "Failed listener event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutListenerEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventArchiveV2Exception nullListenerEventArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventArchiveV2Exception);
            }
            catch (InvalidListenerEventArchiveV2Exception invalidListenerEventArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventArchiveV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventArchiveV2Exception =
                    new FailedStorageListenerEventArchiveV2Exception(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventArchiveV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsListenerEventArchiveV2Exception =
                    new AlreadyExistsListenerEventArchiveV2Exception(
                        message: "Listener event archive with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsListenerEventArchiveV2Exception);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidReferenceListenerEventArchiveV2Exception =
                    new InvalidReferenceListenerEventArchiveV2Exception(
                        message: "Invalid listener event archive reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidReferenceListenerEventArchiveV2Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedListenerEventArchiveV2Exception =
                    new LockedListenerEventArchiveV2Exception(
                        message: "Listener event archive is locked, try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    lockedListenerEventArchiveV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageListenerEventArchiveV2Exception =
                    new FailedStorageListenerEventArchiveV2Exception(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedStorageListenerEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventArchiveV2ServiceException =
                    new FailedListenerEventArchiveV2ServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventArchiveV2ServiceException);
            }
        }

        private async ValueTask<IEnumerable<ListenerEventArchiveV2>> TryCatch(
            ReturningEnumerableListenerEventArchiveV2sFunction returningEnumrableListenerEventArchiveV2sFunction)
        {
            try
            {
                return await returningEnumrableListenerEventArchiveV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventArchiveV2Exception =
                    new TimeoutListenerEventArchiveV2Exception(
                        message: "Failed listener event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutListenerEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventArchiveV2Exception nullListenerEventArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventArchiveV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventArchiveV2Exception =
                    new FailedStorageListenerEventArchiveV2Exception(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventArchiveV2ServiceException =
                    new FailedListenerEventArchiveV2ServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventArchiveV2ServiceException);
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
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventArchiveV2Exception =
                    new TimeoutListenerEventArchiveV2Exception(
                        message: "Failed listener event archive timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutListenerEventArchiveV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventArchiveV2Exception nullListenerEventArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventArchiveV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventArchiveV2Exception =
                    new FailedStorageListenerEventArchiveV2Exception(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventArchiveV2ServiceException =
                    new FailedListenerEventArchiveV2ServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventArchiveV2ServiceException);
            }
        }

        private async ValueTask<ListenerEventArchiveV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var listenerEventArchiveV2ValidationException =
                new ListenerEventArchiveV2ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV2ValidationException);

            return listenerEventArchiveV2ValidationException;
        }

        private async ValueTask<ListenerEventArchiveV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var listenerEventArchiveV2DependencyValidationException =
                new ListenerEventArchiveV2DependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV2DependencyValidationException);

            return listenerEventArchiveV2DependencyValidationException;
        }

        private async ValueTask<ListenerEventArchiveV2DependencyException> CreateAndLogTimeoutDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventArchiveV2DependencyException =
                new ListenerEventArchiveV2DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV2DependencyException);

            return listenerEventArchiveV2DependencyException;
        }

        private async ValueTask<ListenerEventArchiveV2DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventArchiveV2DependencyException =
                new ListenerEventArchiveV2DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV2DependencyException);

            return listenerEventArchiveV2DependencyException;
        }

        private async ValueTask<ListenerEventArchiveV2DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventArchiveV2DependencyException =
                new ListenerEventArchiveV2DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(listenerEventArchiveV2DependencyException);

            return listenerEventArchiveV2DependencyException;
        }

        private async ValueTask<ListenerEventArchiveV2ServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var listenerEventArchiveV2ServiceException =
                new ListenerEventArchiveV2ServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV2ServiceException);

            return listenerEventArchiveV2ServiceException;
        }
    }
}