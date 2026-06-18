// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnBulkRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            IQueryable<EventListenerArchiveV2> randomEventListenerArchiveV2s =
                CreateRandomEventListenerArchiveV2s();

            IEnumerable<EventListenerArchiveV2> inputEventListenerArchiveV2s =
                randomEventListenerArchiveV2s.ToList();

            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            sqlException.Data.Add(
                key: nameof(SqlException.Number),
                value: new List<string> { "Some SQL error code" });

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
                broker.DeleteBulkEventListenerArchiveV2sAsync(
                    inputEventListenerArchiveV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask bulkRemoveEventListenerArchiveV2sTask =
                this.eventListenerArchiveV2Service.BulkRemoveEventListenerArchiveV2sAsync(
                    inputEventListenerArchiveV2s,
                        TestContext.Current.CancellationToken);

            EventListenerArchiveV2DependencyException actualEventListenerArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventListenerArchiveV2DependencyException>(
                    bulkRemoveEventListenerArchiveV2sTask.AsTask);

            // then
            actualEventListenerArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteBulkEventListenerArchiveV2sAsync(
                    inputEventListenerArchiveV2s,
                    It.IsAny<CancellationToken>()),
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
        public async Task ShouldThrowServiceExceptionOnBulkRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            IQueryable<EventListenerArchiveV2> randomEventListenerArchiveV2s =
                CreateRandomEventListenerArchiveV2s();

            IEnumerable<EventListenerArchiveV2> inputEventListenerArchiveV2s =
                randomEventListenerArchiveV2s.ToList();

            var serviceException = new Exception();

            serviceException.Data.Add(
                "ErrorCode",
                new List<string> { "ServiceError" });

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
                broker.DeleteBulkEventListenerArchiveV2sAsync(
                    inputEventListenerArchiveV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask bulkRemoveEventListenerArchiveV2sTask =
                this.eventListenerArchiveV2Service.BulkRemoveEventListenerArchiveV2sAsync(
                    inputEventListenerArchiveV2s,
                        TestContext.Current.CancellationToken);

            EventListenerArchiveV2ServiceException actualEventListenerArchiveV2ServiceException =
                await Assert.ThrowsAsync<EventListenerArchiveV2ServiceException>(
                    bulkRemoveEventListenerArchiveV2sTask.AsTask);

            // then
            actualEventListenerArchiveV2ServiceException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteBulkEventListenerArchiveV2sAsync(
                    inputEventListenerArchiveV2s,
                    It.IsAny<CancellationToken>()),
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
