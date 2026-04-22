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
        public async Task ShouldRemoveHandlerConfigurationByIdAsync()
        {
            // given
            Guid randomHandlerConfigurationId = Guid.NewGuid();
            Guid inputHandlerConfigurationId = randomHandlerConfigurationId;

            HandlerConfiguration randomHandlerConfiguration =
                CreateRandomHandlerConfiguration();

            HandlerConfiguration retrievedHandlerConfiguration =
                randomHandlerConfiguration;

            HandlerConfiguration deletedHandlerConfiguration =
                retrievedHandlerConfiguration;

            HandlerConfiguration expectedHandlerConfiguration =
                deletedHandlerConfiguration.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHandlerConfigurationByIdAsync(
                    inputHandlerConfigurationId))
                        .ReturnsAsync(retrievedHandlerConfiguration);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteHandlerConfigurationAsync(
                    retrievedHandlerConfiguration))
                        .ReturnsAsync(deletedHandlerConfiguration);

            // when
            HandlerConfiguration actualHandlerConfiguration =
                await this.handlerConfigurationService
                    .RemoveHandlerConfigurationByIdAsync(
                        inputHandlerConfigurationId);

            // then
            actualHandlerConfiguration.Should()
                .BeEquivalentTo(expectedHandlerConfiguration);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(
                    inputHandlerConfigurationId),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteHandlerConfigurationAsync(
                    retrievedHandlerConfiguration),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
