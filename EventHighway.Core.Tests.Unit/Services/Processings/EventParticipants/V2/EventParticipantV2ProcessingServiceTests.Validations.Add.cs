// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnAddIfEventParticipantV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 nullEventParticipantV2 = null;

            var nullEventParticipantV2ProcessingException =
                new NullEventParticipantV2ProcessingException(message: "Event participant is null.");

            var expectedEventParticipantV2ProcessingValidationException =
                new EventParticipantV2ProcessingValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: nullEventParticipantV2ProcessingException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2ProcessingService.AddEventParticipantV2Async(
                    nullEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ProcessingValidationException
                actualEventParticipantV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ProcessingValidationException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventParticipantV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ProcessingValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
