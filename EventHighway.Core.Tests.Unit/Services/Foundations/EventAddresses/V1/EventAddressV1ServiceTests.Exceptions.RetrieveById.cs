// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V1
{
    public partial class EventAddressV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressV1Id = GetRandomId();
            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            sqlException.Data.Add(
              key: sqlException.Number,
              value: new List<string> { "Some SQL error code"});

            var failedEventAddressV1StorageException =
                new FailedEventAddressV1StorageException(
                    message: "Failed event address storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventAddressV1DependencyException =
                new EventAddressV1DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: failedStorageEventAddressV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventAddressV1> retrieveEventAddressV1ByIdTask =
                this.eventAddressV1Service.RetrieveEventAddressV1ByIdAsync(
                    someEventAddressV1Id);

            EventAddressV1DependencyException actualEventAddressV1DependencyException =
                await Assert.ThrowsAsync<EventAddressV1DependencyException>(
                    retrieveEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1DependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressV1Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventAddressV1ServiceException =
                new FailedEventAddressV1ServiceException(
                    message: "Failed event address service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventAddressV1ServiceException =
                new EventAddressV1ServiceException(
                    message: "Event address service error occurred, contact support.",
                    innerException: failedEventAddressV1ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventAddressV1> retrieveEventAddressV1ByIdTask =
                this.eventAddressV1Service.RetrieveEventAddressV1ByIdAsync(
                    someEventAddressV1Id);

            EventAddressV1ServiceException actualEventAddressV1ServiceException =
                await Assert.ThrowsAsync<EventAddressV1ServiceException>(
                    retrieveEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1ServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV1ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
