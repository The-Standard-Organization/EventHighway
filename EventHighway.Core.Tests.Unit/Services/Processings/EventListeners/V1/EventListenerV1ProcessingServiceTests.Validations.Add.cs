// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnAddIfEventListenerV1IsNullAndLogItAsync()
        {
            // given
            EventListenerV1 nullEventListenerV1 = null;

            var nullEventListenerV1ProcessingException =
                new NullEventListenerV1ProcessingException(message: "Event listener is null.");

            var expectedEventListenerV1ProcessingValidationException =
                new EventListenerV1ProcessingValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: nullEventListenerV1ProcessingException);

            // when
            ValueTask<EventListenerV1> addEventListenerV1Task =
                this.eventListenerV1ProcessingService.AddEventListenerV1Async(nullEventListenerV1);

            EventListenerV1ProcessingValidationException
                actualEventListenerV1ProcessingValidationException =
                    await Assert.ThrowsAsync<EventListenerV1ProcessingValidationException>(
                        addEventListenerV1Task.AsTask);

            // then
            actualEventListenerV1ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1ProcessingValidationException))),
                        Times.Once);

            this.eventListenerV1ServiceMock.Verify(broker =>
                broker.AddEventListenerAsync(
                    It.IsAny<EventListenerV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
