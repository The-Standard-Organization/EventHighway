// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;

namespace EventHighway.Core.Services.Foundations.HandlerConfigurations
{
    internal interface IHandlerConfigurationService
    {
        ValueTask<HandlerConfiguration> AddHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration);

        ValueTask<IQueryable<HandlerConfiguration>> RetrieveAllHandlerConfigurationsAsync();
        ValueTask<HandlerConfiguration> RetrieveHandlerConfigurationByIdAsync(Guid handlerConfigurationId);
        ValueTask<HandlerConfiguration> RetrieveHandlerConfigurationByNameAsync(string handlerConfigurationName);
        ValueTask<HandlerConfiguration> ModifyHandlerConfigurationAsync(HandlerConfiguration handlerConfiguration);
        ValueTask<HandlerConfiguration> RemoveHandlerConfigurationByIdAsync(Guid handlerConfigurationId);
    }
}
