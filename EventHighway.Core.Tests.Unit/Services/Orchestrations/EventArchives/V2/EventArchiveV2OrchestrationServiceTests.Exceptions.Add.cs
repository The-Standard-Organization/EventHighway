// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventArchiveV2ValidationExceptions))]
        [MemberData(nameof(ListenerEventArchiveV2ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventArchiveV2 someEventArchiveV2 = CreateRandomEventArchiveV2();

            var expectedEventArchiveV2OrchestrationDependencyValidationException =
                new EventArchiveV2OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask addEventArchiveV2Task =
                this.eventArchiveV2OrchestrationService
                    .AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                        someEventArchiveV2,
                        TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyValidationException
                actualEventArchiveV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyValidationException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyValidationException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.Verify(broker =>
                broker.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventArchiveV2DependencyExceptions))]
        [MemberData(nameof(ListenerEventArchiveV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            EventArchiveV2 someEventArchiveV2 = CreateRandomEventArchiveV2();

            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask addEventArchiveV2Task =
                this.eventArchiveV2OrchestrationService
                    .AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                        someEventArchiveV2,
                        TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyException
                actualEventArchiveV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.Verify(broker =>
                broker.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV2 someEventArchiveV2 = CreateRandomEventArchiveV2();
            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventArchiveV2OrchestrationServiceException =
                new FailedEventArchiveV2OrchestrationServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedEventArchiveV2OrchestrationServiceException =
                new EventArchiveV2OrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2OrchestrationServiceException);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            // when
            ValueTask addEventArchiveV2Task =
                this.eventArchiveV2OrchestrationService
                    .AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                        someEventArchiveV2,
                        TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationServiceException
                actualEventArchiveV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationServiceException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationServiceException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationServiceException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.Verify(broker =>
                broker.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
