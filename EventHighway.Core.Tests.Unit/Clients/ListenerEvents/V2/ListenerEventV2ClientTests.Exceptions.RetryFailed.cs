// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Theory]
        [MemberData(nameof(RetryValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetryFailedIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventV2ClientValidationException =
                new ListenerEventV2ClientValidationException(
                    message: "Listener event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.retryingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask retryFailedListenerEventV2sTask =
                this.listenerEventV2Client.RetryFailedListenerEventV2sAsync(
                    randomCancellationToken);

            ListenerEventV2ClientValidationException actualListenerEventV2ClientValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ClientValidationException>(
                    retryFailedListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ClientValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientValidationException);

            this.retryingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.retryingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetryFailedIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var retryingListenerEventV2OrchestrationDependencyException =
                new RetryingListenerEventV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventV2ClientDependencyException =
                new ListenerEventV2ClientDependencyException(
                    message: "Listener event client dependency error occurred, contact support.",

                    innerException: retryingListenerEventV2OrchestrationDependencyException
                        .InnerException as Xeption,

                    data: (retryingListenerEventV2OrchestrationDependencyException
                        .InnerException as Xeption).Data);

            this.retryingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(retryingListenerEventV2OrchestrationDependencyException);

            // when
            ValueTask retryFailedListenerEventV2sTask =
                this.listenerEventV2Client.RetryFailedListenerEventV2sAsync(
                    randomCancellationToken);

            ListenerEventV2ClientDependencyException actualListenerEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV2ClientDependencyException>(
                    retryFailedListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientDependencyException);

            this.retryingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.retryingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetryFailedIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var retryingListenerEventV2OrchestrationServiceException =
                new RetryingListenerEventV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventV2ClientDependencyException =
                new ListenerEventV2ClientDependencyException(
                    message: "Listener event client dependency error occurred, contact support.",

                    innerException: retryingListenerEventV2OrchestrationServiceException
                        .InnerException as Xeption,

                    data: (retryingListenerEventV2OrchestrationServiceException
                        .InnerException as Xeption).Data);

            this.retryingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(retryingListenerEventV2OrchestrationServiceException);

            // when
            ValueTask retryFailedListenerEventV2sTask =
                this.listenerEventV2Client.RetryFailedListenerEventV2sAsync(
                    randomCancellationToken);

            ListenerEventV2ClientDependencyException actualListenerEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV2ClientDependencyException>(
                    retryFailedListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientDependencyException);

            this.retryingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.retryingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetryFailedIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedListenerEventV2ClientServiceException =
                new ListenerEventV2ClientServiceException(
                    message: "Listener event client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.retryingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask retryFailedListenerEventV2sTask =
                this.listenerEventV2Client.RetryFailedListenerEventV2sAsync(
                    randomCancellationToken);

            ListenerEventV2ClientServiceException actualListenerEventV2ClientServiceException =
                await Assert.ThrowsAsync<ListenerEventV2ClientServiceException>(
                    retryFailedListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ClientServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientServiceException);

            this.retryingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.retryingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetryFailedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.retryingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask retryFailedListenerEventV2sTask =
                this.listenerEventV2Client.RetryFailedListenerEventV2sAsync(
                    randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retryFailedListenerEventV2sTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.retryingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetryFailedListenerEventV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.retryingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
