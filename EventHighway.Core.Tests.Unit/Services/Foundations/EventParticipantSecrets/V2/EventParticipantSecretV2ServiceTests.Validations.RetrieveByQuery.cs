// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantSecretV2Query nullEventParticipantSecretV2Query = null;

            var nullEventParticipantSecretV2QueryException =
                new NullEventParticipantSecretV2QueryException(
                    message: "Event participant secret query is null.");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: nullEventParticipantSecretV2QueryException);

            // when
            ValueTask<IReadOnlyList<EventParticipantSecretV2>> retrieveByQueryTask =
                this.eventParticipantSecretV2Service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    nullEventParticipantSecretV2Query,
                    randomCancellationToken);

            EventParticipantSecretV2ValidationException
                actualEventParticipantSecretV2ValidationException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                        retrieveByQueryTask.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantSecretV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
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

            var invalidEventParticipantSecretV2Query = new EventParticipantSecretV2Query
            {
                Skip = -1,
                Take = invalidTake,
                CreatedFrom = randomDateTimeOffset,
                CreatedTo = randomDateTimeOffset.AddMinutes(-1)
            };

            var invalidEventParticipantSecretV2QueryException =
                new InvalidEventParticipantSecretV2QueryException(
                    message: "Event participant secret query is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2QueryException.AddData(
                key: nameof(EventParticipantSecretV2Query.Skip),
                values: "Value must be zero or greater");

            invalidEventParticipantSecretV2QueryException.AddData(
                key: nameof(EventParticipantSecretV2Query.Take),
                values: "Value must be between 1 and 1000");

            invalidEventParticipantSecretV2QueryException.AddData(
                key: nameof(EventParticipantSecretV2Query.CreatedTo),
                values: $"Date must be after {nameof(EventParticipantSecretV2Query.CreatedFrom)}");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2QueryException);

            // when
            ValueTask<IReadOnlyList<EventParticipantSecretV2>> retrieveByQueryTask =
                this.eventParticipantSecretV2Service.RetrieveEventParticipantSecretV2sByQueryAsync(
                    invalidEventParticipantSecretV2Query,
                    randomCancellationToken);

            EventParticipantSecretV2ValidationException
                actualEventParticipantSecretV2ValidationException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                        retrieveByQueryTask.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantSecretV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventParticipantSecretV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
