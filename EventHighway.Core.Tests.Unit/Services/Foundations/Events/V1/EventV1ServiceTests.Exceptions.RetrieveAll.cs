// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V1
{
    public partial class EventV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = GetSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventV1Exception =
                new FailedStorageEventV1Exception(
                    message: "Failed event storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventV1DependencyException =
                new EventV1DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: failedStorageEventV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventV1sAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventV1>> retrieveAllEventV1sTask =
                this.eventV1Service.RetrieveAllEventV1sAsync();

            EventV1DependencyException actualEventV1DependencyException =
                await Assert.ThrowsAsync<EventV1DependencyException>(
                    retrieveAllEventV1sTask.AsTask);

            // then
            actualEventV1DependencyException.Should()
                .BeEquivalentTo(expectedEventV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV1DependencyException))),
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
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV1ServiceException =
                new FailedEventV1ServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV1ServiceException =
                new EventV1ServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventV1sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventV1>> retrieveAllEventV1sTask =
                this.eventV1Service.RetrieveAllEventV1sAsync();

            EventV1ServiceException actualEventV1ServiceException =
                await Assert.ThrowsAsync<EventV1ServiceException>(
                    retrieveAllEventV1sTask.AsTask);

            // then
            actualEventV1ServiceException.Should()
                .BeEquivalentTo(expectedEventV1ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
