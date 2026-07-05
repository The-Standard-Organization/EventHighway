// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnResetRetriesByEventListenerIdIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();

            var expectedListenerEventV2ClientValidationException =
                new ListenerEventV2ClientValidationException(
                    message: "Listener event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.listenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask resetRetriesForListenerEventV2ByEventListenerV2IdTask =
                this.listenerEventV2Client.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            ListenerEventV2ClientValidationException actualListenerEventV2ClientValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ClientValidationException>(
                    resetRetriesForListenerEventV2ByEventListenerV2IdTask.AsTask);

            // then
            actualListenerEventV2ClientValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientValidationException);

            this.listenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnResetRetriesByEventListenerIdIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var listenerEventV2OrchestrationDependencyException =
                new ListenerEventV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventV2ClientDependencyException =
                new ListenerEventV2ClientDependencyException(
                    message: "Listener event client dependency error occurred, contact support.",

                    innerException: listenerEventV2OrchestrationDependencyException
                        .InnerException as Xeption,

                    data: (listenerEventV2OrchestrationDependencyException
                        .InnerException as Xeption).Data);

            this.listenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(listenerEventV2OrchestrationDependencyException);

            // when
            ValueTask resetRetriesForListenerEventV2ByEventListenerV2IdTask =
                this.listenerEventV2Client.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            ListenerEventV2ClientDependencyException actualListenerEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV2ClientDependencyException>(
                    resetRetriesForListenerEventV2ByEventListenerV2IdTask.AsTask);

            // then
            actualListenerEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientDependencyException);

            this.listenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnResetRetriesByEventListenerIdIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var listenerEventV2OrchestrationServiceException =
                new ListenerEventV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventV2ClientDependencyException =
                new ListenerEventV2ClientDependencyException(
                    message: "Listener event client dependency error occurred, contact support.",

                    innerException: listenerEventV2OrchestrationServiceException
                        .InnerException as Xeption,

                    data: (listenerEventV2OrchestrationServiceException
                        .InnerException as Xeption).Data);

            this.listenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(listenerEventV2OrchestrationServiceException);

            // when
            ValueTask resetRetriesForListenerEventV2ByEventListenerV2IdTask =
                this.listenerEventV2Client.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            ListenerEventV2ClientDependencyException actualListenerEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV2ClientDependencyException>(
                    resetRetriesForListenerEventV2ByEventListenerV2IdTask.AsTask);

            // then
            actualListenerEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientDependencyException);

            this.listenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnResetRetriesByEventListenerIdIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedListenerEventV2ClientServiceException =
                new ListenerEventV2ClientServiceException(
                    message: "Listener event client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.listenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask resetRetriesForListenerEventV2ByEventListenerV2IdTask =
                this.listenerEventV2Client.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            ListenerEventV2ClientServiceException actualListenerEventV2ClientServiceException =
                await Assert.ThrowsAsync<ListenerEventV2ClientServiceException>(
                    resetRetriesForListenerEventV2ByEventListenerV2IdTask.AsTask);

            // then
            actualListenerEventV2ClientServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientServiceException);

            this.listenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnResetRetriesByEventListenerIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();

            var operationCanceledException =
                new OperationCanceledException();

            this.listenerEventV2OrchestrationServiceMock.Setup(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask resetRetriesForListenerEventV2ByEventListenerV2IdTask =
                this.listenerEventV2Client.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    resetRetriesForListenerEventV2ByEventListenerV2IdTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.listenerEventV2OrchestrationServiceMock.Verify(service =>
                service.ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
