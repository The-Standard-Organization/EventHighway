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
        public async Task ShouldThrowValidationExceptionOnModifyIfHandlerConfigurationIsNullAndLogItAsync()
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
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    nullHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    modifyHandlerConfigurationTask.AsTask);

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData(" ")]
            public async Task ShouldThrowValidationExceptionOnModifyIfHandlerConfigurationIsInvalidAndLogItAsync(
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
                    values:
                    [
                        "Required",
                        $"Date is the same as {nameof(HandlerConfiguration.CreatedDate)}",
                        "Date is not recent"
                    ]);

                var expectedHandlerConfigurationValidationException =
                    new HandlerConfigurationValidationException(
                        message: "Handler configuration validation error occurred, fix the errors and try again.",
                        innerException: invalidHandlerConfigurationException);

                this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetDateTimeOffsetAsync())
                        .ReturnsAsync(GetRandomDateTimeOffset());

                // when
                ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                    this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                        invalidHandlerConfiguration);

                HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                    await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                        modifyHandlerConfigurationTask.AsTask);

                // then
                actualHandlerConfigurationValidationException.Should()
                    .BeEquivalentTo(expectedHandlerConfigurationValidationException);

                this.dateTimeBrokerMock.Verify(broker =>
                    broker.GetDateTimeOffsetAsync(),
                        Times.Once);

                this.loggingBrokerMock.Verify(broker =>
                    broker.LogErrorAsync(It.Is(SameExceptionAs(
                        expectedHandlerConfigurationValidationException))),
                            Times.Once);

                this.storageBrokerMock.Verify(broker =>
                    broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                        Times.Never);

                this.storageBrokerMock.Verify(broker =>
                    broker.UpdateHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                        Times.Never);

                this.dateTimeBrokerMock.VerifyNoOtherCalls();
                this.loggingBrokerMock.VerifyNoOtherCalls();
                this.storageBrokerMock.VerifyNoOtherCalls();
            }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfNameIsInvalidLengthAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            HandlerConfiguration randomHandlerConfiguration = CreateRandomHandlerConfiguration(randomDateTimeOffset);
            HandlerConfiguration invalidHandlerConfiguration = randomHandlerConfiguration;
            invalidHandlerConfiguration.CreatedDate = randomDateTimeOffset.AddDays(GetRandomNegativeNumber());
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
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    invalidHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    modifyHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            HandlerConfiguration randomHandlerConfiguration =
                CreateRandomHandlerConfiguration(dates: randomDateTimeOffset);

            HandlerConfiguration invalidHandlerConfiguration = randomHandlerConfiguration;

            var invalidHandlerConfigurationException =
                new InvalidHandlerConfigurationException(
                    message: "Handler configuration is invalid, fix the errors and try again.");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.UpdatedDate),
                values: $"Date is the same as {nameof(HandlerConfiguration.CreatedDate)}");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidHandlerConfigurationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    invalidHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    modifyHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            HandlerConfiguration randomHandlerConfiguration =
                CreateRandomHandlerConfiguration(dates: randomDateTimeOffset);

            HandlerConfiguration invalidHandlerConfiguration = randomHandlerConfiguration;
            invalidHandlerConfiguration.CreatedDate = randomDateTimeOffset.AddDays(GetRandomNegativeNumber());

            invalidHandlerConfiguration.UpdatedDate =
                randomDateTimeOffset.AddMinutes(minutesBeforeAndAfter);

            var invalidHandlerConfigurationException =
                new InvalidHandlerConfigurationException(
                    message: "Handler configuration is invalid, fix the errors and try again.");

            invalidHandlerConfigurationException.AddData(
                key: nameof(HandlerConfiguration.UpdatedDate),
                values: "Date is not recent");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidHandlerConfigurationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    invalidHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    modifyHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfHandlerConfigurationDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();

            HandlerConfiguration randomHandlerConfiguration =
                CreateRandomHandlerConfiguration(dates: randomDateTimeOffset);

            HandlerConfiguration nonExistHandlerConfiguration = randomHandlerConfiguration;
            Guid nonExistHandlerConfigurationId = nonExistHandlerConfiguration.Id;
            nonExistHandlerConfiguration.CreatedDate = randomDateTimeOffset.AddDays(randomDaysAgo);
            HandlerConfiguration nullHandlerConfiguration = null;

            var notFoundHandlerConfigurationException =
                new NotFoundHandlerConfigurationException(
                    message: $"Could not find handler configuration with id: {nonExistHandlerConfigurationId}.");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: notFoundHandlerConfigurationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHandlerConfigurationByIdAsync(nonExistHandlerConfigurationId))
                    .ReturnsAsync(nullHandlerConfiguration);

            // when
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    nonExistHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    modifyHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(nonExistHandlerConfigurationId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
