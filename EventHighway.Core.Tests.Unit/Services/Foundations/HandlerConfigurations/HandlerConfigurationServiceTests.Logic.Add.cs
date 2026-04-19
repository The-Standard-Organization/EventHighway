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
        public async Task ShouldAddHandlerConfigurationAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            HandlerConfiguration randomHandlerConfiguration = CreateRandomHandlerConfiguration(randomDateTimeOffset);
            HandlerConfiguration inputHandlerConfiguration = randomHandlerConfiguration;
            HandlerConfiguration insertedHandlerConfiguration = inputHandlerConfiguration;
            HandlerConfiguration expectedHandlerConfiguration = insertedHandlerConfiguration.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertHandlerConfigurationAsync(inputHandlerConfiguration))
                    .ReturnsAsync(insertedHandlerConfiguration);

            // when
            HandlerConfiguration actualHandlerConfiguration =
                await this.handlerConfigurationService.AddHandlerConfigurationAsync(
                    inputHandlerConfiguration);

            // then
            actualHandlerConfiguration.Should().BeEquivalentTo(
                expectedHandlerConfiguration);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHandlerConfigurationAsync(inputHandlerConfiguration),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
