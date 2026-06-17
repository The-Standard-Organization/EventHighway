// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventListeners.V2
{
    public partial class EventListenerV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveOrRegisterIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerV2 someEventListenerV2 = CreateRandomEventListenerV2();

            var expectedEventListenerV2ClientValidationException =
                new EventListenerV2ClientValidationException(
                    message: "Event listener client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveOrRegisterEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventListenerV2> retrieveOrRegisterEventListenerV2Task =
                this.eventListenerV2Client.RetrieveOrRegisterEventListenerV2Async(
                    someEventListenerV2,
                    randomCancellationToken);

            EventListenerV2ClientValidationException actualEventListenerV2ClientValidationException =
                await Assert.ThrowsAsync<EventListenerV2ClientValidationException>(
                    retrieveOrRegisterEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientValidationException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveOrRegisterEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveOrRegisterIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerV2 someEventListenerV2 = CreateRandomEventListenerV2();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventListenerV2ClientDependencyException =
                new EventListenerV2ClientDependencyException(
                    message: "Event listener client dependency error occurred, contact support.",
                    innerException: eventListenerV2OrchestrationDependencyException.InnerException as Xeption,
                    data: (eventListenerV2OrchestrationDependencyException.InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveOrRegisterEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2OrchestrationDependencyException);

            // when
            ValueTask<EventListenerV2> retrieveOrRegisterEventListenerV2Task =
                this.eventListenerV2Client.RetrieveOrRegisterEventListenerV2Async(
                    someEventListenerV2,
                    randomCancellationToken);

            EventListenerV2ClientDependencyException actualEventListenerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventListenerV2ClientDependencyException>(
                    retrieveOrRegisterEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientDependencyException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveOrRegisterEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveOrRegisterIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventListenerV2 someEventListenerV2 = CreateRandomEventListenerV2();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventListenerV2OrchestrationServiceException =
                new EventListenerV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventListenerV2ClientDependencyException =
                new EventListenerV2ClientDependencyException(
                    message: "Event listener client dependency error occurred, contact support.",
                    innerException: eventListenerV2OrchestrationServiceException.InnerException as Xeption,
                    data: (eventListenerV2OrchestrationServiceException.InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveOrRegisterEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2OrchestrationServiceException);

            // when
            ValueTask<EventListenerV2> retrieveOrRegisterEventListenerV2Task =
                this.eventListenerV2Client.RetrieveOrRegisterEventListenerV2Async(
                    someEventListenerV2,
                    randomCancellationToken);

            EventListenerV2ClientDependencyException actualEventListenerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventListenerV2ClientDependencyException>(
                    retrieveOrRegisterEventListenerV2Task.AsTask);

            // then
            actualEventListenerV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientDependencyException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveOrRegisterEventListenerV2Async(
                    It.IsAny<EventListenerV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
