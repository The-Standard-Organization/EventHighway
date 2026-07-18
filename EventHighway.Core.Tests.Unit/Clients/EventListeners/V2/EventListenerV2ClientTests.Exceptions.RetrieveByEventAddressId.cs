// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;
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
        public async Task
            ShouldThrowValidationExceptionOnRetrieveByEventAddressIdIfValidationErrorOccursAsync(
                Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();

            var expectedEventListenerV2ClientValidationException =
                new EventListenerV2ClientValidationException(
                    message: "Event listener client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveEventListenerV2sTask =
                this.eventListenerV2Client.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    randomCancellationToken);

            EventListenerV2ClientValidationException actualEventListenerV2ClientValidationException =
                await Assert.ThrowsAsync<EventListenerV2ClientValidationException>(
                    retrieveEventListenerV2sTask.AsTask);

            // then
            actualEventListenerV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientValidationException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();
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

                    innerException: eventListenerV2OrchestrationDependencyException
                        .InnerException as Xeption,

                    data: (eventListenerV2OrchestrationDependencyException
                        .InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2OrchestrationDependencyException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveEventListenerV2sTask =
                this.eventListenerV2Client.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    randomCancellationToken);

            EventListenerV2ClientDependencyException actualEventListenerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventListenerV2ClientDependencyException>(
                    retrieveEventListenerV2sTask.AsTask);

            // then
            actualEventListenerV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientDependencyException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();
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

                    innerException: eventListenerV2OrchestrationServiceException
                        .InnerException as Xeption,

                    data: (eventListenerV2OrchestrationServiceException
                        .InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2OrchestrationServiceException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveEventListenerV2sTask =
                this.eventListenerV2Client.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    randomCancellationToken);

            EventListenerV2ClientDependencyException actualEventListenerV2ClientDependencyException =
                await Assert.ThrowsAsync<EventListenerV2ClientDependencyException>(
                    retrieveEventListenerV2sTask.AsTask);

            // then
            actualEventListenerV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientDependencyException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnRetrieveByEventAddressIdIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedEventListenerV2ClientServiceException =
                new EventListenerV2ClientServiceException(
                    message: "Event listener client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveEventListenerV2sTask =
                this.eventListenerV2Client.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    randomCancellationToken);

            EventListenerV2ClientServiceException actualEventListenerV2ClientServiceException =
                await Assert.ThrowsAsync<EventListenerV2ClientServiceException>(
                    retrieveEventListenerV2sTask.AsTask);

            // then
            actualEventListenerV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientServiceException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByEventAddressIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();

            var operationCanceledException =
                new OperationCanceledException();

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<EventListenerV2>> retrieveEventListenerV2sTask =
                this.eventListenerV2Client.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    new EventListenerV2Query(),
                    randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveEventListenerV2sTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<EventListenerV2Query>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
