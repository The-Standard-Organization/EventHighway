// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V1
{
    public partial class EventAddressV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressV1Id = GetRandomId();
            SqlException sqlException = CreateSqlException();

            sqlException.Data.Add(
              key: sqlException.Number,
              value: new List<string> { "Some SQL error code" });

            var failedEventAddressV1StorageException =
                new FailedEventAddressV1StorageException(
                    message: "Failed event address storage error occurred, contact support.",
                    innerException: sqlException, 
                    data: sqlException.Data);

            var expectedEventAddressV1DependencyException =
                new EventAddressV1DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: failedEventAddressV1StorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventAddressV1> removeEventAddressV1ByIdTask =
                this.eventAddressV1Service.RemoveEventAddressV1ByIdAsync(
                    someEventAddressV1Id);

            EventAddressV1DependencyException actualEventAddressV1DependencyException =
                await Assert.ThrowsAsync<EventAddressV1DependencyException>(
                    removeEventAddressV1ByIdTask.AsTask);

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
        public async Task ShouldThrowDependencyValidationErrorOnRemoveByIdIfDbUpdateConcurrencyErrorAndLogItAsync()
        {
            // given
            Guid someEventAddressV1Id = GetRandomId();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedEventAddressV1Exception =
                new LockedEventAddressV1Exception(
                    message: "Event address is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedEventAddressV1DependencyValidationException =
                new EventAddressV1DependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: lockedEventAddressV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<EventAddressV1> removeEventAddressV1ByIdTask =
                this.eventAddressV1Service.RemoveEventAddressV1ByIdAsync(
                    someEventAddressV1Id);

            EventAddressV1DependencyValidationException actualEventAddressV1DependencyValidationException =
                await Assert.ThrowsAsync<EventAddressV1DependencyValidationException>(
                    removeEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV1DependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressV1Id = GetRandomId();
            var dbUpdateException = new DbUpdateException();

            var failedEventAddressV1StorageException =
                new FailedEventAddressV1StorageException(
                    message: "Failed event address storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedEventAddressV1DependencyException =
                new EventAddressV1DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: failedEventAddressV1StorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventAddressV1> removeEventAddressV1ByIdTask =
                this.eventAddressV1Service.RemoveEventAddressV1ByIdAsync(someEventAddressV1Id);

            EventAddressV1DependencyException actualEventAddressV1DependencyException =
                await Assert.ThrowsAsync<EventAddressV1DependencyException>(
                    removeEventAddressV1ByIdTask.AsTask);

            // then
            actualEventAddressV1DependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressV1Id = GetRandomId();
            var serviceException = new Exception();

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
            ValueTask<EventAddressV1> removeEventAddressV1ByIdTask =
                this.eventAddressV1Service.RemoveEventAddressV1ByIdAsync(
                    someEventAddressV1Id);

            EventAddressV1ServiceException actualEventAddressV1ServiceException =
                await Assert.ThrowsAsync<EventAddressV1ServiceException>(
                    removeEventAddressV1ByIdTask.AsTask);

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
