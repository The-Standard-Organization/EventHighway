// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Services.Foundations.HandlerConfigurations;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.HandlerConfigurations
{
    public partial class HandlerConfigurationServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly IHandlerConfigurationService handlerConfigurationService;

        public HandlerConfigurationServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();

            this.handlerConfigurationService = new HandlerConfigurationService(
                storageBroker: this.storageBrokerMock.Object);
        }

        private static HandlerConfiguration CreateRandomHandlerConfiguration() =>
            CreateHandlerConfigurationFiller().Create();

        private static Filler<HandlerConfiguration> CreateHandlerConfigurationFiller()
        {
            var filler = new Filler<HandlerConfiguration>();

            filler.Setup()
                .OnType<Guid>().Use(Guid.NewGuid);

            return filler;
        }
    }
}
