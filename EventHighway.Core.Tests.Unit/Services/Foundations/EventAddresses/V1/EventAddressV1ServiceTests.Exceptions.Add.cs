// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            EventAddressV1 someEventAddressV1 = CreateRandomEventAddressV1();
            SqlException sqlException = CreateSqlException();

            var failedEventAddressV1StorageException =
                new FailedEventAddressV1StorageException(
                    message: "Failed event address storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventAddressV1DependencyException =
                new EventAddressV1DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: failedEventAddressV1StorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventAddressV1> addEventAddressV1Task =
                this.eventAddressV1Service.AddEventAddressAsync(someEventAddressV1);

            EventAddressV1DependencyException actualEventAddressV1DependencyException =
                await Assert.ThrowsAsync<EventAddressV1DependencyException>(
                    addEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1DependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventAddressV1AlreadyExistsAndLogItAsync()
        {
            // given
            string someMessage = GetRandomString();
            EventAddressV1 someEventAddressV1 = CreateRandomEventAddressV1();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsEventAddressV1Exception =
                new AlreadyExistsEventAddressV1Exception(
                    message: "Event address with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedEventAddressV1DependencyValidationException =
                new EventAddressV1DependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventAddressV1Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventAddressV1> addEventAddressV1Task =
                this.eventAddressV1Service.AddEventAddressAsync(someEventAddressV1);

            EventAddressV1DependencyValidationException actualEventAddressV1DependencyValidationException =
                await Assert.ThrowsAsync<EventAddressV1DependencyValidationException>(
                    addEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV1DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            EventAddressV1 someEventAddressV1 = CreateRandomEventAddressV1();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventAddressV1> addEventAddressV1Task =
                this.eventAddressV1Service.AddEventAddressAsync(someEventAddressV1);

            EventAddressV1DependencyException actualEventAddressV1DependencyException =
                await Assert.ThrowsAsync<EventAddressV1DependencyException>(
                    addEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1DependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV1DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventAddressV1 someEventAddressV1 = CreateRandomEventAddressV1();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventAddressV1> addEventAddressV1Task =
                this.eventAddressV1Service.AddEventAddressAsync(someEventAddressV1);

            EventAddressV1ServiceException actualEventAddressV1ServiceException =
                await Assert.ThrowsAsync<EventAddressV1ServiceException>(
                    addEventAddressV1Task.AsTask);

            // then
            actualEventAddressV1ServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV1ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV1ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV1Async(It.IsAny<EventAddressV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
