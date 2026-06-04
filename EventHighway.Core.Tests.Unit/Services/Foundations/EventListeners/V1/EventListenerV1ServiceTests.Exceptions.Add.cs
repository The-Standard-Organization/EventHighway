// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            EventListenerV1 someEventListenerV1 = CreateRandomEventListenerV1();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1Service.AddEventListenerV1Async(someEventListenerV1);

            EventListenerV1DependencyException actualEventListenerV1DependencyException =
                await Assert.ThrowsAsync<EventListenerV1DependencyException>(
                    addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(It.IsAny<EventListenerV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventListenerV1AlreadyExistsAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            EventListenerV1 someEventListenerV1 = CreateRandomEventListenerV1();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            var alreadyExistsEventListenerV1Exception =
                new AlreadyExistsEventListenerV1Exception(
                    message: "Event listener with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedEventListenerV1DependencyValidationException =
                new EventListenerV1DependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventListenerV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1Service.AddEventListenerV1Async(someEventListenerV1);

            EventListenerV1DependencyValidationException actualEventListenerV1DependencyValidationException =
                await Assert.ThrowsAsync<EventListenerV1DependencyValidationException>(
                    addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(It.IsAny<EventListenerV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            EventListenerV1 someEventListenerV1 = CreateRandomEventListenerV1();
            string someMessage = GetRandomString();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(someMessage);

            var invalidReferenceEventListenerV1Exception =
                new InvalidReferenceEventListenerV1Exception(
                    message: "Invalid event listener reference error occurred.",
                    innerException: foreignKeyConstraintConflictException,
                    data: foreignKeyConstraintConflictException.Data);

            var expectedEventListenerV1DependencyValidationException =
                new EventListenerV1DependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidReferenceEventListenerV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1Service.AddEventListenerV1Async(someEventListenerV1);

            EventListenerV1DependencyValidationException actualEventListenerV1DependencyValidationException =
                await Assert.ThrowsAsync<EventListenerV1DependencyValidationException>(
                    addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(It.IsAny<EventListenerV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            EventListenerV1 someEventListenerV1 = CreateRandomEventListenerV1();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1Service.AddEventListenerV1Async(someEventListenerV1);

            EventListenerV1DependencyException actualEventListenerV1DependencyException =
                await Assert.ThrowsAsync<EventListenerV1DependencyException>(
                    addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(It.IsAny<EventListenerV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventListenerV1 someEventListenerV1 = CreateRandomEventListenerV1();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1Service.AddEventListenerV1Async(someEventListenerV1);

            EventListenerV1ServiceException actualEventListenerV1ServiceException =
                await Assert.ThrowsAsync<EventListenerV1ServiceException>(
                    addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1ServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV1ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerV1Async(It.IsAny<EventListenerV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
