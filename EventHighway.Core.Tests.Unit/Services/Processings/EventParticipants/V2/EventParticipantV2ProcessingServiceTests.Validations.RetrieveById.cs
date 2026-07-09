// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventParticipants.V2
{
    public partial class EventParticipantV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventParticipantV2Id = Guid.Empty;

            var invalidEventParticipantV2ProcessingException =
                new InvalidEventParticipantV2ProcessingException(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2ProcessingException.AddData(
                key: nameof(EventParticipantV2.Id),
                values: "Required");

            var expectedEventParticipantV2ProcessingValidationException =
                new EventParticipantV2ProcessingValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2ProcessingException);

            // when
            ValueTask<EventParticipantV2> retrieveEventParticipantV2ByIdTask =
                this.eventParticipantV2ProcessingService
                    .RetrieveEventParticipantV2ByIdAsync(
                        invalidEventParticipantV2Id,
                        randomCancellationToken);

            EventParticipantV2ProcessingValidationException
                actualEventParticipantV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ProcessingValidationException>(
                        retrieveEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ProcessingValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
