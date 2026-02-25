// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSubmitIfEventV1IsNullAndLogItAsync()
        {
            // given
            EventV1 nullEventV1 = null;

            var nullEventV1OrchestrationException =
                new NullEventV1OrchestrationException(message: "Event is null.");

            var expectedEventV1OrchestrationValidationException =
                new EventV1OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1OrchestrationException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1OrchestrationService.SubmitEventV1Async(nullEventV1);

            EventV1OrchestrationValidationException
                actualEventV1OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV1OrchestrationValidationException>(
                        submitEventV1Task.AsTask);

            // then
            actualEventV1OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventV1OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationValidationException))),
                        Times.Once);

            this.eventAddressV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.eventV1ProcessingServiceMock.Verify(broker =>
                broker.AddEventV1Async(
                    It.IsAny<EventV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventAddressV1IsNotFoundAndLogItAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            Guid nonExistingEventAddressV1Id = someEventV1.EventAddressId;
            EventAddressV1 nullEventAddressV1 = null;

            var notFoundEventAddressV1OrchestrationException =
                new NotFoundEventAddressV1OrchestrationException(
                    message: $"Could not find event address with id: {nonExistingEventAddressV1Id}.");

            var expectedEventV1OrchestrationValidationException =
                new EventV1OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventAddressV1OrchestrationException);

            this.eventAddressV1ProcessingServiceMock.Setup(broker =>
                broker.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(nullEventAddressV1);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1OrchestrationService.SubmitEventV1Async(someEventV1);

            EventV1OrchestrationValidationException actualEventV1OrchestrationValidationException =
                await Assert.ThrowsAsync<EventV1OrchestrationValidationException>(
                    submitEventV1Task.AsTask);

            // then
            actualEventV1OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationValidationException);

            this.eventAddressV1ProcessingServiceMock.Verify(broker =>
                broker.RetrieveEventAddressV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationValidationException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.Verify(broker =>
                broker.AddEventV1Async(
                    It.IsAny<EventV1>()),
                        Times.Never);

            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
