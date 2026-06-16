// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBulkRemoveIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            var expectedException =
                new ListenerEventArchiveV2ProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.BulkRemoveListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask bulkRemoveTask =
                this.listenerEventArchiveV2ProcessingService
                    .BulkRemoveListenerEventArchiveV2sAsync(
                        someListenerEventArchiveV2s,
                        cancellationToken);

            var actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingDependencyValidationException>(
                        bulkRemoveTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkRemoveListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(
                    It.Is(SameExceptionAs(expectedException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnBulkRemoveIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            var expectedException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.BulkRemoveListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask bulkRemoveTask =
                this.listenerEventArchiveV2ProcessingService
                    .BulkRemoveListenerEventArchiveV2sAsync(
                        someListenerEventArchiveV2s,
                        cancellationToken);

            var actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingDependencyException>(
                        bulkRemoveTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkRemoveListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(
                    It.Is(SameExceptionAs(expectedException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            var serviceException = new Exception();

            serviceException.Data.Add(
                key: GetRandomString(),
                value: new List<string> { GetRandomString() });

            var failedServiceException =
                new FailedListenerEventArchiveV2ProcessingServiceException(
                    message: "Failed listener event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedException =
                new ListenerEventArchiveV2ProcessingServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: failedServiceException);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.BulkRemoveListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask bulkRemoveTask =
                this.listenerEventArchiveV2ProcessingService
                    .BulkRemoveListenerEventArchiveV2sAsync(
                        someListenerEventArchiveV2s,
                        cancellationToken);

            var actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingServiceException>(
                        bulkRemoveTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkRemoveListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(
                    It.Is(SameExceptionAs(expectedException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
