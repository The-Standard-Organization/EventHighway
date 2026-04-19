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
        public async Task ShouldThrowValidationExceptionOnAddIfHandlerConfigurationIsNullAndLogItAsync()
        {
            // given
            HandlerConfiguration nullHandlerConfiguration = null;

            var nullHandlerConfigurationException =
                new NullHandlerConfigurationException(
                    message: "Handler configuration is null.");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: nullHandlerConfigurationException);

            // when
            ValueTask<HandlerConfiguration> addHandlerConfigurationTask =
                this.handlerConfigurationService.AddHandlerConfigurationAsync(nullHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    addHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should().BeEquivalentTo(
                expectedHandlerConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfHandlerConfigurationIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidHandlerConfiguration = new HandlerConfiguration
            {
                Id = Guid.Empty,
                Name = invalidText,
                Value = invalidText
            };

            var invalidHandlerConfigurationException =
                new InvalidHandlerConfigurationException(
                    message: "Handler configuration is invalid, fix the errors and try again.");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.Id),
                values: "Required");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.Name),
                values: "Required");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.Value),
                values: "Required");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.CreatedDate),
                values: "Required");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.UpdatedDate),
                values: "Required");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidHandlerConfigurationException);

            // when
            ValueTask<HandlerConfiguration> addHandlerConfigurationTask =
                this.handlerConfigurationService.AddHandlerConfigurationAsync(invalidHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    addHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should().BeEquivalentTo(
                expectedHandlerConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfNameIsInvalidLengthAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            HandlerConfiguration randomHandlerConfiguration = CreateRandomHandlerConfiguration(randomDateTimeOffset);
            HandlerConfiguration invalidHandlerConfiguration = randomHandlerConfiguration;
            invalidHandlerConfiguration.Name = GetRandomStringWithLengthOf(451);

            var invalidHandlerConfigurationException =
                new InvalidHandlerConfigurationException(
                    message: "Handler configuration is invalid, fix the errors and try again.");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.Name),
                values: $"Exceeds {invalidHandlerConfiguration.Name.Length - 1} characters");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidHandlerConfigurationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<HandlerConfiguration> addHandlerConfigurationTask =
                this.handlerConfigurationService.AddHandlerConfigurationAsync(invalidHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    addHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should().BeEquivalentTo(
                expectedHandlerConfigurationValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset anotherRandomDateTimeOffset = GetRandomDateTimeOffset();
            HandlerConfiguration randomHandlerConfiguration = CreateRandomHandlerConfiguration(randomDateTimeOffset);
            HandlerConfiguration invalidHandlerConfiguration = randomHandlerConfiguration;
            invalidHandlerConfiguration.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidHandlerConfigurationException =
                new InvalidHandlerConfigurationException(
                    message: "Handler configuration is invalid, fix the errors and try again.");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.CreatedDate),
                values: $"Date is not the same as {nameof(HandlerConfiguration.UpdatedDate)}");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidHandlerConfigurationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<HandlerConfiguration> addHandlerConfigurationTask =
                this.handlerConfigurationService.AddHandlerConfigurationAsync(invalidHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    addHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should().BeEquivalentTo(
                expectedHandlerConfigurationValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            HandlerConfiguration randomHandlerConfiguration =
                CreateRandomHandlerConfiguration(randomDateTimeOffset.AddMinutes(minutesBeforeAndAfter));

            HandlerConfiguration invalidHandlerConfiguration = randomHandlerConfiguration;

            var invalidHandlerConfigurationException =
                new InvalidHandlerConfigurationException(
                    message: "Handler configuration is invalid, fix the errors and try again.");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.CreatedDate),
                values: "Date is not recent");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidHandlerConfigurationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<HandlerConfiguration> addHandlerConfigurationTask =
                this.handlerConfigurationService.AddHandlerConfigurationAsync(invalidHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    addHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should().BeEquivalentTo(
                expectedHandlerConfigurationValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
