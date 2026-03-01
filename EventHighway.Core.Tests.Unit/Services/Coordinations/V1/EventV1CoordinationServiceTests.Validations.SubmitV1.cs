// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSubmitIfEventV1IsNullAndLogItAsyncV1()
        {
            // given
            EventV1 nullEventV1 = null;

            var nullEventV1CoordinationException =
                new NullEventV1CoordinationException(message: "Event is null.");

            var expectedEventV1CoordinationValidationException =
                new EventV1CoordinationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1CoordinationException);

            // when
            ValueTask<EventV1> submitEventV1TaskV1 =
                this.eventV1CoordinationService.SubmitEventV1AsyncV1(nullEventV1);

            EventV1CoordinationValidationException
                actualEventV1CoordinationValidationException =
                    await Assert.ThrowsAsync<EventV1CoordinationValidationException>(
                        submitEventV1TaskV1.AsTask);

            // then
            actualEventV1CoordinationValidationException.Should().BeEquivalentTo(
                expectedEventV1CoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationValidationException))),
                        Times.Once);

            this.eventV1OrchestrationServiceMock.Verify(broker =>
                broker.SubmitEventV1Async(
                    It.IsAny<EventV1>()),
                        Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.AddListenerEventV1Async(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.RunEventCallV1AsyncV1(
                    It.IsAny<EventCallV1>()),
                        Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.ModifyListenerEventV1Async(
                    It.IsAny<ListenerEventV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
