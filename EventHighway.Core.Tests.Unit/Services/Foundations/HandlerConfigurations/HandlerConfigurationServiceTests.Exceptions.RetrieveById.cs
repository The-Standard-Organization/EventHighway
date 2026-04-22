// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someHandlerConfigurationId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedStorageHandlerConfigurationException =
                new FailedStorageHandlerConfigurationException(
                    message: "Failed handler configuration storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedHandlerConfigurationDependencyException =
                new HandlerConfigurationDependencyException(
                    message: "Handler configuration dependency error occurred, contact support.",
                    innerException: failedStorageHandlerConfigurationException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<HandlerConfiguration> retrieveHandlerConfigurationByIdTask =
                this.handlerConfigurationService.RetrieveHandlerConfigurationByIdAsync(
                    someHandlerConfigurationId);

            HandlerConfigurationDependencyException actualHandlerConfigurationDependencyException =
                await Assert.ThrowsAsync<HandlerConfigurationDependencyException>(
                    retrieveHandlerConfigurationByIdTask.AsTask);

            // then
            actualHandlerConfigurationDependencyException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someHandlerConfigurationId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedHandlerConfigurationServiceException =
                new FailedHandlerConfigurationServiceException(
                    message: "Failed handler configuration service error occurred, contact support.",
                    innerException: serviceException);

            var expectedHandlerConfigurationServiceException =
                new HandlerConfigurationServiceException(
                    message: "Handler configuration service error occurred, contact support.",
                    innerException: failedHandlerConfigurationServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<HandlerConfiguration> retrieveHandlerConfigurationByIdTask =
                this.handlerConfigurationService.RetrieveHandlerConfigurationByIdAsync(
                    someHandlerConfigurationId);

            HandlerConfigurationServiceException actualHandlerConfigurationServiceException =
                await Assert.ThrowsAsync<HandlerConfigurationServiceException>(
                    retrieveHandlerConfigurationByIdTask.AsTask);

            // then
            actualHandlerConfigurationServiceException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
