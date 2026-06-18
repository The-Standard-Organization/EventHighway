// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventListenerArchives.V2
{
    internal partial class EventListenerArchiveV2Service
    {
        private delegate ValueTask<IQueryable<EventListenerArchiveV2>> ReturningEventListenerArchiveV2sFunction();
        private delegate ValueTask<EventListenerArchiveV2> ReturningEventListenerArchiveV2Function();
        private delegate ValueTask<IEnumerable<EventListenerArchiveV2>> ReturningEnumerableEventListenerArchiveV2sFunction();

        private async ValueTask<IQueryable<EventListenerArchiveV2>> TryCatch(
            ReturningEventListenerArchiveV2sFunction returningEventListenerArchiveV2sFunction)
        {
            try
            {
                return await returningEventListenerArchiveV2sFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventListenerArchiveV2Exception =
                    new FailedStorageEventListenerArchiveV2Exception(
                        message: "Failed event listener archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventListenerArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventListenerArchiveV2ServiceException =
                    new FailedEventListenerArchiveV2ServiceException(
                        message: "Failed event listener archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerArchiveV2ServiceException);
            }
        }

        private async ValueTask<IEnumerable<EventListenerArchiveV2>> TryCatch(
            ReturningEnumerableEventListenerArchiveV2sFunction returningEnumerableEventListenerArchiveV2sFunction)
        {
            try
            {
                return await returningEnumerableEventListenerArchiveV2sFunction();
            }
            catch (NullEventListenerArchiveV2Exception nullEventListenerArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventListenerArchiveV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventListenerArchiveV2Exception =
                    new FailedStorageEventListenerArchiveV2Exception(
                        message: "Failed event listener archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventListenerArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventListenerArchiveV2ServiceException =
                    new FailedEventListenerArchiveV2ServiceException(
                        message: "Failed event listener archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerArchiveV2ServiceException);
            }
        }

        private async ValueTask<EventListenerArchiveV2> TryCatch(
            ReturningEventListenerArchiveV2Function returningEventListenerArchiveV2Function)
        {
            try
            {
                return await returningEventListenerArchiveV2Function();
            }
            catch (NullEventListenerArchiveV2Exception nullEventListenerArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventListenerArchiveV2Exception);
            }
            catch (InvalidEventListenerArchiveV2Exception invalidEventListenerArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerArchiveV2Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventListenerArchiveV2Exception =
                    new FailedStorageEventListenerArchiveV2Exception(
                        message: "Failed event listener archive storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventListenerArchiveV2Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventListenerArchiveV2Exception =
                    new AlreadyExistsEventListenerArchiveV2Exception(
                        message: "Event listener archive with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventListenerArchiveV2Exception);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidReferenceEventListenerArchiveV2Exception =
                    new InvalidReferenceEventListenerArchiveV2Exception(
                        message: "Invalid event listener archive reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidReferenceEventListenerArchiveV2Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedEventListenerArchiveV2Exception =
                    new LockedEventListenerArchiveV2Exception(
                        message: "Event listener archive is locked, try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    lockedEventListenerArchiveV2Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventListenerArchiveV2Exception =
                    new FailedStorageEventListenerArchiveV2Exception(
                        message: "Failed event listener archive storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedStorageEventListenerArchiveV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventListenerArchiveV2ServiceException =
                    new FailedEventListenerArchiveV2ServiceException(
                        message: "Failed event listener archive service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerArchiveV2ServiceException);
            }
        }

        private async ValueTask<EventListenerArchiveV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventListenerArchiveV2ValidationException =
                new EventListenerArchiveV2ValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerArchiveV2ValidationException);

            return eventListenerArchiveV2ValidationException;
        }

        private async ValueTask<EventListenerArchiveV2DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventListenerArchiveV2DependencyValidationException =
                new EventListenerArchiveV2DependencyValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerArchiveV2DependencyValidationException);

            return eventListenerArchiveV2DependencyValidationException;
        }

        private async ValueTask<EventListenerArchiveV2ServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventListenerArchiveV2ServiceException =
                new EventListenerArchiveV2ServiceException(
                    message: "Event listener archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerArchiveV2ServiceException);

            return eventListenerArchiveV2ServiceException;
        }

        private async ValueTask<EventListenerArchiveV2DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerArchiveV2DependencyException =
                new EventListenerArchiveV2DependencyException(
                    message: "Event listener archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerArchiveV2DependencyException);

            return eventListenerArchiveV2DependencyException;
        }

        private async ValueTask<EventListenerArchiveV2DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerArchiveV2DependencyException =
                new EventListenerArchiveV2DependencyException(
                    message: "Event listener archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventListenerArchiveV2DependencyException);

            return eventListenerArchiveV2DependencyException;
        }
    }
}
