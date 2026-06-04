// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V1
{
    public partial class EventListenerV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageEventListenerV1Exception =
                new FailedStorageEventListenerV1Exception(
                    message: "Failed event listener storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventListenerV1DependencyException =
                new EventListenerV1DependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: failedStorageEventListenerV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventListenerV1sAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventListenerV1>> retrieveAllEventListenerV1sTask =
                this.eventListenerV1Service.RetrieveAllEventListenerV1sAsync();

            EventListenerV1DependencyException actualEventListenerV1DependencyException =
                await Assert.ThrowsAsync<EventListenerV1DependencyException>(
                    retrieveAllEventListenerV1sTask.AsTask);

            // then
            actualEventListenerV1DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventListenerV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();

            var failedEventListenerV1ServiceException =
                new FailedEventListenerV1ServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV1ServiceException =
                new EventListenerV1ServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV1ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventListenerV1sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventListenerV1>> retrieveAllEventListenerV1sTask =
                this.eventListenerV1Service.RetrieveAllEventListenerV1sAsync();

            EventListenerV1ServiceException actualEventListenerV1ServiceException =
                await Assert.ThrowsAsync<EventListenerV1ServiceException>(
                    retrieveAllEventListenerV1sTask.AsTask);

            // then
            actualEventListenerV1ServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV1ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventListenerV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
