// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V1
{
    public partial class ListenerEventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventV1Archive someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            SqlException sqlException = GetSqlException();

            var failedListenerEventV1ArchiveStorageException =
                new FailedListenerEventV1ArchiveStorageException(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedListenerEventV1ArchiveDependencyException =
                new ListenerEventV1ArchiveDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedListenerEventV1ArchiveStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventV1ArchiveDependencyException actualListenerEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<ListenerEventV1ArchiveDependencyException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
