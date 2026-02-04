// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowCriticalDependencyErrorOnRetrieveAllWithListenersIfSqlErrorOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedEventV1StorageException =
                new FailedEventV1StorageException(
                    message: "Failed event storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedEventV1DependencyException =
                new EventV1DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: failedEventV1StorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventV1sWithListenersAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventV1>> retrieveAllEventV1sTask =
                this.eventV1Service.RetrieveAllEventV1sWithListenersAsync();

            EventV1DependencyException actualEventV1DependencyException =
                await Assert.ThrowsAsync<EventV1DependencyException>(
                    retrieveAllEventV1sTask.AsTask);

            // then
            actualEventV1DependencyException.Should()
                .BeEquivalentTo(expectedEventV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventV1sWithListenersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }


    }
}
