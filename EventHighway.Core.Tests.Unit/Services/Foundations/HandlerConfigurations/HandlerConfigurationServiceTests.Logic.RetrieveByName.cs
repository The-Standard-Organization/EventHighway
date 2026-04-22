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
        public async Task ShouldRetrieveHandlerConfigurationByNameAsync()
        {
            // given
            string randomHandlerConfigurationName = GetRandomString();
            string inputHandlerConfigurationName = randomHandlerConfigurationName;

            HandlerConfiguration randomHandlerConfiguration =
                CreateRandomHandlerConfiguration();

            randomHandlerConfiguration.Name = inputHandlerConfigurationName;

            HandlerConfiguration selectedHandlerConfiguration =
                randomHandlerConfiguration;

            HandlerConfiguration expectedHandlerConfiguration =
                selectedHandlerConfiguration.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHandlerConfigurationsAsync())
                    .ReturnsAsync(new[] { selectedHandlerConfiguration }.AsQueryable());

            // when
            HandlerConfiguration actualHandlerConfiguration =
                await this.handlerConfigurationService
                    .RetrieveHandlerConfigurationByNameAsync(
                        inputHandlerConfigurationName);

            // then
            actualHandlerConfiguration.Should()
                .BeEquivalentTo(expectedHandlerConfiguration);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllHandlerConfigurationsAsync(),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
