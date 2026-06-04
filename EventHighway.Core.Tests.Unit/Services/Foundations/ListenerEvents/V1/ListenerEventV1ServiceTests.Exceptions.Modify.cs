// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            ListenerEventV1 someListenerEventV1 = CreateRandomListenerEventV1();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(
                    someListenerEventV1);

            ListenerEventV1DependencyException actualListenerEventV1DependencyException =
                await Assert.ThrowsAsync<ListenerEventV1DependencyException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1DependencyException.Should().BeEquivalentTo(
                expectedListenerEventV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            ListenerEventV1 someListenerEventV1 = CreateRandomListenerEventV1();
            string someMessage = GetRandomString();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(someMessage);

            var invalidReferenceListenerEventV1Exception =
                new InvalidReferenceListenerEventV1Exception(
                    message: "Invalid listener event reference error occurred.",
                    innerException: foreignKeyConstraintConflictException,
                    data: foreignKeyConstraintConflictException.Data);

            var expectedListenerEventV1DependencyValidationException =
                new ListenerEventV1DependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidReferenceListenerEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(someListenerEventV1);

            ListenerEventV1DependencyValidationException actualListenerEventV1DependencyValidationException =
                await Assert.ThrowsAsync<ListenerEventV1DependencyValidationException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateListenerEventV1Async(It.IsAny<ListenerEventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationErrorOnModifyIfDatabaseUpdateConcurrencyOccursAndLogItAsync()
        {
            // given
            ListenerEventV1 someListenerEventV1 = CreateRandomListenerEventV1();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(someListenerEventV1);

            ListenerEventV1DependencyValidationException actualListenerEventV1DependencyValidationException =
                await Assert.ThrowsAsync<ListenerEventV1DependencyValidationException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1DependencyValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventV1 someListenerEventV1 = CreateRandomListenerEventV1();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(someListenerEventV1);

            ListenerEventV1DependencyException actualListenerEventV1DependencyException =
                await Assert.ThrowsAsync<ListenerEventV1DependencyException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1DependencyException.Should().BeEquivalentTo(
                expectedListenerEventV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventV1 someListenerEventV1 = CreateRandomListenerEventV1();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.listenerEventV1Service.ModifyListenerEventV1Async(someListenerEventV1);

            ListenerEventV1ServiceException actualListenerEventV1ServiceException =
                await Assert.ThrowsAsync<ListenerEventV1ServiceException>(
                    modifyListenerEventV1Task.AsTask);

            // then
            actualListenerEventV1ServiceException.Should().BeEquivalentTo(
                expectedListenerEventV1ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
