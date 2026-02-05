// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventV1ArchiveIsNullAndLogItAsync()
        {
            // given
            EventV1Archive nullEventV1Archive = null;

            var nullEventV1ArchiveException =
                new NullEventV1ArchiveException(
                    message: "Event archive is null.");

            var expectedEventV1ArchiveValidationException =
                new EventV1ArchiveValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1ArchiveException);

            // when
            ValueTask<EventV1Archive> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventV1ArchiveAsync(nullEventV1Archive);

            EventV1ArchiveValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventV1ArchiveValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventV1Archive>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
