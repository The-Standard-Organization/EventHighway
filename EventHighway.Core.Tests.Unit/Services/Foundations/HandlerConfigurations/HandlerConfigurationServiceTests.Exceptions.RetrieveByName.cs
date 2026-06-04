// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByNameIfSqlErrorOccursAndLogItAsync()
        {
            // given
            string someHandlerConfigurationName = GetRandomString();
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

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHandlerConfigurationsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<HandlerConfiguration> retrieveHandlerConfigurationByNameTask =
                this.handlerConfigurationService.RetrieveHandlerConfigurationByNameAsync(
                    someHandlerConfigurationName);

            HandlerConfigurationDependencyException actualHandlerConfigurationDependencyException =
                await Assert.ThrowsAsync<HandlerConfigurationDependencyException>(
                    retrieveHandlerConfigurationByNameTask.AsTask);

            // then
            actualHandlerConfigurationDependencyException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllHandlerConfigurationsAsync(),
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
        public async Task ShouldThrowServiceExceptionOnRetrieveByNameIfExceptionOccursAndLogItAsync()
        {
            // given
            string someHandlerConfigurationName = GetRandomString();
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

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllHandlerConfigurationsAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<HandlerConfiguration> retrieveHandlerConfigurationByNameTask =
                this.handlerConfigurationService.RetrieveHandlerConfigurationByNameAsync(
                    someHandlerConfigurationName);

            HandlerConfigurationServiceException actualHandlerConfigurationServiceException =
                await Assert.ThrowsAsync<HandlerConfigurationServiceException>(
                    retrieveHandlerConfigurationByNameTask.AsTask);

            // then
            actualHandlerConfigurationServiceException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllHandlerConfigurationsAsync(),
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
