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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
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
            ValueTask<EventArchiveV1> retrieveEventArchiveV1ByIdTask =
                this.eventArchiveV1Service.RetrieveEventArchiveV1ByIdAsync(
                    invalidEventArchiveV1Id);

            EventArchiveV1ValidationException actualEventArchiveV1ValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    retrieveEventArchiveV1ByIdTask.AsTask);

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
    }
}
