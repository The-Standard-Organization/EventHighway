// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V1
{
    public partial class EventV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            SqlException sqlException = GetSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventV1Exception =
                new FailedStorageEventV1Exception(
                    message: "Failed event storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventV1DependencyException =
                new EventV1DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: failedStorageEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1Service.AddEventV1Async(someEventV1);

            EventV1DependencyException actualEventV1DependencyException =
                await Assert.ThrowsAsync<EventV1DependencyException>(
                    addEventV1Task.AsTask);

            // then
            actualEventV1DependencyException.Should()
                .BeEquivalentTo(expectedEventV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventV1AlreadyExistsAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            EventV1 someEventV1 = CreateRandomEventV1();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);
            duplicateKeyException.Data.Add("ErrorCode", new List<string> { "DuplicateKeyError" });

            var alreadyExistsEventV1Exception =
                new AlreadyExistsEventV1Exception(
                    message: "Event with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedEventV1DependencyValidationException =
                new EventV1DependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1Service.AddEventV1Async(someEventV1);

            EventV1DependencyValidationException actualEventV1DependencyValidationException =
                await Assert.ThrowsAsync<EventV1DependencyValidationException>(
                    addEventV1Task.AsTask);

            // then
            actualEventV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            string someMessage = GetRandomString();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(someMessage);

            var invalidReferenceEventV1Exception =
                new InvalidReferenceEventV1Exception(
                    message: "Invalid event reference error occurred.",
                    innerException: foreignKeyConstraintConflictException,
                    data: foreignKeyConstraintConflictException.Data);

            var expectedEventV1DependencyValidationException =
                new EventV1DependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidReferenceEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1Service.AddEventV1Async(someEventV1);

            EventV1DependencyValidationException actualEventV1DependencyValidationException =
                await Assert.ThrowsAsync<EventV1DependencyValidationException>(
                    addEventV1Task.AsTask);

            // then
            actualEventV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            var dbUpdateException = new DbUpdateException();
            dbUpdateException.Data.Add("ErrorCode", new List<string> { "DbUpdateError" });

            var failedStorageEventV1Exception =
                new FailedStorageEventV1Exception(
                    message: "Failed event storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedEventV1DependencyException =
                new EventV1DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: failedStorageEventV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1Service.AddEventV1Async(someEventV1);

            EventV1DependencyException actualEventV1DependencyException =
                await Assert.ThrowsAsync<EventV1DependencyException>(
                    addEventV1Task.AsTask);

            // then
            actualEventV1DependencyException.Should()
                .BeEquivalentTo(expectedEventV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV1ServiceException =
                new FailedEventV1ServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV1ServiceException =
                new EventV1ServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1ServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV1> addEventV1Task =
                this.eventV1Service.AddEventV1Async(someEventV1);

            EventV1ServiceException actualEventV1ServiceException =
                await Assert.ThrowsAsync<EventV1ServiceException>(
                    addEventV1Task.AsTask);

            // then
            actualEventV1ServiceException.Should()
                .BeEquivalentTo(expectedEventV1ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
