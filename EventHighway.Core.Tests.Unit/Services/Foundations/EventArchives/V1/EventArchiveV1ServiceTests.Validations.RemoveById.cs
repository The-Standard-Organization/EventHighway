// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventArchiveV1ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventArchiveV1Id = Guid.Empty;

            var invalidEventArchiveV1Exception =
                new InvalidEventArchiveV1Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV1Exception.AddData(
                key: nameof(EventArchiveV1.Id),
                values: "Required");

            var expectedEventArchiveV1ValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV1Exception);

            // when
            ValueTask<EventArchiveV1> removeEventArchiveV1ByIdTask =
                this.eventArchiveV1Service.RemoveEventArchiveV1ByIdAsync(
                    invalidEventArchiveV1Id);

            EventArchiveV1ValidationException actualEventArchiveV1ValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    removeEventArchiveV1ByIdTask.AsTask);

            // then
            actualEventArchiveV1ValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV1ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventArchiveV1IsNotFoundAndLogItAsync()
        {
            // given
            Guid nonExistingEventArchiveV1Id = GetRandomId();
            EventArchiveV1 nullEventArchiveV1 = null;

            var notFoundEventArchiveV1Exception =
                new NotFoundEventArchiveV1Exception(
                    message: $"Could not find event archive with id: {nonExistingEventArchiveV1Id}.");

            var expectedEventArchiveV1ValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventArchiveV1Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV1ByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(nullEventArchiveV1);

            // when
            ValueTask<EventArchiveV1> removeEventArchiveV1ByIdTask =
                this.eventArchiveV1Service.RemoveEventArchiveV1ByIdAsync(nonExistingEventArchiveV1Id);

            EventArchiveV1ValidationException actualEventArchiveV1ValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    removeEventArchiveV1ByIdTask.AsTask);

            // then
            actualEventArchiveV1ValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV1ValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
