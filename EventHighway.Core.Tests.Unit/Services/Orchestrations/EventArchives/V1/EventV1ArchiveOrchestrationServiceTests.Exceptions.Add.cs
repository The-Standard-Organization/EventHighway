// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventArchiveV1OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventArchiveV1ValidationExceptions))]
        [MemberData(nameof(ListenerEventArchiveV1ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();

            var expectedEventArchiveV1OrchestrationDependencyValidationException =
                new EventArchiveV1OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventArchiveV1ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask addEventArchiveV1Task =
                this.eventArchiveV1OrchestrationService.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                    someEventArchiveV1);

            EventArchiveV1OrchestrationDependencyValidationException
                actualEventArchiveV1OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV1OrchestrationDependencyValidationException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV1OrchestrationDependencyValidationException);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.Verify(broker =>
                broker.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Never);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventArchiveV1DependencyExceptions))]
        [MemberData(nameof(ListenerEventArchiveV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();

            var expectedEventArchiveV1OrchestrationDependencyException =
                new EventArchiveV1OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventArchiveV1ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask addEventArchiveV1Task =
                this.eventArchiveV1OrchestrationService.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                    someEventArchiveV1);

            EventArchiveV1OrchestrationDependencyException
                actualEventArchiveV1OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV1OrchestrationDependencyException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV1OrchestrationDependencyException);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.Verify(broker =>
                broker.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Never);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();
            var exception = new Exception();

            var failedEventArchiveV1OrchestrationServiceException =
                new FailedEventArchiveV1OrchestrationServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: exception);

            var expectedEventArchiveV1OrchestrationServiceException =
                new EventArchiveV1OrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV1OrchestrationServiceException);

            this.listenerEventArchiveV1ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()))
                    .ThrowsAsync(exception);

            // when
            ValueTask addEventArchiveV1Task =
                this.eventArchiveV1OrchestrationService.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                    someEventArchiveV1);

            EventArchiveV1OrchestrationServiceException
                actualEventArchiveV1OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventArchiveV1OrchestrationServiceException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventArchiveV1OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV1OrchestrationServiceException);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV1OrchestrationServiceException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.Verify(broker =>
                broker.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Never);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
        }
    }
}
