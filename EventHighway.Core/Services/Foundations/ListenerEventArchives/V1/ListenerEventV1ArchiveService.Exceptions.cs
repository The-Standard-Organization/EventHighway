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
    internal partial class ListenerEventArchiveV1Service
    {
        private delegate ValueTask<ListenerEventArchiveV1> ReturningListenerEventArchiveV1Function();

        private async ValueTask<ListenerEventArchiveV1> TryCatch(
            ReturningListenerEventArchiveV1Function returningListenerEventArchiveV1Function)
        {
            try
            {
                return await returningListenerEventArchiveV1Function();
            }
            catch (NullListenerEventArchiveV1Exception
                nullListenerEventArchiveV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventArchiveV1Exception);
            }
            catch (InvalidListenerEventArchiveV1Exception
                invalidListenerEventArchiveV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventArchiveV1Exception);
            }
            catch (SqlException sqlException)
            {
                var failedListenerEventArchiveV1StorageException =
                    new FailedListenerEventArchiveV1StorageException(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedListenerEventArchiveV1StorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsListenerEventArchiveV1Exception =
                    new AlreadyExistsListenerEventArchiveV1Exception(
                        message: "Listener event archive with the same id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsListenerEventArchiveV1Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedListenerEventArchiveV1StorageException =
                    new FailedListenerEventArchiveV1StorageException(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedListenerEventArchiveV1StorageException);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventArchiveV1ServiceException =
                    new FailedListenerEventArchiveV1ServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventArchiveV1ServiceException);
            }
        }

        private async ValueTask<ListenerEventArchiveV1ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var listenerEventArchiveV1ValidationException =
                new ListenerEventArchiveV1ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV1ValidationException);

            return listenerEventArchiveV1ValidationException;
        }

        private async ValueTask<ListenerEventArchiveV1DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventArchiveV1DependencyException =
                new ListenerEventArchiveV1DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(listenerEventArchiveV1DependencyException);

            return listenerEventArchiveV1DependencyException;
        }

        private async ValueTask<ListenerEventArchiveV1DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var listenerEventArchiveV1DependencyValidationException =
                new ListenerEventArchiveV1DependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV1DependencyValidationException);

            return listenerEventArchiveV1DependencyValidationException;
        }

        private async ValueTask<ListenerEventArchiveV1DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventArchiveV1DependencyException =
                new ListenerEventArchiveV1DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV1DependencyException);

            return listenerEventArchiveV1DependencyException;
        }

        private async ValueTask<ListenerEventArchiveV1ServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var listenerEventArchiveV1ServiceException =
                new ListenerEventArchiveV1ServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV1ServiceException);

            return listenerEventArchiveV1ServiceException;
        }
    }
}
