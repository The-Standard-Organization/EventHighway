// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<HandlerConfiguration> HandlerConfigurations { get; set; }

        public async ValueTask<HandlerConfiguration> InsertHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration) =>
            await InsertAsync(handlerConfiguration);

        public async ValueTask<IQueryable<HandlerConfiguration>> SelectAllHandlerConfigurationsAsync() =>
            SelectAll<HandlerConfiguration>();

        public async ValueTask<HandlerConfiguration> SelectHandlerConfigurationByIdAsync(
            Guid handlerConfigurationId) =>
            await SelectAsync<HandlerConfiguration>(handlerConfigurationId);

        public async ValueTask<HandlerConfiguration> UpdateHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration) =>
            await UpdateAsync(handlerConfiguration);

        public async ValueTask<HandlerConfiguration> DeleteHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration) =>
            await DeleteAsync(handlerConfiguration);
    }
}
