// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V2
{
    public partial class EventAddressV2ClientTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someEventAddressV2Query = new EventAddressV2Query();

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventAddressV2ProcessingDependencyException =
                new EventAddressV2ProcessingDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV2ClientDependencyException =
                new EventAddressV2ClientDependencyException(
                    message: "Event address client dependency error occurred, contact support.",
                    innerException: eventAddressV2ProcessingDependencyException.InnerException as Xeption,
                    data: (eventAddressV2ProcessingDependencyException.InnerException as Xeption).Data);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventAddressV2ProcessingDependencyException);

            // when
            ValueTask<IReadOnlyList<EventAddressV2>> retrieveAllEventAddressV2sTask =
                this.eventAddressV2Client.RetrieveAllEventAddressV2sAsync(
                    someEventAddressV2Query, randomCancellationToken);

            EventAddressV2ClientDependencyException actualEventAddressV2ClientDependencyException =
                await Assert.ThrowsAsync<EventAddressV2ClientDependencyException>(
                    retrieveAllEventAddressV2sTask.AsTask);

            // then
            actualEventAddressV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientDependencyException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someEventAddressV2Query = new EventAddressV2Query();

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventAddressV2ProcessingServiceException =
                new EventAddressV2ProcessingServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV2ClientDependencyException =
                new EventAddressV2ClientDependencyException(
                    message: "Event address client dependency error occurred, contact support.",
                    innerException: eventAddressV2ProcessingServiceException.InnerException as Xeption,
                    data: (eventAddressV2ProcessingServiceException.InnerException as Xeption).Data);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventAddressV2ProcessingServiceException);

            // when
            ValueTask<IReadOnlyList<EventAddressV2>> retrieveAllEventAddressV2sTask =
                this.eventAddressV2Client.RetrieveAllEventAddressV2sAsync(
                    someEventAddressV2Query, randomCancellationToken);

            EventAddressV2ClientDependencyException actualEventAddressV2ClientDependencyException =
                await Assert.ThrowsAsync<EventAddressV2ClientDependencyException>(
                    retrieveAllEventAddressV2sTask.AsTask);

            // then
            actualEventAddressV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientDependencyException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someEventAddressV2Query = new EventAddressV2Query();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedEventAddressV2ClientServiceException =
                new EventAddressV2ClientServiceException(
                    message: "Event address client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(someXeption);

            // when
            ValueTask<IReadOnlyList<EventAddressV2>> retrieveAllEventAddressV2sTask =
                this.eventAddressV2Client.RetrieveAllEventAddressV2sAsync(
                    someEventAddressV2Query, randomCancellationToken);

            EventAddressV2ClientServiceException actualEventAddressV2ClientServiceException =
                await Assert.ThrowsAsync<EventAddressV2ClientServiceException>(
                    retrieveAllEventAddressV2sTask.AsTask);

            // then
            actualEventAddressV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientServiceException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAllAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someEventAddressV2Query = new EventAddressV2Query();

            var operationCanceledException =
                new OperationCanceledException();

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<EventAddressV2>> retrieveAllEventAddressV2sTask =
                this.eventAddressV2Client.RetrieveAllEventAddressV2sAsync(
                    someEventAddressV2Query, randomCancellationToken);

            OperationCanceledException actualOperationCanceledException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllEventAddressV2sTask.AsTask);

            // then
            actualOperationCanceledException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    It.IsAny<EventAddressV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
