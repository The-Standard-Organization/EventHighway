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
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventV1ValidationExceptions))]
        [MemberData(nameof(EventListenerV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnSubmitV1IfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();

            var expectedEventV1CoordinationDependencyValidationException =
                new EventV1CoordinationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.dateTimeBrokerMock.Setup(service =>
                service.GetDateTimeOffsetAsync())
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1CoordinationService.SubmitEventV1AsyncV1(someEventV1);

            EventV1CoordinationDependencyValidationException
                actualEventV1CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV1CoordinationDependencyValidationException>(
                        submitEventV1Task.AsTask);

            // then
            expectedEventV1CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyValidationException))),
                        Times.Once);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()),
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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventV1DependencyExceptions))]
        [MemberData(nameof(EventListenerV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSubmitV1IfDependencyExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();

            var expectedEventV1CoordinationDependencyException =
                new EventV1CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: validationException.InnerException as Xeption);

            this.dateTimeBrokerMock.Setup(service =>
                service.GetDateTimeOffsetAsync())
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1CoordinationService.SubmitEventV1AsyncV1(someEventV1);

            EventV1CoordinationDependencyException
                actualEventV1CoordinationDependencyException =
                    await Assert.ThrowsAsync<EventV1CoordinationDependencyException>(
                        submitEventV1Task.AsTask);

            // then
            expectedEventV1CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyException))),
                        Times.Once);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()),
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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSubmitV1IfExceptionOccursAndLogItAsync()
        {
            // given
            EventV1 someEventV1 = CreateRandomEventV1();
            var serviceException = new Exception();

            var failedEventV1CoordinationServiceException =
                new FailedEventV1CoordinationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV1CoordinationServiceException =
                new EventV1CoordinationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1CoordinationServiceException);

            this.dateTimeBrokerMock.Setup(service =>
                service.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1CoordinationService.SubmitEventV1AsyncV1(someEventV1);

            EventV1CoordinationServiceException
                actualEventV1CoordinationServiceException =
                    await Assert.ThrowsAsync<EventV1CoordinationServiceException>(
                        submitEventV1Task.AsTask);

            // then
            expectedEventV1CoordinationServiceException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationServiceException))),
                        Times.Once);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()),
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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventV1DependencyExceptions))]
        [MemberData(nameof(EventListenerV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSubmitV1IfAllRetryAttemptsAreExhaustedAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            int randomRetryAttempts = GetRandomNumber();
            int inputRetryAttempts = randomRetryAttempts;
            int expectedCallCount = randomRetryAttempts + 1;
            EventV1 someEventV1 = CreateRandomEventV1(inputRetryAttempts);

            var expectedEventV1CoordinationDependencyException =
                new EventV1CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1CoordinationService.SubmitEventV1AsyncV1(someEventV1);

            EventV1CoordinationDependencyException
                actualEventV1CoordinationDependencyException =
                    await Assert.ThrowsAsync<EventV1CoordinationDependencyException>(
                        submitEventV1Task.AsTask);

            // then
            actualEventV1CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(expectedCallCount));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationDependencyException))),
                        Times.Once);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    It.IsAny<Guid>()),
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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSubmitV1IfAllRetryAttemptsAreExhaustedAndLogItAsync()
        {
            // given
            int randomRetryAttempts = GetRandomNumber();
            int inputRetryAttempts = randomRetryAttempts;
            int expectedCallCount = randomRetryAttempts + 1;
            EventV1 someEventV1 = CreateRandomEventV1(inputRetryAttempts);
            var serviceException = new Exception();

            var failedEventV1CoordinationServiceException =
                new FailedEventV1CoordinationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV1CoordinationServiceException =
                new EventV1CoordinationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV1CoordinationServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV1> submitEventV1Task =
                this.eventV1CoordinationService.SubmitEventV1AsyncV1(someEventV1);

            EventV1CoordinationServiceException
                actualEventV1CoordinationServiceException =
                    await Assert.ThrowsAsync<EventV1CoordinationServiceException>(
                        submitEventV1Task.AsTask);

            // then
            actualEventV1CoordinationServiceException.Should()
                .BeEquivalentTo(expectedEventV1CoordinationServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(expectedCallCount));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1CoordinationServiceException))),
                        Times.Once);

            this.eventV1OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV1Async(It.IsAny<EventV1>()),
                    Times.Never);

            this.eventListenerV1OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV1sByEventAddressIdAsync(
                    It.IsAny<Guid>()),
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

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV1OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
