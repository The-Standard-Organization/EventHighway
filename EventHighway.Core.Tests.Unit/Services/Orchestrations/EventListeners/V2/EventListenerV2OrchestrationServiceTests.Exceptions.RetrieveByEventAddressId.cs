// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRetrieveByEventAddressIdIfValidationErrorAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventListenerV2ProcessingValidationException =
                new EventListenerV2ProcessingValidationException(
                    someMessage,
                    someInnerException);

            var expectedEventListenerV2OrchestrationDependencyValidationException =
                new EventListenerV2OrchestrationDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: eventListenerV2ProcessingValidationException.InnerException as Xeption);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2ProcessingValidationException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveTask =
                this.eventListenerV2OrchestrationService.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    randomCancellationToken);

            EventListenerV2OrchestrationDependencyValidationException
                actualEventListenerV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyValidationException>(
                        retrieveTask.AsTask);

            // then
            actualEventListenerV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyValidationException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventListenerV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdIfDependencyErrorOccursAndLogItAsync(
            Xeption eventListenerV2DependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();

            var expectedEventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: eventListenerV2DependencyException.InnerException as Xeption);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2DependencyException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveTask =
                this.eventListenerV2OrchestrationService.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    randomCancellationToken);

            EventListenerV2OrchestrationDependencyException
                actualEventListenerV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyException>(
                        retrieveTask.AsTask);

            // then
            actualEventListenerV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByEventAddressIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventListenerV2OrchestrationServiceException =
                new FailedEventListenerV2OrchestrationServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV2OrchestrationServiceException =
                new EventListenerV2OrchestrationServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV2OrchestrationServiceException);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveTask =
                this.eventListenerV2OrchestrationService.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    randomCancellationToken);

            EventListenerV2OrchestrationServiceException
                actualEventListenerV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventListenerV2OrchestrationServiceException>(
                        retrieveTask.AsTask);

            // then
            actualEventListenerV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationServiceException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationServiceException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
