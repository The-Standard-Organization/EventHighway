// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventArchiveV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageEventArchiveV1Exception =
                new FailedStorageEventArchiveV1Exception(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: sqlException, data: sqlException.Data);

            var expectedEventArchiveV1DependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedStorageEventArchiveV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV1sAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventArchiveV1>> retrieveAllEventArchiveV1sTask =
                this.eventArchiveV1Service.RetrieveAllEventArchiveV1sAsync();

            EventArchiveV1DependencyException actualEventArchiveV1DependencyException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyException>(
                    retrieveAllEventArchiveV1sTask.AsTask);

            // then
            actualEventArchiveV1DependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1DependencyException))),
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

            var failedEventArchiveV1ServiceException =
                new FailedEventArchiveV1ServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventArchiveV1ServiceException =
                new EventArchiveV1ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV1ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV1sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventArchiveV1>> retrieveAllEventArchiveV1sTask =
                this.eventArchiveV1Service.RetrieveAllEventArchiveV1sAsync();

            EventArchiveV1ServiceException actualEventArchiveV1ServiceException =
                await Assert.ThrowsAsync<EventArchiveV1ServiceException>(
                    retrieveAllEventArchiveV1sTask.AsTask);

            // then
            actualEventArchiveV1ServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV1ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
