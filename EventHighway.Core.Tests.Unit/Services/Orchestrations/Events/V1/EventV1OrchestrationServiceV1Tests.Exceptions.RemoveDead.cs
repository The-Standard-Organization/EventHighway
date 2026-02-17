// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceV1Tests
    {
        [Theory]
        [MemberData(nameof(EventV1ValidationExceptions))]
        [MemberData(nameof(ListenerEventV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveDeadIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();

            var expectedEventV1OrchestrationDependencyValidationException =
                new EventV1OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventV1ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask removeEventV1AndListenerEventV1sTask =
                this.eventV1OrchestrationServiceV1.RemoveEventV1AndListenerEventV1sAsync(
                    someEventV1);

            EventV1OrchestrationDependencyValidationException
                actualEventV1OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1OrchestrationDependencyValidationException>(
                        removeEventV1AndListenerEventV1sTask.AsTask);

            // then
            actualEventV1OrchestrationDependencyValidationException.Should().BeEquivalentTo(
                expectedEventV1OrchestrationDependencyValidationException);

            this.listenerEventV1ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.Verify(service =>
                service.RemoveEventV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.listenerEventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
