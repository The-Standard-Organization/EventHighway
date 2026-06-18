// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListenerArchives.V2
{
    public partial class EventListenerArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventListenerArchiveV2Exception =
                new FailedStorageEventListenerArchiveV2Exception(
                    message: "Failed event listener archive storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventListenerArchiveV2DependencyException =
                new EventListenerArchiveV2DependencyException(
                    message: "Event listener archive dependency error occurred, contact support.",
                    innerException: failedStorageEventListenerArchiveV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventListenerArchiveV2sAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventListenerArchiveV2>> retrieveAllTask =
                this.eventListenerArchiveV2Service.RetrieveAllEventListenerArchiveV2sAsync();

            EventListenerArchiveV2DependencyException actualEventListenerArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventListenerArchiveV2DependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualEventListenerArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventListenerArchiveV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2DependencyException))),
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

            var failedEventListenerArchiveV2ServiceException =
                new FailedEventListenerArchiveV2ServiceException(
                    message: "Failed event listener archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerArchiveV2ServiceException =
                new EventListenerArchiveV2ServiceException(
                    message: "Event listener archive service error occurred, contact support.",
                    innerException: failedEventListenerArchiveV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventListenerArchiveV2sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventListenerArchiveV2>> retrieveAllTask =
                this.eventListenerArchiveV2Service.RetrieveAllEventListenerArchiveV2sAsync();

            EventListenerArchiveV2ServiceException actualEventListenerArchiveV2ServiceException =
                await Assert.ThrowsAsync<EventListenerArchiveV2ServiceException>(
                    retrieveAllTask.AsTask);

            // then
            actualEventListenerArchiveV2ServiceException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventListenerArchiveV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
