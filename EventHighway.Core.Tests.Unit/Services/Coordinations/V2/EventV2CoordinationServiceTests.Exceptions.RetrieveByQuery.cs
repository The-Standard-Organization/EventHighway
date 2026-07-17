// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRetrieveByQueryIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var inputEventV2Query = new EventV2Query();

            var expectedEventV2CoordinationDependencyValidationException =
                new EventV2CoordinationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveEventV2sByQueryTask =
                this.eventV2CoordinationService.RetrieveEventV2sByQueryAsync(
                    inputEventV2Query,
                    randomCancellationToken);

            EventV2CoordinationDependencyValidationException
                actualEventV2CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV2CoordinationDependencyValidationException>(
                        retrieveEventV2sByQueryTask.AsTask);

            // then
            actualEventV2CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationDependencyValidationException);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationDependencyValidationException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByQueryIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var inputEventV2Query = new EventV2Query();

            var expectedEventV2CoordinationDependencyException =
                new EventV2CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveEventV2sByQueryTask =
                this.eventV2CoordinationService.RetrieveEventV2sByQueryAsync(
                    inputEventV2Query,
                    randomCancellationToken);

            EventV2CoordinationDependencyException
                actualEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<EventV2CoordinationDependencyException>(
                        retrieveEventV2sByQueryTask.AsTask);

            // then
            actualEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationDependencyException);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationDependencyException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByQueryIfTimeoutOccursAndLogItAsync()
        {
            // given
            var inputEventV2Query = new EventV2Query();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutEventV2CoordinationException =
                new TimeoutEventV2CoordinationException(
                    message: "Failed event coordination timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedEventV2CoordinationDependencyException =
                new EventV2CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutEventV2CoordinationException);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveEventV2sByQueryTask =
                this.eventV2CoordinationService.RetrieveEventV2sByQueryAsync(
                    inputEventV2Query,
                    TestContext.Current.CancellationToken);

            EventV2CoordinationDependencyException
                actualEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<EventV2CoordinationDependencyException>(
                        retrieveEventV2sByQueryTask.AsTask);

            // then
            actualEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationDependencyException);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationDependencyException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByQueryAsync()
        {
            // given
            var inputEventV2Query = new EventV2Query();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IQueryable<EventV2>> retrieveEventV2sByQueryTask =
                this.eventV2CoordinationService.RetrieveEventV2sByQueryAsync(
                    inputEventV2Query,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveEventV2sByQueryTask.AsTask);

            actualException.Should().NotBeOfType<EventV2CoordinationDependencyException>();
            actualException.Should().NotBeOfType<EventV2CoordinationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByQueryIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var inputEventV2Query = new EventV2Query();
            var serviceException = new Exception();

            var failedEventV2CoordinationServiceException =
                new FailedEventV2CoordinationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV2CoordinationServiceException =
                new EventV2CoordinationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2CoordinationServiceException);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveEventV2sByQueryTask =
                this.eventV2CoordinationService.RetrieveEventV2sByQueryAsync(
                    inputEventV2Query,
                    randomCancellationToken);

            EventV2CoordinationServiceException
                actualEventV2CoordinationServiceException =
                    await Assert.ThrowsAsync<EventV2CoordinationServiceException>(
                        retrieveEventV2sByQueryTask.AsTask);

            // then
            actualEventV2CoordinationServiceException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationServiceException);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationServiceException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
