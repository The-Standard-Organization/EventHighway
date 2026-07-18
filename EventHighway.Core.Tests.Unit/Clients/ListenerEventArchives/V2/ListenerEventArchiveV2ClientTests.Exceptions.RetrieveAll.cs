// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveAllIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventArchiveV2ClientValidationException =
                new ListenerEventArchiveV2ClientValidationException(
                    message: "Listener event archive client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<IReadOnlyList<ListenerEventArchiveV2>> retrieveAllListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2Client.RetrieveAllListenerEventArchiveV2sAsync(
                    new ListenerEventArchiveV2Query(), randomCancellationToken);

            ListenerEventArchiveV2ClientValidationException actualListenerEventArchiveV2ClientValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ClientValidationException>(
                    retrieveAllListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ClientValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ClientValidationException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var listenerEventArchiveV2DependencyException =
                new ListenerEventArchiveV2DependencyException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventArchiveV2ClientDependencyException =
                new ListenerEventArchiveV2ClientDependencyException(
                    message: "Listener event archive client dependency error occurred, contact support.",

                    innerException: listenerEventArchiveV2DependencyException
                        .InnerException as Xeption,

                    data: (listenerEventArchiveV2DependencyException
                        .InnerException as Xeption).Data);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(listenerEventArchiveV2DependencyException);

            // when
            ValueTask<IReadOnlyList<ListenerEventArchiveV2>> retrieveAllListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2Client.RetrieveAllListenerEventArchiveV2sAsync(
                    new ListenerEventArchiveV2Query(), randomCancellationToken);

            ListenerEventArchiveV2ClientDependencyException actualListenerEventArchiveV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ClientDependencyException>(
                    retrieveAllListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ClientDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
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

            var listenerEventArchiveV2ServiceException =
                new ListenerEventArchiveV2ServiceException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventArchiveV2ClientDependencyException =
                new ListenerEventArchiveV2ClientDependencyException(
                    message: "Listener event archive client dependency error occurred, contact support.",

                    innerException: listenerEventArchiveV2ServiceException
                        .InnerException as Xeption,

                    data: (listenerEventArchiveV2ServiceException
                        .InnerException as Xeption).Data);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(listenerEventArchiveV2ServiceException);

            // when
            ValueTask<IReadOnlyList<ListenerEventArchiveV2>> retrieveAllListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2Client.RetrieveAllListenerEventArchiveV2sAsync(
                    new ListenerEventArchiveV2Query(), randomCancellationToken);

            ListenerEventArchiveV2ClientDependencyException actualListenerEventArchiveV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ClientDependencyException>(
                    retrieveAllListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ClientDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedListenerEventArchiveV2ClientServiceException =
                new ListenerEventArchiveV2ClientServiceException(
                    message: "Listener event archive client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(someXeption);

            // when
            ValueTask<IReadOnlyList<ListenerEventArchiveV2>> retrieveAllListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2Client.RetrieveAllListenerEventArchiveV2sAsync(
                    new ListenerEventArchiveV2Query(), randomCancellationToken);

            ListenerEventArchiveV2ClientServiceException actualListenerEventArchiveV2ClientServiceException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ClientServiceException>(
                    retrieveAllListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ClientServiceException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ClientServiceException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAllAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IReadOnlyList<ListenerEventArchiveV2>> retrieveAllListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2Client.RetrieveAllListenerEventArchiveV2sAsync(
                    new ListenerEventArchiveV2Query(), randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllListenerEventArchiveV2sTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventArchiveV2sByQueryAsync(
                    It.IsAny<ListenerEventArchiveV2Query>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
