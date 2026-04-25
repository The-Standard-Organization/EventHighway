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
    public partial class ListenerEventArchiveV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            ListenerEventArchiveV1 someListenerEventArchiveV1 = CreateRandomListenerEventArchiveV1();

            var expectedListenerEventArchiveV1ProcessingDependencyValidationException =
                new ListenerEventArchiveV1ProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventArchiveV1ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV1Async(
                    It.IsAny<ListenerEventArchiveV1>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1ProcessingService.AddListenerEventArchiveV1Async(
                    someListenerEventArchiveV1);

            ListenerEventArchiveV1ProcessingDependencyValidationException
                actualListenerEventArchiveV1ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV1ProcessingDependencyValidationException>(
                        addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV1ProcessingDependencyValidationException);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1ProcessingDependencyValidationException))),
                        Times.Once);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            ListenerEventArchiveV1 someListenerEventArchiveV1 = CreateRandomListenerEventArchiveV1();

            var expectedListenerEventArchiveV1ProcessingDependencyException =
                new ListenerEventArchiveV1ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV1ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1ProcessingService.AddListenerEventArchiveV1Async(someListenerEventArchiveV1);

            ListenerEventArchiveV1ProcessingDependencyException
                actualListenerEventArchiveV1ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV1ProcessingDependencyException>(
                        addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1ProcessingDependencyException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV1ProcessingDependencyException);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 someListenerEventArchiveV1 = CreateRandomListenerEventArchiveV1();
            var serviceException = new Exception();

            var failedListenerEventArchiveV1ProcessingServiceException =
                new FailedListenerEventArchiveV1ProcessingServiceException(
                    message: "Failed listener event archive service error occurred, contact support.",
                    innerException: serviceException);

            var expectedListenerEventArchiveV1ProcessingExceptionException =
                new ListenerEventArchiveV1ProcessingServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: failedListenerEventArchiveV1ProcessingServiceException);

            this.listenerEventArchiveV1ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV1Async(It.IsAny<ListenerEventArchiveV1>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventArchiveV1Task =
                this.listenerEventArchiveV1ProcessingService.AddListenerEventArchiveV1Async(
                    someListenerEventArchiveV1);

            ListenerEventArchiveV1ProcessingServiceException
                actualListenerEventArchiveV1ProcessingServiceException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV1ProcessingServiceException>(
                        addListenerEventArchiveV1Task.AsTask);

            // then
            actualListenerEventArchiveV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV1ProcessingExceptionException);

            this.listenerEventArchiveV1ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV1Async(
                    It.IsAny<ListenerEventArchiveV1>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV1ProcessingExceptionException))),
                        Times.Once);

            this.listenerEventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
