// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.HandlerConfigurations
{
    public partial class HandlerConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllHandlerConfigurationsAsync()
        {
            // given
            IQueryable<HandlerConfiguration> randomHandlerConfigurations =
                CreateRandomHandlerConfigurations();

            IQueryable<HandlerConfiguration> retrievedHandlerConfigurations =
                randomHandlerConfigurations;

            IQueryable<HandlerConfiguration> expectedHandlerConfigurations =
                retrievedHandlerConfigurations.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHandlerConfigurationsAsync())
                    .ReturnsAsync(retrievedHandlerConfigurations);

            // when
            IQueryable<HandlerConfiguration> actualHandlerConfigurations =
                await this.handlerConfigurationService
                    .RetrieveAllHandlerConfigurationsAsync();

            // then
            actualHandlerConfigurations.Should().BeEquivalentTo(
                expectedHandlerConfigurations);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllHandlerConfigurationsAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
