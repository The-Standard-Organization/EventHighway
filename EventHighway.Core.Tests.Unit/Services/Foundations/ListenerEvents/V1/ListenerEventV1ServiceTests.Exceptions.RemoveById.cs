// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V1
{
    public partial class ListenerEventV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();
            SqlException sqlException = GetSqlException();

            var failedStorageListenerEventV1Exception =
                new FailedStorageListenerEventV1Exception(
                    message: "Failed listener event storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedListenerEventV1DependencyException =
                new ListenerEventV1DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1Service.RemoveListenerEventV1ByIdAsync(
                    someListenerEventV1Id);

            ListenerEventV1DependencyException actualListenerEventV1DependencyException =
                await Assert.ThrowsAsync<ListenerEventV1DependencyException>(
                    removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationErrorOnRemoveByIdIfDbUpdateConcurrencyErrorAndLogItAsync()
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedListenerEventV1Exception =
                new LockedListenerEventV1Exception(
                    message: "Listener event is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedListenerEventV1DependencyValidationException =
                new ListenerEventV1DependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: lockedListenerEventV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1Service.RemoveListenerEventV1ByIdAsync(
                    someListenerEventV1Id);

            ListenerEventV1DependencyValidationException actualListenerEventV1DependencyValidationException =
                await Assert.ThrowsAsync<ListenerEventV1DependencyValidationException>(
                    removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1DependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();
            var dbUpdateException = new DbUpdateException();

            var failedStorageListenerEventV1Exception =
                new FailedStorageListenerEventV1Exception(
                    message: "Failed listener event storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedListenerEventV1DependencyException =
                new ListenerEventV1DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1Service.RemoveListenerEventV1ByIdAsync(someListenerEventV1Id);

            ListenerEventV1DependencyException actualListenerEventV1DependencyException =
                await Assert.ThrowsAsync<ListenerEventV1DependencyException>(
                    removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someListenerEventV1Id = GetRandomId();
            var serviceException = new Exception();

            var failedListenerEventV1ServiceException =
                new FailedListenerEventV1ServiceException(
                    message: "Failed listener event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV1ServiceException =
                new ListenerEventV1ServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV1ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV1> removeListenerEventV1ByIdTask =
                this.listenerEventV1Service.RemoveListenerEventV1ByIdAsync(
                    someListenerEventV1Id);

            ListenerEventV1ServiceException actualListenerEventV1ServiceException =
                await Assert.ThrowsAsync<ListenerEventV1ServiceException>(
                    removeListenerEventV1ByIdTask.AsTask);

            // then
            actualListenerEventV1ServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV1ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
