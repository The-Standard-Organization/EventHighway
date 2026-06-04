// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.Events.V1
{
    internal partial class EventV1Service
    {
        private delegate ValueTask<EventV1> ReturningEventV1Function();
        private delegate ValueTask<IQueryable<EventV1>> ReturningEventV1sFunction();

        private async ValueTask<EventV1> TryCatch(ReturningEventV1Function returningEventV1Function)
        {
            try
            {
                return await returningEventV1Function();
            }
            catch (NullEventV1Exception nullEventV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventV1Exception);
            }
            catch (InvalidEventV1Exception invalidEventV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventV1Exception);
            }
            catch (NotFoundEventV1Exception notFoundEventV1Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundEventV1Exception);
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventV1Exception =
                    new FailedStorageEventV1Exception(
                        message: "Failed event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedStorageEventV1Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEventV1Exception =
                    new AlreadyExistsEventV1Exception(
                        message: "Event with the same id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsEventV1Exception);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidReferenceEventV1Exception =
                    new InvalidReferenceEventV1Exception(
                        message: "Invalid event reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(invalidReferenceEventV1Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedEventV1Exception =
                    new LockedEventV1Exception(
                        message: "Event is locked, try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedEventV1Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageEventV1Exception =
                    new FailedStorageEventV1Exception(
                        message: "Failed event storage error occurred, contact support.",
                        innerException: dbUpdateException,
                        data: dbUpdateException.Data);

                throw await CreateAndLogDependencyExceptionAsync(failedStorageEventV1Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventV1ServiceException =
                    new FailedEventV1ServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV1ServiceException);
            }
        }

        private async ValueTask<IQueryable<EventV1>> TryCatch(ReturningEventV1sFunction returningEventV1sFunction)
        {
            try
            {
                return await returningEventV1sFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageEventV1Exception =
                    new FailedStorageEventV1Exception(
                        message: "Failed event storage error occurred, contact support.",
                        innerException: sqlException,
                        data: sqlException.Data);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageEventV1Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventV1ServiceException =
                    new FailedEventV1ServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV1ServiceException);
            }
        }

        private async ValueTask<EventV1ValidationException> CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventV1ValidationException =
                new EventV1ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ValidationException);

            return eventV1ValidationException;
        }

        private async ValueTask<EventV1DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1DependencyException =
                new EventV1DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(eventV1DependencyException);

            return eventV1DependencyException;
        }

        private async ValueTask<EventV1DependencyValidationException> CreateAndLogDependencyValidationExceptionAsync(
            Xeption exception)
        {
            var eventV1DependencyValidationException =
                new EventV1DependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1DependencyValidationException);

            return eventV1DependencyValidationException;
        }

        private async ValueTask<EventV1DependencyException> CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventV1DependencyException =
                new EventV1DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1DependencyException);

            return eventV1DependencyException;
        }

        private async ValueTask<EventV1ServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventV1ServiceException =
                new EventV1ServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ServiceException);

            return eventV1ServiceException;
        }
    }
}
