// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V1
{
    public partial class EventListenerV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();
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
                broker.SelectEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1Service.RemoveEventListenerV1ByIdAsync(
                    someEventListenerV1Id);

            EventListenerV1DependencyException actualEventListenerV1DependencyException =
                await Assert.ThrowsAsync<EventListenerV1DependencyException>(
                    removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventListenerV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationErrorOnRemoveByIdIfDbUpdateConcurrencyErrorAndLogItAsync()
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedEventListenerV1Exception =
                new LockedEventListenerV1Exception(
                    message: "Event listener is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedEventListenerV1DependencyValidationException =
                new EventListenerV1DependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: lockedEventListenerV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1Service.RemoveEventListenerV1ByIdAsync(
                    someEventListenerV1Id);

            EventListenerV1DependencyValidationException actualEventListenerV1DependencyValidationException =
                await Assert.ThrowsAsync<EventListenerV1DependencyValidationException>(
                    removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1DependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventListenerV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();
            var dbUpdateException = new DbUpdateException();

            var failedStorageEventListenerV1Exception =
                new FailedStorageEventListenerV1Exception(
                    message: "Failed event listener storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedEventListenerV1DependencyException =
                new EventListenerV1DependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: failedStorageEventListenerV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1Service.RemoveEventListenerV1ByIdAsync(someEventListenerV1Id);

            EventListenerV1DependencyException actualEventListenerV1DependencyException =
                await Assert.ThrowsAsync<EventListenerV1DependencyException>(
                    removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventListenerV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEventListenerV1Id = GetRandomId();
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
                broker.SelectEventListenerV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventListenerV1> removeEventListenerV1ByIdTask =
                this.eventListenerV1Service.RemoveEventListenerV1ByIdAsync(
                    someEventListenerV1Id);

            EventListenerV1ServiceException actualEventListenerV1ServiceException =
                await Assert.ThrowsAsync<EventListenerV1ServiceException>(
                    removeEventListenerV1ByIdTask.AsTask);

            // then
            actualEventListenerV1ServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV1ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventListenerV1ByIdAsync(It.IsAny<Guid>()),
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
