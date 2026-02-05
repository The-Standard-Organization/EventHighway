// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    internal partial class EventV1ArchiveService
    {
        private delegate ValueTask<EventV1Archive> ReturningEventV1ArchiveFunction();

        private async ValueTask<EventV1Archive> TryCatch(
            ReturningEventV1ArchiveFunction returningEventV1ArchiveFunction)
        {
            try
            {
                return await returningEventV1ArchiveFunction();
            }
            catch (NullEventV1ArchiveException nullEventV1ArchiveException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1ArchiveException);
            }
            catch (InvalidEventV1ArchiveException invalidEventV1ArchiveException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV1ArchiveException);
            }
            catch (SqlException sqlException)
            {
                var failedEventV1ArchiveStorageException =
                    new FailedEventV1ArchiveStorageException(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedEventV1ArchiveStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventV1ArchiveException =
                    new AlreadyExistsEventV1ArchiveException(
                        message: "Event archive with the same id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventV1ArchiveException);
            }
            catch (ForeignKeyConstraintConflictException
                foreignKeyConstraintConflictException)
            {
                var invalidEventV1ArchiveReferenceException =
                    new InvalidEventV1ArchiveReferenceException(
                        message: "Invalid event archive reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidEventV1ArchiveReferenceException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedEventV1ArchiveStorageException =
                    new FailedEventV1ArchiveStorageException(
                        message: "Failed event archive storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedEventV1ArchiveStorageException);
            }
            catch (Exception serviceException)
            {
                var failedEventV1ArchiveServiceException =
                    new FailedEventV1ArchiveServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1ArchiveServiceException);
            }
        }

        private async ValueTask<EventV1ArchiveValidationException> CreateAndLogValidationExceptionAsync(
           Xeption exception)
        {
            var eventV1ArchiveValidationException =
                new EventV1ArchiveValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveValidationException);

            return eventV1ArchiveValidationException;
        }

        private async ValueTask<EventV1ArchiveDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV1ArchiveDependencyValidationException =
                new EventV1ArchiveDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveDependencyValidationException);

            return eventV1ArchiveDependencyValidationException;
        }

        private async ValueTask<EventV1ArchiveDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveDependencyException =
                new EventV1ArchiveDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveDependencyException);

            return eventV1ArchiveDependencyException;
        }

        private async ValueTask<EventV1ArchiveDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveDependencyException =
                new EventV1ArchiveDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventV1ArchiveDependencyException);

            return eventV1ArchiveDependencyException;
        }

        private async ValueTask<EventV1ArchiveServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveServiceException =
                new EventV1ArchiveServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveServiceException);

            return eventV1ArchiveServiceException;
        }
    }
}
