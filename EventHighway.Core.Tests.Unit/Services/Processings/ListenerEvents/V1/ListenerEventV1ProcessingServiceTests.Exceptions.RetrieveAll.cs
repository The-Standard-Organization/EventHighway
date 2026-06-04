// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V1
{
    public partial class ListenerEventV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ListenerEventV1DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyExceptionOccursAndLogItAsync(
            Xeption listenerEventV1DependencyException)
        {
            // given
            var expectedListenerEventV1ProcessingDependencyException =
                new ListenerEventV1ProcessingDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: listenerEventV1DependencyException.InnerException as Xeption);

            this.listenerEventV1ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV1sAsync())
                    .ThrowsAsync(listenerEventV1DependencyException);

            // when
            ValueTask<IQueryable<ListenerEventV1>> retrieveAllListenerEventV1sTask =
                this.listenerEventV1ProcessingService.RetrieveAllListenerEventV1sAsync();

            ListenerEventV1ProcessingDependencyException
                actualListenerEventV1ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV1ProcessingDependencyException>(
                        retrieveAllListenerEventV1sTask.AsTask);

            // then
            actualListenerEventV1ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1ProcessingDependencyException);

            this.listenerEventV1ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedListenerEventV1ProcessingServiceException =
                new FailedListenerEventV1ProcessingServiceException(
                    message: "Failed listener event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV1ProcessingExceptionException =
                new ListenerEventV1ProcessingServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV1ProcessingServiceException);

            this.listenerEventV1ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV1sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<ListenerEventV1>> retrieveAllListenerEventV1sTask =
                this.listenerEventV1ProcessingService.RetrieveAllListenerEventV1sAsync();

            ListenerEventV1ProcessingServiceException
                actualListenerEventV1ProcessingServiceException =
                    await Assert.ThrowsAsync<ListenerEventV1ProcessingServiceException>(
                        retrieveAllListenerEventV1sTask.AsTask);

            // then
            actualListenerEventV1ProcessingServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV1ProcessingExceptionException);

            this.listenerEventV1ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV1sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ProcessingExceptionException))),
                        Times.Once);

            this.listenerEventV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
