// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.ListenerEvents.V2
{
    internal partial class ListenerEventV2Service
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<ListenerEventV2> ReturningListenerEventV2Function();
        private delegate ValueTask<IQueryable<ListenerEventV2>> ReturningListenerEventV2sFunction();
        private delegate ValueTask<IEnumerable<ListenerEventV2>> ReturningListenerEventV2EnumerableFunction();

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

                var timeoutListenerEventV2Exception =
                    new TimeoutListenerEventV2Exception(
                        message: "Failed listener event timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutListenerEventV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventV2Exception nullListenerEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventV2Exception =
                    new FailedStorageListenerEventV2Exception(
                        message: "Failed listener event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventV2ServiceException =
                    new FailedListenerEventV2ServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2ServiceException);
            }
        }

        private async ValueTask<ListenerEventV2> TryCatch(
            ReturningListenerEventV2Function returningListenerEventV2Function)
        {
            try
            {
                return await returningListenerEventV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventV2Exception =
                    new TimeoutListenerEventV2Exception(
                        message: "Failed listener event timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutListenerEventV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventV2Exception
                nullListenerEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV2Exception);
            }
            catch (InvalidListenerEventV2Exception
                invalidListenerEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV2Exception);
            }
            catch (NotFoundListenerEventV2Exception
                notFoundListenerEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundListenerEventV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventV2Exception =
                    new FailedStorageListenerEventV2Exception(
                        message: "Failed listener event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsListenerEventV2Exception =
                    new AlreadyExistsListenerEventV2Exception(
                        message: "Listener event with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsListenerEventV2Exception);
            }
            catch (ForeignKeyConstraintConflictException
                foreignKeyConstraintConflictException)
            {
                var invalidReferenceListenerEventV2Exception =
                    new InvalidReferenceListenerEventV2Exception(
                        message: "Invalid listener event reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidReferenceListenerEventV2Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedListenerEventV2Exception =
                    new LockedListenerEventV2Exception(
                        message: "Listener event is locked, try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    lockedListenerEventV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageListenerEventV2Exception =
                    new FailedStorageListenerEventV2Exception(
                        message: "Failed listener event storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedStorageListenerEventV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventV2ServiceException =
                    new FailedListenerEventV2ServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2ServiceException);
            }
        }

        private async ValueTask<IQueryable<ListenerEventV2>> TryCatch(
            ReturningListenerEventV2sFunction returningListenerEventV2sFunction)
        {
            try
            {
                return await returningListenerEventV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventV2Exception =
                    new TimeoutListenerEventV2Exception(
                        message: "Failed listener event timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutListenerEventV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventV2Exception nullListenerEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV2Exception);
            }
            catch (InvalidListenerEventV2Exception invalidListenerEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventV2Exception =
                    new FailedStorageListenerEventV2Exception(
                        message: "Failed listener event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventV2ServiceException =
                    new FailedListenerEventV2ServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV2ServiceException);
            }
        }

        private async ValueTask<IEnumerable<ListenerEventV2>> TryCatch(
            ReturningListenerEventV2EnumerableFunction returningListenerEventV2EnumerableFunction)
        {
            try
            {
                return await returningListenerEventV2EnumerableFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutListenerEventV2Exception =
                    new TimeoutListenerEventV2Exception(
                        message: "Failed listener event timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutListenerEventV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullListenerEventV2Exception nullListenerEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullListenerEventV2Exception);
            }
            catch (InvalidListenerEventV2Exception invalidListenerEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidListenerEventV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventV2Exception =
                    new FailedStorageListenerEventV2Exception(
                        message: "Failed listener event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageListenerEventV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageListenerEventV2Exception =
                    new FailedStorageListenerEventV2Exception(
                        message: "Failed listener event storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedStorageListenerEventV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventV2ServiceException =
                    new FailedListenerEventV2ServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedListenerEventV2ServiceException);
            }
        }

        private async ValueTask<ListenerEventV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var listenerEventV2ValidationException =
                new ListenerEventV2ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV2ValidationException);

            return listenerEventV2ValidationException;
        }

        private async ValueTask<ListenerEventV2DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV2DependencyException =
                new ListenerEventV2DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(listenerEventV2DependencyException);

            return listenerEventV2DependencyException;
        }

        private async ValueTask<ListenerEventV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var listenerEventV2DependencyValidationException =
                new ListenerEventV2DependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV2DependencyValidationException);

            return listenerEventV2DependencyValidationException;
        }

        private async ValueTask<ListenerEventV2DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV2DependencyException =
                new ListenerEventV2DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV2DependencyException);

            return listenerEventV2DependencyException;
        }

        private async ValueTask<ListenerEventV2ServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var listenerEventV2ServiceException =
                new ListenerEventV2ServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV2ServiceException);

            return listenerEventV2ServiceException;
        }
    }
}
