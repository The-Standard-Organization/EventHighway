// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V1
{
    public partial class EventV1OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventV1ValidationExceptions))]
        [MemberData(nameof(EventAddressV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnSubmitIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();

            var expectedEventV1OrchestrationDependencyValidationException =
                new EventV1OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventAddressV1ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1OrchestrationService.SubmitEventV1Async(
                    someEventV1);

            EventV1OrchestrationDependencyValidationException
                actualEventV1OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1OrchestrationDependencyValidationException>(
                        submitEventV1Task.AsTask);

            // then
            actualEventV1OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationDependencyValidationException);

            this.eventAddressV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.Verify(broker =>
                broker.AddEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventV1DependencyExceptions))]
        [MemberData(nameof(EventAddressV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSubmitIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();

            var expectedEventV1OrchestrationDependencyException =
                new EventV1OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV1ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1OrchestrationService.SubmitEventV1Async(
                    someEventV1);

            EventV1OrchestrationDependencyException
                actualEventV1OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventV1OrchestrationDependencyException>(
                        submitEventV1Task.AsTask);

            // then
            actualEventV1OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationDependencyException);

            this.eventAddressV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationDependencyException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.Verify(broker =>
                broker.AddEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSubmitIfExceptionOccursAndLogItAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV1OrchestrationServiceException =
                new FailedEventV1OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV1OrchestrationServiceException =
                new EventV1OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1OrchestrationServiceException);

            this.eventAddressV1ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1OrchestrationService.SubmitEventV1Async(
                    someEventV1);

            EventV1OrchestrationServiceException
                actualEventV1OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventV1OrchestrationServiceException>(
                        submitEventV1Task.AsTask);

            // then
            actualEventV1OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventV1OrchestrationServiceException);

            this.eventAddressV1ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV1ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1OrchestrationServiceException))),
                        Times.Once);

            this.eventV1ProcessingServiceMock.Verify(broker =>
                broker.AddEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.eventAddressV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV1ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
