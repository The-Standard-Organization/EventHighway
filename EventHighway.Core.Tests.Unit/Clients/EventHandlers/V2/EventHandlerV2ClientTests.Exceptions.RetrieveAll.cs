// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventHandlers.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventHandlers.V2
{
    public partial class EventHandlerV2ClientTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventHandlerV2ProcessingDependencyException =
                new EventHandlerV2ProcessingDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventHandlerV2ClientDependencyException =
                new EventHandlerV2ClientDependencyException(
                    message: "Event handler client dependency error occurred, contact support.",
                    innerException: eventHandlerV2ProcessingDependencyException.InnerException as Xeption,
                    data: (eventHandlerV2ProcessingDependencyException.InnerException as Xeption).Data);

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventHandlerV2ProcessingDependencyException);

            // when
            ValueTask<IQueryable<EventHandlerV2>> retrieveAllEventHandlerV2sTask =
                this.eventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                    randomCancellationToken);

            EventHandlerV2ClientDependencyException actualEventHandlerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventHandlerV2ClientDependencyException>(
                    retrieveAllEventHandlerV2sTask.AsTask);

            // then
            actualEventHandlerV2ClientDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientDependencyException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventHandlerV2ProcessingServiceException =
                new EventHandlerV2ProcessingServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventHandlerV2ClientDependencyException =
                new EventHandlerV2ClientDependencyException(
                    message: "Event handler client dependency error occurred, contact support.",
                    innerException: eventHandlerV2ProcessingServiceException.InnerException as Xeption,
                    data: (eventHandlerV2ProcessingServiceException.InnerException as Xeption).Data);

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventHandlerV2ProcessingServiceException);

            // when
            ValueTask<IQueryable<EventHandlerV2>> retrieveAllEventHandlerV2sTask =
                this.eventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                    randomCancellationToken);

            EventHandlerV2ClientDependencyException actualEventHandlerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventHandlerV2ClientDependencyException>(
                    retrieveAllEventHandlerV2sTask.AsTask);

            // then
            actualEventHandlerV2ClientDependencyException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientDependencyException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedEventHandlerV2ClientServiceException =
                new EventHandlerV2ClientServiceException(
                    message: "Event handler client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(someXeption);

            // when
            ValueTask<IQueryable<EventHandlerV2>> retrieveAllEventHandlerV2sTask =
                this.eventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                    randomCancellationToken);

            EventHandlerV2ClientServiceException actualEventHandlerV2ClientServiceException =
                await Assert.ThrowsAsync<EventHandlerV2ClientServiceException>(
                    retrieveAllEventHandlerV2sTask.AsTask);

            // then
            actualEventHandlerV2ClientServiceException.Should().BeEquivalentTo(
                expectedEventHandlerV2ClientServiceException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAllAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<EventHandlerV2>> retrieveAllEventHandlerV2sTask =
                this.eventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                    randomCancellationToken);

            OperationCanceledException actualOperationCanceledException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllEventHandlerV2sTask.AsTask);

            // then
            actualOperationCanceledException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
