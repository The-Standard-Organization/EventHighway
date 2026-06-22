// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.Events.V2
{
    internal partial class EventV2Service
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<int> ReturningIntFunction();
        private delegate ValueTask<string> ReturningStringFunction();
        private delegate ValueTask<EventV2> ReturningEventV2Function();
        private delegate ValueTask<IQueryable<EventV2>> ReturningEventV2sFunction();

        private async ValueTask<int> TryCatch(ReturningIntFunction returningIntFunction)
        {
            try
            {
                return await returningIntFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventV2Exception =
                    new TimeoutEventV2Exception(
                        message: "Failed event timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutEventV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventV2Exception nullEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventV2Exception);
            }
            catch (InvalidEventV2Exception invalidEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventV2Exception =
                    new FailedStorageEventV2Exception(
                        message: "Failed event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogDependencyExceptionAsync(failedStorageEventV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventV2ServiceException =
                    new FailedEventV2ServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2ServiceException);
            }
        }

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventV2Exception =
                    new TimeoutEventV2Exception(
                        message: "Failed event timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutEventV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventV2Exception nullEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventV2Exception);
            }
            catch (InvalidEventV2Exception invalidEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventV2Exception);
            }
            catch (JsonException jsonException)
            {
                var failedJsonEventV2Exception =
                    new FailedJsonEventV2Exception(
                        message: "Failed json event error occurred, contact support.",
                        innerException: jsonException,
                        data: jsonException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(failedJsonEventV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventV2ServiceException =
                    new FailedEventV2ServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2ServiceException);
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

                var timeoutEventV2Exception =
                    new TimeoutEventV2Exception(
                        message: "Failed event timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutEventV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventV2Exception nullEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventV2Exception =
                    new FailedStorageEventV2Exception(
                        message: "Failed event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageEventV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventV2ServiceException =
                    new FailedEventV2ServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2ServiceException);
            }
        }

        private async ValueTask<EventV2> TryCatch(ReturningEventV2Function returningEventV2Function)
        {
            try
            {
                return await returningEventV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventV2Exception =
                    new TimeoutEventV2Exception(
                        message: "Failed event timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutEventV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventV2Exception nullEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventV2Exception);
            }
            catch (InvalidEventV2Exception invalidEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventV2Exception);
            }
            catch (NotFoundEventV2Exception notFoundEventV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundEventV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventV2Exception =
                    new FailedStorageEventV2Exception(
                        message: "Failed event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventV2Exception =
                    new AlreadyExistsEventV2Exception(
                        message: "Event with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventV2Exception);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidReferenceEventV2Exception =
                    new InvalidReferenceEventV2Exception(
                        message: "Invalid event reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(invalidReferenceEventV2Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedEventV2Exception =
                    new LockedEventV2Exception(
                        message: "Event is locked, try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedEventV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventV2Exception =
                    new FailedStorageEventV2Exception(
                        message: "Failed event storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(failedStorageEventV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventV2ServiceException =
                    new FailedEventV2ServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2ServiceException);
            }
        }

        private async ValueTask<IQueryable<EventV2>> TryCatch(ReturningEventV2sFunction returningEventV2sFunction)
        {
            try
            {
                return await returningEventV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventV2Exception =
                    new TimeoutEventV2Exception(
                        message: "Failed event timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutEventV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventV2Exception =
                    new FailedStorageEventV2Exception(
                        message: "Failed event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageEventV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventV2ServiceException =
                    new FailedEventV2ServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2ServiceException);
            }
        }

        private async ValueTask<EventV2ValidationException> CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2ValidationException);

            return eventV2ValidationException;
        }

        private async ValueTask<EventV2DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventV2DependencyException);

            return eventV2DependencyException;
        }

        private async ValueTask<EventV2DependencyValidationException> CreateAndLogDependencyValidationExceptionAsync(
            Xeption exception)
        {
            var eventV2DependencyValidationException =
                new EventV2DependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2DependencyValidationException);

            return eventV2DependencyValidationException;
        }

        private async ValueTask<EventV2DependencyException> CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2DependencyException);

            return eventV2DependencyException;
        }

        private async ValueTask<EventV2ServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventV2ServiceException =
                new EventV2ServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2ServiceException);

            return eventV2ServiceException;
        }
    }
}
