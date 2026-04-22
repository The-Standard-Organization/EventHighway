// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.HandlerConfigurations
{
    internal partial class HandlerConfigurationService
    {
        private delegate ValueTask<HandlerConfiguration> ReturningHandlerConfigurationFunction();
        private delegate ValueTask<IQueryable<HandlerConfiguration>> ReturningHandlerConfigurationsFunction();

        private async ValueTask<HandlerConfiguration> TryCatch(
            ReturningHandlerConfigurationFunction returningHandlerConfigurationFunction)
        {
            try
            {
                return await returningHandlerConfigurationFunction();
            }
            catch (NullHandlerConfigurationException nullHandlerConfigurationException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullHandlerConfigurationException);
            }
            catch (InvalidHandlerConfigurationException invalidHandlerConfigurationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidHandlerConfigurationException);
            }
            catch (NotFoundHandlerConfigurationException notFoundHandlerConfigurationException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundHandlerConfigurationException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageHandlerConfigurationException =
                    new FailedStorageHandlerConfigurationException(
                        message: "Failed handler configuration storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageHandlerConfigurationException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsHandlerConfigurationException =
                    new AlreadyExistsHandlerConfigurationException(
                        message: "Handler configuration with the same id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsHandlerConfigurationException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidReferenceHandlerConfigurationException =
                    new InvalidReferenceHandlerConfigurationException(
                        message: "Invalid handler configuration reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationExceptionAsync(invalidReferenceHandlerConfigurationException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedHandlerConfigurationException =
                    new LockedHandlerConfigurationException(
                        message: "Handler configuration is locked, try again.",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    lockedHandlerConfigurationException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedStorageHandlerConfigurationException =
                    new FailedStorageHandlerConfigurationException(
                        message: "Failed handler configuration storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedStorageHandlerConfigurationException);
            }
            catch (Exception serviceException)
            {
                var failedHandlerConfigurationServiceException =
                    new FailedHandlerConfigurationServiceException(
                        message: "Failed handler configuration service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(
                    failedHandlerConfigurationServiceException);
            }
        }

        private async ValueTask<IQueryable<HandlerConfiguration>> TryCatch(
            ReturningHandlerConfigurationsFunction returningHandlerConfigurationsFunction)
        {
            try
            {
                return await returningHandlerConfigurationsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageHandlerConfigurationException =
                    new FailedStorageHandlerConfigurationException(
                        message: "Failed handler configuration storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageHandlerConfigurationException);
            }
            catch (Exception serviceException)
            {
                var failedHandlerConfigurationServiceException =
                    new FailedHandlerConfigurationServiceException(
                        message: "Failed handler configuration service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(
                    failedHandlerConfigurationServiceException);
            }
        }

        private async ValueTask<HandlerConfigurationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var handlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(handlerConfigurationValidationException);

            return handlerConfigurationValidationException;
        }

        private async ValueTask<HandlerConfigurationDependencyException>
            CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var handlerConfigurationDependencyException =
                new HandlerConfigurationDependencyException(
                    message: "Handler configuration dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(handlerConfigurationDependencyException);

            return handlerConfigurationDependencyException;
        }

        private async ValueTask<HandlerConfigurationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var handlerConfigurationDependencyValidationException =
                new HandlerConfigurationDependencyValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(handlerConfigurationDependencyValidationException);

            return handlerConfigurationDependencyValidationException;
        }

        private async ValueTask<HandlerConfigurationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var handlerConfigurationDependencyException =
                new HandlerConfigurationDependencyException(
                    message: "Handler configuration dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(handlerConfigurationDependencyException);

            return handlerConfigurationDependencyException;
        }

        private async ValueTask<HandlerConfigurationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var handlerConfigurationServiceException =
                new HandlerConfigurationServiceException(
                    message: "Handler configuration service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(handlerConfigurationServiceException);

            return handlerConfigurationServiceException;
        }
    }
}
