// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventArchiveV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();
            SqlException sqlException = CreateSqlException();

            var failedEventArchiveV1StorageException =
                new FailedEventArchiveV1StorageException(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedEventArchiveV1DependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedEventArchiveV1StorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1Service.AddEventArchiveV1Async(someEventArchiveV1);

            EventArchiveV1DependencyException actualEventArchiveV1DependencyException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyException>(
                    addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1DependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventArchiveV1AlreadyExistsAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            var alreadyExistsEventArchiveV1Exception =
                new AlreadyExistsEventArchiveV1Exception(
                    message: "Event archive with the same id already exists.",
                    innerException: duplicateKeyException);

            var expectedEventArchiveV1DependencyValidationException =
                new EventArchiveV1DependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventArchiveV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1Service.AddEventArchiveV1Async(someEventArchiveV1);

            EventArchiveV1DependencyValidationException actualEventArchiveV1DependencyValidationException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyValidationException>(
                    addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();
            string someMessage = GetRandomString();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(someMessage);

            var invalidEventArchiveV1ReferenceException =
                new InvalidEventArchiveV1ReferenceException(
                    message: "Invalid event archive reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            var expectedEventArchiveV1DependencyValidationException =
                new EventArchiveV1DependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV1ReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1Service.AddEventArchiveV1Async(someEventArchiveV1);

            EventArchiveV1DependencyValidationException actualEventArchiveV1DependencyValidationException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyValidationException>(
                    addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();
            var dbUpdateException = new DbUpdateException();

            var failedEventArchiveV1StorageException =
                new FailedEventArchiveV1StorageException(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedEventArchiveV1DependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedEventArchiveV1StorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1Service.AddEventArchiveV1Async(someEventArchiveV1);

            EventArchiveV1DependencyException actualEventArchiveV1DependencyException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyException>(
                    addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1DependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();
            var serviceException = new Exception();

            var failedEventArchiveV1ServiceException =
                new FailedEventArchiveV1ServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventArchiveV1ServiceException =
                new EventArchiveV1ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV1ServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1Service.AddEventArchiveV1Async(someEventArchiveV1);

            EventArchiveV1ServiceException actualEventArchiveV1ServiceException =
                await Assert.ThrowsAsync<EventArchiveV1ServiceException>(
                    addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1ServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV1ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
