// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V1
{
    public partial class ListenerEventV1ArchiveProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            ListenerEventV1Archive someListenerEventV1Archive = CreateRandomListenerEventV1Archive();

            var expectedListenerEventV1ArchiveProcessingDependencyValidationException =
                new ListenerEventV1ArchiveProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventV1ArchiveServiceMock.Setup(service =>
                service.AddListenerEventV1ArchiveAsync(
                    It.IsAny<ListenerEventV1Archive>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveProcessingService.AddListenerEventV1ArchiveAsync(
                    someListenerEventV1Archive);

            ListenerEventV1ArchiveProcessingDependencyValidationException
                actualListenerEventV1ArchiveProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventV1ArchiveProcessingDependencyValidationException>(
                        addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ArchiveProcessingDependencyValidationException);

            this.listenerEventV1ArchiveServiceMock.Verify(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveProcessingDependencyValidationException))),
                        Times.Once);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            ListenerEventV1Archive someListenerEventV1Archive = CreateRandomListenerEventV1Archive();

            var expectedListenerEventV1ArchiveProcessingDependencyException =
                new ListenerEventV1ArchiveProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventV1ArchiveServiceMock.Setup(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveProcessingService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventV1ArchiveProcessingDependencyException
                actualListenerEventV1ArchiveProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV1ArchiveProcessingDependencyException>(
                        addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveProcessingDependencyException.Should().BeEquivalentTo(
                expectedListenerEventV1ArchiveProcessingDependencyException);

            this.listenerEventV1ArchiveServiceMock.Verify(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveProcessingDependencyException))),
                        Times.Once);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventV1Archive someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            var serviceException = new Exception();

            var failedListenerEventV1ArchiveProcessingServiceException =
                new FailedListenerEventV1ArchiveProcessingServiceException(
                    message: "Failed listener event archive service error occurred, contact support.",
                    innerException: serviceException);

            var expectedListenerEventV1ArchiveProcessingExceptionException =
                new ListenerEventV1ArchiveProcessingServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: failedListenerEventV1ArchiveProcessingServiceException);

            this.listenerEventV1ArchiveServiceMock.Setup(service =>
                service.AddListenerEventV1ArchiveAsync(It.IsAny<ListenerEventV1Archive>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV1Archive> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveProcessingService.AddListenerEventV1ArchiveAsync(
                    someListenerEventV1Archive);

            ListenerEventV1ArchiveProcessingServiceException
                actualListenerEventV1ArchiveProcessingServiceException =
                    await Assert.ThrowsAsync<ListenerEventV1ArchiveProcessingServiceException>(
                        addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveProcessingServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveProcessingExceptionException);

            this.listenerEventV1ArchiveServiceMock.Verify(service =>
                service.AddListenerEventV1ArchiveAsync(
                    It.IsAny<ListenerEventV1Archive>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveProcessingExceptionException))),
                        Times.Once);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
