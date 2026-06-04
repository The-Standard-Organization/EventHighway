// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

            var failedStorageHandlerConfigurationException =
                new FailedStorageHandlerConfigurationException(
                    message: "Failed handler configuration storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedHandlerConfigurationDependencyException =
                new HandlerConfigurationDependencyException(
                    message: "Handler configuration dependency error occurred, contact support.",
                    innerException: failedStorageHandlerConfigurationException);

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
            duplicateKeyException.Data.Add("ErrorCode", new List<string> { "DuplicateError" });

            var alreadyExistsHandlerConfigurationException =
                new AlreadyExistsHandlerConfigurationException(
                    message: "Handler configuration with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

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

            var invalidReferenceHandlerConfigurationException =
                new InvalidReferenceHandlerConfigurationException(
                    message: "Invalid handler configuration reference error occurred.",
                    innerException: foreignKeyConstraintConflictException,
                    data: foreignKeyConstraintConflictException.Data);

            var expectedHandlerConfigurationDependencyValidationException =
                new HandlerConfigurationDependencyValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidReferenceHandlerConfigurationException);

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

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            var dbUpdateException = new DbUpdateException();
            dbUpdateException.Data.Add("ErrorCode", new List<string> { "UpdateError" });

            var failedStorageHandlerConfigurationException =
                new FailedStorageHandlerConfigurationException(
                    message: "Failed handler configuration storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedHandlerConfigurationDependencyException =
                new HandlerConfigurationDependencyException(
                    message: "Handler configuration dependency error occurred, contact support.",
                    innerException: failedStorageHandlerConfigurationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

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
                broker.LogErrorAsync(It.Is(SameExceptionAs(
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
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            var serviceException = new Exception();

            var failedHandlerConfigurationServiceException =
                new FailedHandlerConfigurationServiceException(
                    message: "Failed handler configuration service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedHandlerConfigurationServiceException =
                new HandlerConfigurationServiceException(
                    message: "Handler configuration service error occurred, contact support.",
                    innerException: failedHandlerConfigurationServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<HandlerConfiguration> addHandlerConfigurationTask =
                this.handlerConfigurationService.AddHandlerConfigurationAsync(someHandlerConfiguration);

            HandlerConfigurationServiceException actualHandlerConfigurationServiceException =
                await Assert.ThrowsAsync<HandlerConfigurationServiceException>(
                    addHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationServiceException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationServiceException))),
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
