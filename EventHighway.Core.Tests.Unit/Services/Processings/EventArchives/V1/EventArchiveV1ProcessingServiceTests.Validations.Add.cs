// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventArchiveV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventArchiveV1IsNullAndLogItAsync()
        {
            // given
            EventArchiveV1 nullEventArchiveV1 = null;

            var nullEventArchiveV1ProcessingException =
                new NullEventArchiveV1ProcessingException(
                    message: "Event archive is null.");

            var expectedEventArchiveV1ProcessingValidationException =
                new EventArchiveV1ProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV1ProcessingException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1ProcessingService.AddEventArchiveV1Async(nullEventArchiveV1);

            EventArchiveV1ProcessingValidationException
                actualEventArchiveV1ProcessingValidationException =
                    await Assert.ThrowsAsync<EventArchiveV1ProcessingValidationException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1ProcessingValidationException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
