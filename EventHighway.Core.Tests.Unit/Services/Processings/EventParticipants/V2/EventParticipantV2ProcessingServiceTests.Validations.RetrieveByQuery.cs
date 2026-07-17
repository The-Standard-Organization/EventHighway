// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventParticipants.V2
{
    public partial class EventParticipantV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2Query nullEventParticipantV2Query = null;

            var nullEventParticipantV2QueryProcessingException =
                new NullEventParticipantV2QueryProcessingException(
                    message: "Event participant query is null.");

            var expectedEventParticipantV2ProcessingValidationException =
                new EventParticipantV2ProcessingValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: nullEventParticipantV2QueryProcessingException);

            // when
            ValueTask<IQueryable<EventParticipantV2>> retrieveEventParticipantV2sByQueryTask =
                this.eventParticipantV2ProcessingService.RetrieveEventParticipantV2sByQueryAsync(
                    nullEventParticipantV2Query,
                    randomCancellationToken);

            EventParticipantV2ProcessingValidationException
                actualEventParticipantV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ProcessingValidationException>(
                        retrieveEventParticipantV2sByQueryTask.AsTask);

            // then
            actualEventParticipantV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ProcessingValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1001)]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsInvalidAndLogItAsync(
            int invalidTake)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var invalidEventParticipantV2Query = new EventParticipantV2Query
            {
                Skip = -1,
                Take = invalidTake,
                CreatedFrom = randomDateTimeOffset,
                CreatedTo = randomDateTimeOffset.AddMinutes(-1)
            };

            var invalidEventParticipantV2QueryProcessingException =
                new InvalidEventParticipantV2QueryProcessingException(
                    message: "Event participant query is invalid, fix the errors and try again.");

            invalidEventParticipantV2QueryProcessingException.AddData(
                key: nameof(EventParticipantV2Query.Skip),
                values: "Value must be zero or greater");

            invalidEventParticipantV2QueryProcessingException.AddData(
                key: nameof(EventParticipantV2Query.Take),
                values: "Value must be between 1 and 1000");

            invalidEventParticipantV2QueryProcessingException.AddData(
                key: nameof(EventParticipantV2Query.CreatedTo),
                values: $"Date must be after {nameof(EventParticipantV2Query.CreatedFrom)}");

            var expectedEventParticipantV2ProcessingValidationException =
                new EventParticipantV2ProcessingValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2QueryProcessingException);

            // when
            ValueTask<IQueryable<EventParticipantV2>> retrieveEventParticipantV2sByQueryTask =
                this.eventParticipantV2ProcessingService.RetrieveEventParticipantV2sByQueryAsync(
                    invalidEventParticipantV2Query,
                    randomCancellationToken);

            EventParticipantV2ProcessingValidationException
                actualEventParticipantV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ProcessingValidationException>(
                        retrieveEventParticipantV2sByQueryTask.AsTask);

            // then
            actualEventParticipantV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ProcessingValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
