// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V1
{
    public partial class EventListenerV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyListenerEventV1IfListenerEventV1IsNullAndLogItAsync()
        {
            // given
            ListenerEventV1 nullListenerEventV1 = null;

            var nullListenerEventV1OrchestrationException =
                new NullListenerEventV1OrchestrationException(message: "Listener event is null.");

            var expectedEventListenerV1OrchestrationValidationException =
                new EventListenerV1OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV1OrchestrationException);

            // when
            ValueTask<ListenerEventV1> modifyListenerEventV1Task =
                this.eventListenerV1OrchestrationService
                    .ModifyListenerEventAsync(nullListenerEventV1);

            EventListenerV1OrchestrationValidationException
                actualEventListenerV1OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventListenerV1OrchestrationValidationException>(
                        modifyListenerEventV1Task.AsTask);

            // then
            actualEventListenerV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventListenerV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV1OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV1ProcessingServiceMock.Verify(broker =>
                broker.ModifyListenerEventAsync(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
