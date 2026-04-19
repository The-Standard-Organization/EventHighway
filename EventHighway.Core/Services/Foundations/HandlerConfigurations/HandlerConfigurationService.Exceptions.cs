// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.HandlerConfigurations
{
    internal partial class HandlerConfigurationService
    {
        private delegate ValueTask<HandlerConfiguration> ReturningHandlerConfigurationFunction();

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
            catch (SqlException sqlException)
            {
                var failedHandlerConfigurationStorageException =
                    new FailedHandlerConfigurationStorageException(
                        message: "Failed handler configuration storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedHandlerConfigurationStorageException);
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
    }
}
