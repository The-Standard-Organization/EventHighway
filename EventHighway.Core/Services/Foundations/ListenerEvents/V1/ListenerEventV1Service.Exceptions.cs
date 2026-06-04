// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.ListernEvents.V1
{
    internal partial class ListenerEventV1Service
    {
        private delegate ValueTask<ListenerEventV1> ReturningListenerEventV1Function();
        private delegate ValueTask<IQueryable<ListenerEventV1>> ReturningListenerEventV1sFunction();

        private async ValueTask<ListenerEventV1> TryCatch(
            ReturningListenerEventV1Function returningListenerEventV1Function)
        {
            try
            {
                return await returningListenerEventV1Function();
            }
            catch (NullListenerEventV1Exception
                nullListenerEventV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV1Exception);
            }
            catch (InvalidListenerEventV1Exception
                invalidListenerEventV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV1Exception);
            }
            catch (NotFoundListenerEventV1Exception
                notFoundListenerEventV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundListenerEventV1Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventV1Exception =
                    new FailedStorageListenerEventV1Exception(
                        message: "Failed listener event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventV1Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsListenerEventV1Exception =
                    new AlreadyExistsListenerEventV1Exception(
                        message: "Listener event with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsListenerEventV1Exception);
            }
            catch (ForeignKeyConstraintConflictException
                foreignKeyConstraintConflictException)
            {
                var invalidReferenceListenerEventV1Exception =
                    new InvalidReferenceListenerEventV1Exception(
                        message: "Invalid listener event reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidReferenceListenerEventV1Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedListenerEventV1Exception =
                    new LockedListenerEventV1Exception(
                        message: "Listener event is locked, try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    lockedListenerEventV1Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageListenerEventV1Exception =
                    new FailedStorageListenerEventV1Exception(
                        message: "Failed listener event storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedStorageListenerEventV1Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventV1ServiceException =
                    new FailedListenerEventV1ServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV1ServiceException);
            }
        }

        private async ValueTask<IQueryable<ListenerEventV1>> TryCatch(
            ReturningListenerEventV1sFunction returningListenerEventV1sFunction)
        {
            try
            {
                return await returningListenerEventV1sFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageListenerEventV1Exception =
                    new FailedStorageListenerEventV1Exception(
                        message: "Failed listener event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageListenerEventV1Exception);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventV1ServiceException =
                    new FailedListenerEventV1ServiceException(
                        message: "Failed listener event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV1ServiceException);
            }
        }

        private async ValueTask<ListenerEventV1ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ValidationException =
                new ListenerEventV1ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ValidationException);

            return listenerEventV1ValidationException;
        }

        private async ValueTask<ListenerEventV1DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1DependencyException =
                new ListenerEventV1DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(listenerEventV1DependencyException);

            return listenerEventV1DependencyException;
        }

        private async ValueTask<ListenerEventV1DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var listenerEventV1DependencyValidationException =
                new ListenerEventV1DependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1DependencyValidationException);

            return listenerEventV1DependencyValidationException;
        }

        private async ValueTask<ListenerEventV1DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1DependencyException =
                new ListenerEventV1DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1DependencyException);

            return listenerEventV1DependencyException;
        }

        private async ValueTask<ListenerEventV1ServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var listenerEventV1ServiceException =
                new ListenerEventV1ServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ServiceException);

            return listenerEventV1ServiceException;
        }
    }
}
