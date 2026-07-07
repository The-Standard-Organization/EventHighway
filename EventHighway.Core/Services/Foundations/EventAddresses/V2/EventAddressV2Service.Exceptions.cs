// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventAddresses.V2
{
    internal partial class EventAddressV2Service
    {
        private delegate ValueTask<EventAddressV2> ReturningEventAddressV2Function();
        private delegate ValueTask<IQueryable<EventAddressV2>> ReturningEventAddressV2sFunction();

        private async ValueTask<EventAddressV2> TryCatch(
            ReturningEventAddressV2Function returningEventAddressV2Function)
        {
            try
            {
                return await returningEventAddressV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventAddressV2Exception =
                    new TimeoutEventAddressV2Exception(
                        message: "Failed event address timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventAddressV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventAddressV2Exception nullEventAddressV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventAddressV2Exception);
            }
            catch (InvalidEventAddressV2Exception invalidEventAddressV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventAddressV2Exception);
            }
            catch (NotFoundEventAddressV2Exception notFoundEventAddressV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundEventAddressV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventAddressV2Exception =
                    new FailedStorageEventAddressV2Exception(
                        message: "Failed event address storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventAddressV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventAddressV2Exception =
                    new AlreadyExistsEventAddressV2Exception(
                        message: "Event address with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventAddressV2Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedEventAddressV2Exception =
                    new LockedEventAddressV2Exception(
                        message: "Event address is locked, try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedEventAddressV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventAddressV2Exception =
                    new FailedStorageEventAddressV2Exception(
                        message: "Failed event address storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(failedStorageEventAddressV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventAddressV2ServiceException =
                    new FailedEventAddressV2ServiceException(
                        message: "Failed event address service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventAddressV2ServiceException);
            }
        }

        private async ValueTask<IQueryable<EventAddressV2>> TryCatch(
            ReturningEventAddressV2sFunction returningEventAddressV2sFunction)
        {
            try
            {
                return await returningEventAddressV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventAddressV2Exception =
                    new TimeoutEventAddressV2Exception(
                        message: "Failed event address timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(timeoutEventAddressV2Exception);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventAddressV2Exception =
                    new FailedStorageEventAddressV2Exception(
                        message: "Failed event address storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventAddressV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventAddressV2ServiceException =
                    new FailedEventAddressV2ServiceException(
                        message: "Failed event address service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventAddressV2ServiceException);
            }
        }

        private async ValueTask<EventAddressV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventAddressV2ValidationException =
                new EventAddressV2ValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventAddressV2ValidationException);

            return eventAddressV2ValidationException;
        }

        private async ValueTask<EventAddressV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventAddressV2DependencyValidationException =
                new EventAddressV2DependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventAddressV2DependencyValidationException);

            return eventAddressV2DependencyValidationException;
        }

        private async ValueTask<EventAddressV2DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventAddressV2DependencyException);

            return eventAddressV2DependencyException;
        }

        private async ValueTask<EventAddressV2DependencyException> CreateAndLogTimeoutDependencyExceptionAsync(
            Xeption exception)
        {
            var eventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventAddressV2DependencyException);

            return eventAddressV2DependencyException;
        }

        private async ValueTask<EventAddressV2DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventAddressV2DependencyException);

            return eventAddressV2DependencyException;
        }

        private async ValueTask<EventAddressV2ServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventAddressV2ServiceException =
                new EventAddressV2ServiceException(
                    message: "Event address service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventAddressV2ServiceException);

            return eventAddressV2ServiceException;
        }
    }
}
