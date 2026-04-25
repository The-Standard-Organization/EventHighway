// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Processings.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V1
{
    public partial class EventListenerV1ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationOnRetrieveByEventAddressIdIfEventAddressIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventAddressId = Guid.Empty;

            var invalidEventListenerV1ProcessingException =
                new InvalidEventListenerV1ProcessingException(
                    message: "Event listener is invalid, fix the errors and try again.");

            invalidEventListenerV1ProcessingException.AddData(
                key: nameof(EventListenerV1.EventAddressId),
                values: "Required");

            var expectedEventListenerV1ProcessingValidationException =
                new EventListenerV1ProcessingValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: invalidEventListenerV1ProcessingException);

            // when
            ValueTask<IQueryable<EventListenerV1>> retrieveEventListenerV1sByEventAddressIdTask =
                this.eventListenerV1ProcessingService.RetrieveEventListenersByEventAddressIdAsync(
                    invalidEventAddressId);

            EventListenerV1ProcessingValidationException actualEventListenerV1ProcessingValidationException =
                await Assert.ThrowsAsync<EventListenerV1ProcessingValidationException>(
                    retrieveEventListenerV1sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV1ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ProcessingValidationException))),
                        Times.Once);

            this.eventListenerV1ServiceMock.Verify(service =>
                service.RetrieveAllEventListenersAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
