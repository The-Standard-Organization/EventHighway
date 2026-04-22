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
        public async Task ShouldModifyHandlerConfigurationAsync()
        {
            // given
            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            int randomDaysAgo =
                GetRandomNegativeNumber();

            HandlerConfiguration randomHandlerConfiguration =
                CreateRandomHandlerConfiguration(dates: randomDateTimeOffset);

            HandlerConfiguration inputHandlerConfiguration =
                randomHandlerConfiguration;

            inputHandlerConfiguration.CreatedDate =
                randomDateTimeOffset.AddDays(randomDaysAgo);

            HandlerConfiguration storageHandlerConfiguration =
                inputHandlerConfiguration.DeepClone();

            int randomSecondsAgo =
                GetRandomNegativeNumber();

            storageHandlerConfiguration.UpdatedDate =
                randomDateTimeOffset.AddSeconds(randomSecondsAgo);

            HandlerConfiguration persistedHandlerConfiguration =
                inputHandlerConfiguration;

            HandlerConfiguration expectedHandlerConfiguration =
                persistedHandlerConfiguration.DeepClone();

            Guid inputHandlerConfigurationId =
                inputHandlerConfiguration.Id;

            this.dateTimeBrokerMock
                .InSequence(mockSequence).Setup(broker =>
                    broker.GetDateTimeOffsetAsync())
                        .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock
                .InSequence(mockSequence).Setup(broker =>
                    broker.SelectHandlerConfigurationByIdAsync(
                        inputHandlerConfigurationId))
                            .ReturnsAsync(storageHandlerConfiguration);

            this.storageBrokerMock
                .InSequence(mockSequence).Setup(broker =>
                    broker.UpdateHandlerConfigurationAsync(
                        inputHandlerConfiguration))
                            .ReturnsAsync(persistedHandlerConfiguration);

            // when
            HandlerConfiguration actualHandlerConfiguration =
                await this.handlerConfigurationService
                    .ModifyHandlerConfigurationAsync(inputHandlerConfiguration);

            // then
            actualHandlerConfiguration.Should()
                .BeEquivalentTo(expectedHandlerConfiguration);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(
                    inputHandlerConfigurationId),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHandlerConfigurationAsync(
                    inputHandlerConfiguration),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
