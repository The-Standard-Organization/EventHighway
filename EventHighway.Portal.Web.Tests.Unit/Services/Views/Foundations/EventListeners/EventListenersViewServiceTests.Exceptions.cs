// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventListeners.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventListeners
{
    public partial class EventListenersViewServiceTests
    {
        public static TheoryData<Xeption> DependencyExceptions()
        {
            var someInnerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventListenerV2ClientDependencyException(
                    message: GetRandomString(), innerException: someInnerException,
                    data: new Hashtable()),

                new EventListenerV2ClientServiceException(
                    message: GetRandomString(), innerException: someInnerException,
                    data: new Hashtable()),
            };
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveIfValidationErrorOccursAndLogItAsync()
        {
            // given
            var clientValidationException =
                new EventListenerV2ClientValidationException(
                    message: GetRandomString(),
                    innerException: new Xeption(message: GetRandomString()),
                    data: new Hashtable());

            var expectedException =
                new EventListenersViewDependencyValidationException(
                    innerException: clientValidationException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(clientValidationException);

            // when
            EventListenersViewDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventListenersViewDependencyValidationException>(
                    async () => await this.eventListenersViewService
                        .RetrieveListenersByAddressAsync(
                            Guid.NewGuid(), TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()), Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var expectedException =
                new EventListenersViewDependencyException(innerException: dependencyException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            EventListenersViewDependencyException actualException =
                await Assert.ThrowsAsync<EventListenersViewDependencyException>(
                    async () => await this.eventListenersViewService
                        .RetrieveListenersByAddressAsync(
                            Guid.NewGuid(), TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()), Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception(message: GetRandomString());

            var failedServiceException =
                new FailedEventListenersViewServiceException(innerException: serviceException);

            var expectedException =
                new EventListenersViewServiceException(innerException: failedServiceException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            EventListenersViewServiceException actualException =
                await Assert.ThrowsAsync<EventListenersViewServiceException>(
                    async () => await this.eventListenersViewService
                        .RetrieveListenersByAddressAsync(
                            Guid.NewGuid(), TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()), Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
