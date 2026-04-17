// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions;
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
    }
}
