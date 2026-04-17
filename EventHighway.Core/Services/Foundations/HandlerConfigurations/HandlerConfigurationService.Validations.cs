// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions;

namespace EventHighway.Core.Services.Foundations.HandlerConfigurations
{
    internal partial class HandlerConfigurationService
    {
        private static ValueTask ValidateHandlerConfigurationOnAddAsync(
            HandlerConfiguration handlerConfiguration)
        {
            ValidateHandlerConfigurationIsNotNull(handlerConfiguration);

            return ValueTask.CompletedTask;
        }

        private static void ValidateHandlerConfigurationIsNotNull(
            HandlerConfiguration handlerConfiguration)
        {
            if (handlerConfiguration is null)
            {
                throw new NullHandlerConfigurationException(
                    message: "Handler configuration is null.");
            }
        }
    }
}
