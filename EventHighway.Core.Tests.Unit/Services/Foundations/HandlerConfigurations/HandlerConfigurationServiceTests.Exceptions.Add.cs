// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;


namespace EventHighway.Core.Tests.Unit.Services.Foundations.HandlerConfigurations
{
    public partial class HandlerConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            SqlException sqlException = GetSqlException();

            var failedHandlerConfigurationStorageException =
                new FailedHandlerConfigurationStorageException(
                    message: "Failed handler configuration storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedHandlerConfigurationDependencyException =
                new HandlerConfigurationDependencyException(
                    message: "Handler configuration dependency error occurred, contact support.",
                    innerException: failedHandlerConfigurationStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<HandlerConfiguration> addHandlerConfigurationTask =
                this.handlerConfigurationService.AddHandlerConfigurationAsync(someHandlerConfiguration);

            HandlerConfigurationDependencyException actualHandlerConfigurationDependencyException =
                await Assert.ThrowsAsync<HandlerConfigurationDependencyException>(
                    addHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationDependencyException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfHandlerConfigAlreadyExistsAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            var alreadyExistsHandlerConfigurationException =
                new AlreadyExistsHandlerConfigurationException(
                    message: "Handler configuration with the same id already exists.",
                    innerException: duplicateKeyException);

            var expectedHandlerConfigurationDependencyValidationException =
                new HandlerConfigurationDependencyValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsHandlerConfigurationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<HandlerConfiguration> addHandlerConfigurationTask =
                this.handlerConfigurationService.AddHandlerConfigurationAsync(someHandlerConfiguration);

            HandlerConfigurationDependencyValidationException actualHandlerConfigurationDependencyValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationDependencyValidationException>(
                    addHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationDependencyValidationException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            string someMessage = GetRandomString();
            var foreignKeyConstraintConflictException = new ForeignKeyConstraintConflictException(someMessage);

            var invalidHandlerConfigurationReferenceException =
                new InvalidHandlerConfigurationReferenceException(
                    message: "Invalid handler configuration reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            var expectedHandlerConfigurationDependencyValidationException =
                new HandlerConfigurationDependencyValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidHandlerConfigurationReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<HandlerConfiguration> addHandlerConfigurationTask =
                this.handlerConfigurationService.AddHandlerConfigurationAsync(someHandlerConfiguration);

            HandlerConfigurationDependencyValidationException actualHandlerConfigurationDependencyValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationDependencyValidationException>(
                    addHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationDependencyValidationException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationDependencyValidationException))),
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
