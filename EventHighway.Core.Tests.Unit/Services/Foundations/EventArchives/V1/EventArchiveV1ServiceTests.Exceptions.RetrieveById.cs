// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someEventArchiveV1Id = GetRandomId();
            SqlException sqlException = CreateSqlException();

            var failedEventArchiveV1StorageException =
                new FailedEventArchiveV1StorageException(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedEventArchiveV1DependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedEventArchiveV1StorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventArchiveV1> retrieveEventArchiveV1ByIdTask =
                this.eventArchiveV1Service.RetrieveEventArchiveV1ByIdAsync(
                    someEventArchiveV1Id);

            EventArchiveV1DependencyException actualEventArchiveV1DependencyException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyException>(
                    retrieveEventArchiveV1ByIdTask.AsTask);

            // then
            actualEventArchiveV1DependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV1ByIdAsync(It.IsAny<Guid>()),
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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventArchiveV1Id = GetRandomId();
            var serviceException = new Exception();

            var failedEventArchiveV1ServiceException =
                new FailedEventArchiveV1ServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventArchiveV1ServiceException =
                new EventArchiveV1ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV1ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventArchiveV1> retrieveEventArchiveV1ByIdTask =
                this.eventArchiveV1Service.RetrieveEventArchiveV1ByIdAsync(
                    someEventArchiveV1Id);

            EventArchiveV1ServiceException actualEventArchiveV1ServiceException =
                await Assert.ThrowsAsync<EventArchiveV1ServiceException>(
                    retrieveEventArchiveV1ByIdTask.AsTask);

            // then
            actualEventArchiveV1ServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV1ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV1ByIdAsync(It.IsAny<Guid>()),
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
