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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            SqlException sqlException = GetSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

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
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    someHandlerConfiguration);

            HandlerConfigurationDependencyException actualHandlerConfigurationDependencyException =
                await Assert.ThrowsAsync<HandlerConfigurationDependencyException>(
                    modifyHandlerConfigurationTask.AsTask);

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
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            string someMessage = GetRandomString();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(someMessage);

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
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    someHandlerConfiguration);

            HandlerConfigurationDependencyValidationException actualHandlerConfigurationDependencyValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationDependencyValidationException>(
                    modifyHandlerConfigurationTask.AsTask);

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
                broker.UpdateHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyOccursAndLogItAsync()
        {
            // given
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();
            dbUpdateConcurrencyException.Data.Add("ErrorCode", new List<string> { "ConcurrencyError" });

            var lockedHandlerConfigurationException =
                new LockedHandlerConfigurationException(
                    message: "Handler configuration is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedHandlerConfigurationDependencyValidationException =
                new HandlerConfigurationDependencyValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: lockedHandlerConfigurationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    someHandlerConfiguration);

            HandlerConfigurationDependencyValidationException actualHandlerConfigurationDependencyValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationDependencyValidationException>(
                    modifyHandlerConfigurationTask.AsTask);

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
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            var dbUpdateException = new DbUpdateException();
            dbUpdateException.Data.Add("ErrorCode", new List<string> { "StorageError" });

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
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    someHandlerConfiguration);

            HandlerConfigurationDependencyException actualHandlerConfigurationDependencyException =
                await Assert.ThrowsAsync<HandlerConfigurationDependencyException>(
                    modifyHandlerConfigurationTask.AsTask);

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
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfExceptionOccursAndLogItAsync()
        {
            // given
            HandlerConfiguration someHandlerConfiguration = CreateRandomHandlerConfiguration();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

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
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    someHandlerConfiguration);

            HandlerConfigurationServiceException actualHandlerConfigurationServiceException =
                await Assert.ThrowsAsync<HandlerConfigurationServiceException>(
                    modifyHandlerConfigurationTask.AsTask);

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
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
