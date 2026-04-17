// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;

namespace EventHighway.Core.Services.Foundations.HandlerConfigurations
{
    internal partial class HandlerConfigurationService : IHandlerConfigurationService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public HandlerConfigurationService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<HandlerConfiguration> AddHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration) =>
            await storageBroker.InsertHandlerConfigurationAsync(handlerConfiguration);
    }
}
