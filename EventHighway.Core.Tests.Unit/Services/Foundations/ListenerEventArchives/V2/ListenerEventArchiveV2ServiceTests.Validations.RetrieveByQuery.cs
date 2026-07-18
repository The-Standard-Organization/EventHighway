// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByQueryIfQueryIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventArchiveV2Query nullListenerEventArchiveV2Query = null;

            var nullListenerEventArchiveV2QueryException =
                new NullListenerEventArchiveV2QueryException(
                    message: "Listener event archive query is null.");

            var expectedListenerEventArchiveV2ValidationException =
                new ListenerEventArchiveV2ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventArchiveV2QueryException);

            // when
            ValueTask<IReadOnlyList<ListenerEventArchiveV2>> retrieveListenerEventArchiveV2sByQueryTask =
                this.listenerEventArchiveV2Service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    nullListenerEventArchiveV2Query,
                    randomCancellationToken);

            ListenerEventArchiveV2ValidationException
                actualListenerEventArchiveV2ValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ValidationException>(
                        retrieveListenerEventArchiveV2sByQueryTask.AsTask);

            // then
            actualListenerEventArchiveV2ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(
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

            var invalidListenerEventArchiveV2Query = new ListenerEventArchiveV2Query
            {
                Skip = -1,
                Take = invalidTake,
                CreatedFrom = randomDateTimeOffset,
                CreatedTo = randomDateTimeOffset.AddMinutes(-1),
                ArchivedFrom = randomDateTimeOffset,
                ArchivedTo = randomDateTimeOffset.AddMinutes(-1)
            };

            var invalidListenerEventArchiveV2QueryException =
                new InvalidListenerEventArchiveV2QueryException(
                    message: "Listener event archive query is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV2QueryException.AddData(
                key: nameof(ListenerEventArchiveV2Query.Skip),
                values: "Value must be zero or greater");

            invalidListenerEventArchiveV2QueryException.AddData(
                key: nameof(ListenerEventArchiveV2Query.Take),
                values: "Value must be between 1 and 1000");

            invalidListenerEventArchiveV2QueryException.AddData(
                key: nameof(ListenerEventArchiveV2Query.CreatedTo),
                values: $"Date must be after {nameof(ListenerEventArchiveV2Query.CreatedFrom)}");

            invalidListenerEventArchiveV2QueryException.AddData(
                key: nameof(ListenerEventArchiveV2Query.ArchivedTo),
                values: $"Date must be after {nameof(ListenerEventArchiveV2Query.ArchivedFrom)}");

            var expectedListenerEventArchiveV2ValidationException =
                new ListenerEventArchiveV2ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV2QueryException);

            // when
            ValueTask<IReadOnlyList<ListenerEventArchiveV2>> retrieveListenerEventArchiveV2sByQueryTask =
                this.listenerEventArchiveV2Service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    invalidListenerEventArchiveV2Query,
                    randomCancellationToken);

            ListenerEventArchiveV2ValidationException
                actualListenerEventArchiveV2ValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ValidationException>(
                        retrieveListenerEventArchiveV2sByQueryTask.AsTask);

            // then
            actualListenerEventArchiveV2ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
