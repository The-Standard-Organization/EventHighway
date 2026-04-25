// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    internal partial class EventArchiveV1Service
    {
        private delegate ValueTask<EventArchiveV1> ReturningEventArchiveV1Function();
        private delegate ValueTask<IQueryable<EventArchiveV1>> ReturningEventV1ArchivesFunction();

        private async ValueTask<EventArchiveV1> TryCatch(
            ReturningEventArchiveV1Function returningEventArchiveV1Function)
        {
            try
            {
                return await returningEventArchiveV1Function();
            }
            catch (NullEventArchiveV1Exception nullEventV1ArchiveException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1ArchiveException);
            }
            catch (InvalidEventArchiveV1Exception invalidEventV1ArchiveException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV1ArchiveException);
            }
            catch (SqlException sqlException)
            {
                var failedEventV1ArchiveStorageException =
                    new FailedStorageEventArchiveV1Exception(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventV1ArchiveStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventV1ArchiveException =
                    new AlreadyExistsEventArchiveV1Exception(
                        message: "Event archive with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventV1ArchiveException);
            }
            catch (ForeignKeyConstraintConflictException
                foreignKeyConstraintConflictException)
            {
                var invalidEventV1ArchiveReferenceException =
                    new InvalidEventArchiveV1ReferenceException(
                        message: "Invalid event archive reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidEventV1ArchiveReferenceException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedEventV1ArchiveStorageException =
                    new FailedStorageEventArchiveV1Exception(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(failedEventV1ArchiveStorageException);
            }
            catch (Exception serviceException)
            {
                var failedEventV1ArchiveServiceException =
                    new FailedEventArchiveV1ServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1ArchiveServiceException);
            }
        }

        private async ValueTask<IQueryable<EventArchiveV1>> TryCatch(
            ReturningEventV1ArchivesFunction returningEventV1ArchivesFunction)
        {
            try
            {
                return await returningEventV1ArchivesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedEventV1ArchiveStorageException =
                    new FailedStorageEventArchiveV1Exception(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventV1ArchiveStorageException);
            }
            catch (Exception serviceException)
            {
                var failedEventV1ArchiveServiceException =
                    new FailedEventArchiveV1ServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1ArchiveServiceException);
            }
        }

        private async ValueTask<EventArchiveV1ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveValidationException);

            return eventV1ArchiveValidationException;
        }

        private async ValueTask<EventArchiveV1DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV1ArchiveDependencyValidationException =
                new EventArchiveV1DependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveDependencyValidationException);

            return eventV1ArchiveDependencyValidationException;
        }

        private async ValueTask<EventArchiveV1DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveDependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveDependencyException);

            return eventV1ArchiveDependencyException;
        }

        private async ValueTask<EventArchiveV1DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveDependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventV1ArchiveDependencyException);

            return eventV1ArchiveDependencyException;
        }

        private async ValueTask<EventArchiveV1ServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveServiceException =
                new EventArchiveV1ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveServiceException);

            return eventV1ArchiveServiceException;
        }
    }
}
