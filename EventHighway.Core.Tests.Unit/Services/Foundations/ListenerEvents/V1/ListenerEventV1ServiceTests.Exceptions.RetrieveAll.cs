// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V1
{
    public partial class ListenerEventV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageListenerEventV1Exception =
                new FailedStorageListenerEventV1Exception(
                    message: "Failed listener event storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedListenerEventV1DependencyException =
                new ListenerEventV1DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventV1sAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<ListenerEventV1>> retrieveAllListenerEventV1sTask =
                this.listenerEventV1Service.RetrieveAllListenerEventV1sAsync();

            ListenerEventV1DependencyException actualListenerEventV1DependencyException =
                await Assert.ThrowsAsync<ListenerEventV1DependencyException>(
                    retrieveAllListenerEventV1sTask.AsTask);

            // then
            actualListenerEventV1DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();

            var failedListenerEventV1ServiceException =
                new FailedListenerEventV1ServiceException(
                    message: "Failed listener event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV1ServiceException =
                new ListenerEventV1ServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV1ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventV1sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<ListenerEventV1>> retrieveAllListenerEventV1sTask =
                this.listenerEventV1Service.RetrieveAllListenerEventV1sAsync();

            ListenerEventV1ServiceException actualListenerEventV1ServiceException =
                await Assert.ThrowsAsync<ListenerEventV1ServiceException>(
                    retrieveAllListenerEventV1sTask.AsTask);

            // then
            actualListenerEventV1ServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV1ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
