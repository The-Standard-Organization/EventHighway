// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V1
{
    internal partial class ListenerEventV1ArchiveService
    {
        private delegate ValueTask<ListenerEventV1Archive> ReturningListenerEventV1ArchiveFunction();

        private async ValueTask<ListenerEventV1Archive> TryCatch(
            ReturningListenerEventV1ArchiveFunction returningListenerEventV1ArchiveFunction)
        {
            try
            {
                return await returningListenerEventV1ArchiveFunction();
            }
            catch (NullListenerEventV1ArchiveException
                nullListenerEventV1ArchiveException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV1ArchiveException);
            }
            catch (InvalidListenerEventV1ArchiveException
                invalidListenerEventV1ArchiveException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV1ArchiveException);
            }
            catch (SqlException sqlException)
            {
                var failedListenerEventV1ArchiveStorageException =
                    new FailedListenerEventV1ArchiveStorageException(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedListenerEventV1ArchiveStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsListenerEventV1ArchiveException =
                    new AlreadyExistsListenerEventV1ArchiveException(
                        message: "Listener event archive with the same id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsListenerEventV1ArchiveException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedListenerEventV1ArchiveStorageException =
                    new FailedListenerEventV1ArchiveStorageException(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedListenerEventV1ArchiveStorageException);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventV1ArchiveServiceException =
                    new FailedListenerEventV1ArchiveServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV1ArchiveServiceException);
            }
        }

        private async ValueTask<ListenerEventV1ArchiveValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ArchiveValidationException =
                new ListenerEventV1ArchiveValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ArchiveValidationException);

            return listenerEventV1ArchiveValidationException;
        }

        private async ValueTask<ListenerEventV1ArchiveDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ArchiveDependencyException =
                new ListenerEventV1ArchiveDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(listenerEventV1ArchiveDependencyException);

            return listenerEventV1ArchiveDependencyException;
        }

        private async ValueTask<ListenerEventV1ArchiveDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var listenerEventV1ArchiveDependencyValidationException =
                new ListenerEventV1ArchiveDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ArchiveDependencyValidationException);

            return listenerEventV1ArchiveDependencyValidationException;
        }

        private async ValueTask<ListenerEventV1ArchiveDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ArchiveDependencyException =
                new ListenerEventV1ArchiveDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ArchiveDependencyException);

            return listenerEventV1ArchiveDependencyException;
        }

        private async ValueTask<ListenerEventV1ArchiveServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var listenerEventV1ArchiveServiceException =
                new ListenerEventV1ArchiveServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ArchiveServiceException);

            return listenerEventV1ArchiveServiceException;
        }
    }
}
