// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;

namespace EventHighway.Core.Services.Foundations.HandlerConfigurations
{
    internal partial class HandlerConfigurationService : IHandlerConfigurationService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public HandlerConfigurationService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<HandlerConfiguration> AddHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration) =>
        TryCatch(async () =>
        {
            await ValidateHandlerConfigurationOnAddAsync(handlerConfiguration);

            return await storageBroker.InsertHandlerConfigurationAsync(handlerConfiguration);
        });

        public ValueTask<IQueryable<HandlerConfiguration>> RetrieveAllHandlerConfigurationsAsync() =>
            TryCatch(async () => await this.storageBroker.SelectAllHandlerConfigurationsAsync());

        public ValueTask<HandlerConfiguration> RetrieveHandlerConfigurationByIdAsync(
            Guid handlerConfigurationId) =>
        TryCatch(async () =>
        {
            ValidateHandlerConfigurationId(handlerConfigurationId);

            return await this.storageBroker.SelectHandlerConfigurationByIdAsync(handlerConfigurationId);
        });

        public ValueTask<HandlerConfiguration> RetrieveHandlerConfigurationByNameAsync(
            string handlerConfigurationName) =>
        TryCatch(async () =>
        {
            ValidateHandlerConfigurationName(handlerConfigurationName);

            IQueryable<HandlerConfiguration> handlerConfigurations =
                await this.storageBroker.SelectAllHandlerConfigurationsAsync();

            return handlerConfigurations.FirstOrDefault(handlerConfiguration =>
                handlerConfiguration.Name == handlerConfigurationName);
        });

        public ValueTask<HandlerConfiguration> ModifyHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration) =>
        TryCatch(async () =>
        {
            await ValidateHandlerConfigurationOnModifyAsync(handlerConfiguration);

            HandlerConfiguration maybeHandlerConfiguration =
                await this.storageBroker.SelectHandlerConfigurationByIdAsync(
                    handlerConfiguration.Id);

            ValidateHandlerConfigurationExists(maybeHandlerConfiguration, handlerConfiguration.Id);

            return await this.storageBroker.UpdateHandlerConfigurationAsync(handlerConfiguration);
        });

        public ValueTask<HandlerConfiguration> RemoveHandlerConfigurationByIdAsync(
            Guid handlerConfigurationId) =>
        TryCatch(async () =>
        {
            ValidateHandlerConfigurationId(handlerConfigurationId);

            HandlerConfiguration maybeHandlerConfiguration =
                await this.storageBroker.SelectHandlerConfigurationByIdAsync(
                    handlerConfigurationId);

            ValidateHandlerConfigurationExists(maybeHandlerConfiguration, handlerConfigurationId);

            return await this.storageBroker.DeleteHandlerConfigurationAsync(
                maybeHandlerConfiguration);
        });
    }
}
