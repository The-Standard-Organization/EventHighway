// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldRetrieveHandlerConfigurationByIdAsync()
        {
            // given
            Guid randomHandlerConfigurationId = Guid.NewGuid();
            Guid inputHandlerConfigurationId = randomHandlerConfigurationId;

            HandlerConfiguration randomHandlerConfiguration =
                CreateRandomHandlerConfiguration();

            HandlerConfiguration selectedHandlerConfiguration =
                randomHandlerConfiguration;

            HandlerConfiguration expectedHandlerConfiguration =
                selectedHandlerConfiguration.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHandlerConfigurationByIdAsync(
                    inputHandlerConfigurationId))
                        .ReturnsAsync(selectedHandlerConfiguration);

            // when
            HandlerConfiguration actualHandlerConfiguration =
                await this.handlerConfigurationService
                    .RetrieveHandlerConfigurationByIdAsync(
                        inputHandlerConfigurationId);

            // then
            actualHandlerConfiguration.Should()
                .BeEquivalentTo(expectedHandlerConfiguration);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(
                    inputHandlerConfigurationId),
                        Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
