// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventArchives.V2
{
    public partial class EventArchiveV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveAllWithEventAddressV2IfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someEventArchiveV2Query = new EventArchiveV2Query();

            var expectedEventArchiveV2ClientValidationException =
                new EventArchiveV2ClientValidationException(
                    message: "Event archive client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2Client
                    .RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                        someEventArchiveV2Query, randomCancellationToken);

            EventArchiveV2ClientValidationException actualEventArchiveV2ClientValidationException =
                await Assert.ThrowsAsync<EventArchiveV2ClientValidationException>(
                    retrieveAllTask.AsTask);

            // then
            actualEventArchiveV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ClientValidationException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllWithEventAddressV2IfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someEventArchiveV2Query = new EventArchiveV2Query();

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventArchiveV2ClientDependencyException =
                new EventArchiveV2ClientDependencyException(
                    message: "Event archive client dependency error occurred, contact support.",
                    innerException: eventArchiveV2DependencyException.InnerException as Xeption,
                    data: (eventArchiveV2DependencyException.InnerException as Xeption).Data);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventArchiveV2DependencyException);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2Client
                    .RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                        someEventArchiveV2Query, randomCancellationToken);

            EventArchiveV2ClientDependencyException actualEventArchiveV2ClientDependencyException =
                await Assert.ThrowsAsync<EventArchiveV2ClientDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualEventArchiveV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ClientDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllWithEventAddressV2IfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someEventArchiveV2Query = new EventArchiveV2Query();

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventArchiveV2ServiceException =
                new EventArchiveV2ServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventArchiveV2ClientDependencyException =
                new EventArchiveV2ClientDependencyException(
                    message: "Event archive client dependency error occurred, contact support.",
                    innerException: eventArchiveV2ServiceException.InnerException as Xeption,
                    data: (eventArchiveV2ServiceException.InnerException as Xeption).Data);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventArchiveV2ServiceException);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2Client
                    .RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                        someEventArchiveV2Query, randomCancellationToken);

            EventArchiveV2ClientDependencyException actualEventArchiveV2ClientDependencyException =
                await Assert.ThrowsAsync<EventArchiveV2ClientDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualEventArchiveV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ClientDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllWithEventAddressV2IfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someEventArchiveV2Query = new EventArchiveV2Query();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedEventArchiveV2ClientServiceException =
                new EventArchiveV2ClientServiceException(
                    message: "Event archive client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2Client
                    .RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                        someEventArchiveV2Query, randomCancellationToken);

            EventArchiveV2ClientServiceException actualEventArchiveV2ClientServiceException =
                await Assert.ThrowsAsync<EventArchiveV2ClientServiceException>(
                    retrieveAllTask.AsTask);

            // then
            actualEventArchiveV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ClientServiceException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAllWithEventAddressV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someEventArchiveV2Query = new EventArchiveV2Query();

            var operationCanceledException =
                new OperationCanceledException();

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2Client
                    .RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                        someEventArchiveV2Query, randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                    It.IsAny<EventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
