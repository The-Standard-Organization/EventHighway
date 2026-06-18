// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListenerArchives.V2
{
    public partial class EventListenerArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            EventListenerArchiveV2 someEventListenerArchiveV2 = CreateRandomEventListenerArchiveV2();
            SqlException sqlException = CreateSqlException();

            sqlException.Data.Add(
                key: nameof(SqlException.Number),
                value: new List<string> { "Some SQL error code" });

            var failedStorageEventListenerArchiveV2Exception =
                new FailedStorageEventListenerArchiveV2Exception(
                    message: "Failed event listener archive storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventListenerArchiveV2DependencyException =
                new EventListenerArchiveV2DependencyException(
                    message: "Event listener archive dependency error occurred, contact support.",
                    innerException: failedStorageEventListenerArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventListenerArchiveV2> addEventListenerArchiveV2Task =
                this.eventListenerArchiveV2Service.AddEventListenerArchiveV2Async(
                    someEventListenerArchiveV2,
                    TestContext.Current.CancellationToken);

            EventListenerArchiveV2DependencyException actualEventListenerArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventListenerArchiveV2DependencyException>(
                    addEventListenerArchiveV2Task.AsTask);

            // then
            actualEventListenerArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    It.IsAny<EventListenerArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyValidationExceptionOnAddIfEventListenerArchiveV2AlreadyExistsAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            EventListenerArchiveV2 someEventListenerArchiveV2 = CreateRandomEventListenerArchiveV2();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            duplicateKeyException.Data.Add(
                "ErrorCode",
                new List<string> { "DuplicateKeyError" });

            var alreadyExistsEventListenerArchiveV2Exception =
                new AlreadyExistsEventListenerArchiveV2Exception(
                    message: "Event listener archive with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedEventListenerArchiveV2DependencyValidationException =
                new EventListenerArchiveV2DependencyValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventListenerArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventListenerArchiveV2> addEventListenerArchiveV2Task =
                this.eventListenerArchiveV2Service.AddEventListenerArchiveV2Async(
                    someEventListenerArchiveV2,
                    TestContext.Current.CancellationToken);

            EventListenerArchiveV2DependencyValidationException
                actualEventListenerArchiveV2DependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerArchiveV2DependencyValidationException>(
                        addEventListenerArchiveV2Task.AsTask);

            // then
            actualEventListenerArchiveV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    It.IsAny<EventListenerArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            EventListenerArchiveV2 someEventListenerArchiveV2 = CreateRandomEventListenerArchiveV2();
            string someMessage = GetRandomString();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(someMessage);

            foreignKeyConstraintConflictException.Data.Add(
                "ErrorCode",
                new List<string> { "ForeignKeyConstraintConflictExceptionError" });

            var invalidReferenceEventListenerArchiveV2Exception =
                new InvalidReferenceEventListenerArchiveV2Exception(
                    message: "Invalid event listener archive reference error occurred.",
                    innerException: foreignKeyConstraintConflictException,
                    data: foreignKeyConstraintConflictException.Data);

            var expectedEventListenerArchiveV2DependencyValidationException =
                new EventListenerArchiveV2DependencyValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: invalidReferenceEventListenerArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<EventListenerArchiveV2> addEventListenerArchiveV2Task =
                this.eventListenerArchiveV2Service.AddEventListenerArchiveV2Async(
                    someEventListenerArchiveV2,
                    TestContext.Current.CancellationToken);

            EventListenerArchiveV2DependencyValidationException
                actualEventListenerArchiveV2DependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerArchiveV2DependencyValidationException>(
                        addEventListenerArchiveV2Task.AsTask);

            // then
            actualEventListenerArchiveV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    It.IsAny<EventListenerArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDbUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            EventListenerArchiveV2 someEventListenerArchiveV2 = CreateRandomEventListenerArchiveV2();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            dbUpdateConcurrencyException.Data.Add(
                "ErrorCode",
                new List<string> { "DbUpdateConcurrencyError" });

            var lockedEventListenerArchiveV2Exception =
                new LockedEventListenerArchiveV2Exception(
                    message: "Event listener archive is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedEventListenerArchiveV2DependencyValidationException =
                new EventListenerArchiveV2DependencyValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: lockedEventListenerArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<EventListenerArchiveV2> addEventListenerArchiveV2Task =
                this.eventListenerArchiveV2Service.AddEventListenerArchiveV2Async(
                    someEventListenerArchiveV2,
                    TestContext.Current.CancellationToken);

            EventListenerArchiveV2DependencyValidationException
                actualEventListenerArchiveV2DependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerArchiveV2DependencyValidationException>(
                        addEventListenerArchiveV2Task.AsTask);

            // then
            actualEventListenerArchiveV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    It.IsAny<EventListenerArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            EventListenerArchiveV2 someEventListenerArchiveV2 = CreateRandomEventListenerArchiveV2();
            var dbUpdateException = new DbUpdateException();

            dbUpdateException.Data.Add(
                "ErrorCode",
                new List<string> { "DbUpdateError" });

            var failedStorageEventListenerArchiveV2Exception =
                new FailedStorageEventListenerArchiveV2Exception(
                    message: "Failed event listener archive storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedEventListenerArchiveV2DependencyException =
                new EventListenerArchiveV2DependencyException(
                    message: "Event listener archive dependency error occurred, contact support.",
                    innerException: failedStorageEventListenerArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventListenerArchiveV2> addEventListenerArchiveV2Task =
                this.eventListenerArchiveV2Service.AddEventListenerArchiveV2Async(
                    someEventListenerArchiveV2,
                    TestContext.Current.CancellationToken);

            EventListenerArchiveV2DependencyException actualEventListenerArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventListenerArchiveV2DependencyException>(
                    addEventListenerArchiveV2Task.AsTask);

            // then
            actualEventListenerArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    It.IsAny<EventListenerArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventListenerArchiveV2 someEventListenerArchiveV2 = CreateRandomEventListenerArchiveV2();
            var serviceException = new Exception();

            serviceException.Data.Add(
                "ErrorCode",
                new List<string> { "ServiceError" });

            var failedEventListenerArchiveV2ServiceException =
                new FailedEventListenerArchiveV2ServiceException(
                    message: "Failed event listener archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerArchiveV2ServiceException =
                new EventListenerArchiveV2ServiceException(
                    message: "Event listener archive service error occurred, contact support.",
                    innerException: failedEventListenerArchiveV2ServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventListenerArchiveV2> addEventListenerArchiveV2Task =
                this.eventListenerArchiveV2Service.AddEventListenerArchiveV2Async(
                    someEventListenerArchiveV2,
                    TestContext.Current.CancellationToken);

            EventListenerArchiveV2ServiceException actualEventListenerArchiveV2ServiceException =
                await Assert.ThrowsAsync<EventListenerArchiveV2ServiceException>(
                    addEventListenerArchiveV2Task.AsTask);

            // then
            actualEventListenerArchiveV2ServiceException.Should()
                .BeEquivalentTo(expectedEventListenerArchiveV2ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerArchiveV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventListenerArchiveV2Async(
                    It.IsAny<EventListenerArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
