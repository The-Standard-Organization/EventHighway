// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventArchiveV2Query nullEventArchiveV2Query = null;

            var nullEventArchiveV2QueryException =
                new NullEventArchiveV2QueryException(
                    message: "Event archive query is null.");

            var expectedEventArchiveV2ValidationException =
                new EventArchiveV2ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV2QueryException);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveEventArchiveV2sByQueryTask =
                this.eventArchiveV2Service.RetrieveEventArchiveV2sByQueryAsync(
                    nullEventArchiveV2Query,
                    randomCancellationToken);

            EventArchiveV2ValidationException
                actualEventArchiveV2ValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2ValidationException>(
                        retrieveEventArchiveV2sByQueryTask.AsTask);

            // then
            actualEventArchiveV2ValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
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

            var invalidEventArchiveV2Query = new EventArchiveV2Query
            {
                Skip = -1,
                Take = invalidTake,
                CreatedFrom = randomDateTimeOffset,
                CreatedTo = randomDateTimeOffset.AddMinutes(-1),
                ArchivedFrom = randomDateTimeOffset,
                ArchivedTo = randomDateTimeOffset.AddMinutes(-1)
            };

            var invalidEventArchiveV2QueryException =
                new InvalidEventArchiveV2QueryException(
                    message: "Event archive query is invalid, fix the errors and try again.");

            invalidEventArchiveV2QueryException.AddData(
                key: nameof(EventArchiveV2Query.Skip),
                values: "Value must be zero or greater");

            invalidEventArchiveV2QueryException.AddData(
                key: nameof(EventArchiveV2Query.Take),
                values: "Value must be between 1 and 1000");

            invalidEventArchiveV2QueryException.AddData(
                key: nameof(EventArchiveV2Query.CreatedTo),
                values: $"Date must be after {nameof(EventArchiveV2Query.CreatedFrom)}");

            invalidEventArchiveV2QueryException.AddData(
                key: nameof(EventArchiveV2Query.ArchivedTo),
                values: $"Date must be after {nameof(EventArchiveV2Query.ArchivedFrom)}");

            var expectedEventArchiveV2ValidationException =
                new EventArchiveV2ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV2QueryException);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveEventArchiveV2sByQueryTask =
                this.eventArchiveV2Service.RetrieveEventArchiveV2sByQueryAsync(
                    invalidEventArchiveV2Query,
                    randomCancellationToken);

            EventArchiveV2ValidationException
                actualEventArchiveV2ValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2ValidationException>(
                        retrieveEventArchiveV2sByQueryTask.AsTask);

            // then
            actualEventArchiveV2ValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
