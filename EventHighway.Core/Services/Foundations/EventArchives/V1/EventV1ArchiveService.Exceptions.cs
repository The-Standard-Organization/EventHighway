// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    internal partial class EventArchiveV1Service
    {
        private delegate ValueTask<EventArchiveV1> ReturningEventArchiveV1Function();
        private delegate ValueTask<IQueryable<EventArchiveV1>> ReturningEventArchiveV1sFunction();

        private async ValueTask<EventArchiveV1> TryCatch(
            ReturningEventArchiveV1Function returningEventArchiveV1Function)
        {
            try
            {
                return await returningEventArchiveV1Function();
            }
            catch (NullEventArchiveV1Exception nullEventArchiveV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV1Exception);
            }
            catch (InvalidEventArchiveV1Exception invalidEventArchiveV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventArchiveV1Exception);
            }
            catch (NotFoundEventArchiveV1Exception notFoundEventArchiveV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundEventArchiveV1Exception);
            }
            catch (SqlException sqlException)
            {
                var failedEventArchiveV1StorageException =
                    new FailedEventArchiveV1StorageException(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventArchiveV1StorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventArchiveV1Exception =
                    new AlreadyExistsEventArchiveV1Exception(
                        message: "Event archive with the same id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventArchiveV1Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedEventArchiveV1Exception =
                    new LockedEventArchiveV1Exception(
                        message: "Event archive is locked, try again.",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedEventArchiveV1Exception);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidEventArchiveV1ReferenceException =
                    new InvalidEventArchiveV1ReferenceException(
                        message: "Invalid event archive reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidEventArchiveV1ReferenceException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedEventArchiveV1StorageException =
                    new FailedEventArchiveV1StorageException(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedEventArchiveV1StorageException);
            }
            catch (Exception serviceException)
            {
                var failedEventArchiveV1ServiceException =
                    new FailedEventArchiveV1ServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV1ServiceException);
            }
        }

        private async ValueTask<IQueryable<EventArchiveV1>> TryCatch(
            ReturningEventArchiveV1sFunction returningEventArchiveV1sFunction)
        {
            try
            {
                return await returningEventArchiveV1sFunction();
            }
            catch (SqlException sqlException)
            {
                var failedEventArchiveV1StorageException =
                    new FailedEventArchiveV1StorageException(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventArchiveV1StorageException);
            }
            catch (Exception serviceException)
            {
                var failedEventArchiveV1ServiceException =
                    new FailedEventArchiveV1ServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV1ServiceException);
            }
        }

        private async ValueTask<EventArchiveV1ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1ValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1ValidationException);

            return eventArchiveV1ValidationException;
        }

        private async ValueTask<EventArchiveV1DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventArchiveV1DependencyValidationException =
                new EventArchiveV1DependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1DependencyValidationException);

            return eventArchiveV1DependencyValidationException;
        }

        private async ValueTask<EventArchiveV1DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1DependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1DependencyException);

            return eventArchiveV1DependencyException;
        }

        private async ValueTask<EventArchiveV1DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1DependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventArchiveV1DependencyException);

            return eventArchiveV1DependencyException;
        }

        private async ValueTask<EventArchiveV1ServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1ServiceException =
                new EventArchiveV1ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1ServiceException);

            return eventArchiveV1ServiceException;
        }
    }
}
