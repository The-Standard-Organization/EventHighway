// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.HandlerConfigurations
{
    public partial class HandlerConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidHandlerConfigurationId = Guid.Empty;

            var invalidHandlerConfigurationException =
                new InvalidHandlerConfigurationException(
                    message: "Handler configuration is invalid, fix the errors and try again.");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.Id),
                values: "Required");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidHandlerConfigurationException);

            // when
            ValueTask<HandlerConfiguration> removeHandlerConfigurationByIdTask =
                this.handlerConfigurationService.RemoveHandlerConfigurationByIdAsync(
                    invalidHandlerConfigurationId);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    removeHandlerConfigurationByIdTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfHandlerConfigurationIsNotFoundAndLogItAsync()
        {
            // given
            Guid nonExistingHandlerConfigurationId = Guid.NewGuid();
            HandlerConfiguration nullHandlerConfiguration = null;

            var notFoundHandlerConfigurationException =
                new NotFoundHandlerConfigurationException(
                    message: $"Could not find handler configuration " +
                        $"with id: {nonExistingHandlerConfigurationId}.");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: notFoundHandlerConfigurationException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(nullHandlerConfiguration);

            // when
            ValueTask<HandlerConfiguration> removeHandlerConfigurationByIdTask =
                this.handlerConfigurationService.RemoveHandlerConfigurationByIdAsync(
                    nonExistingHandlerConfigurationId);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    removeHandlerConfigurationByIdTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
