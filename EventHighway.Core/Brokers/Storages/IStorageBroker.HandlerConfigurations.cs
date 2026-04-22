// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<HandlerConfiguration> InsertHandlerConfigurationAsync(HandlerConfiguration handlerConfiguration);
        ValueTask<IQueryable<HandlerConfiguration>> SelectAllHandlerConfigurationsAsync();
        ValueTask<HandlerConfiguration> SelectHandlerConfigurationByIdAsync(Guid handlerConfigurationId);
        ValueTask<HandlerConfiguration> UpdateHandlerConfigurationAsync(HandlerConfiguration handlerConfiguration);
        ValueTask<HandlerConfiguration> DeleteHandlerConfigurationAsync(HandlerConfiguration handlerConfiguration);
    }
}
