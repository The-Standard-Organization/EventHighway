// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V2.Exceptions;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventAddresses
{
    public partial class EventAddressesViewServiceTests
    {

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveIfValidationErrorOccursAndLogItAsync()
        {
            // given
            var someInnerException = new Xeption(message: GetRandomString());

            var clientValidationException =
                new EventAddressV2ClientValidationException(
                    message: GetRandomString(), innerException: someInnerException,
                    data: new Hashtable());

            var expectedException =
                new EventAddressesViewDependencyValidationException(
                    innerException: clientValidationException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(clientValidationException);

            // when
            EventAddressesViewDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventAddressesViewDependencyValidationException>(
                    async () => await this.eventAddressesViewService.RetrieveAllAddressesAsync(
                        TestContext.Current.CancellationToken));

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
                new EventAddressesViewDependencyException(innerException: dependencyException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            EventAddressesViewDependencyException actualException =
                await Assert.ThrowsAsync<EventAddressesViewDependencyException>(
                    async () => await this.eventAddressesViewService.RetrieveAllAddressesAsync(
                        TestContext.Current.CancellationToken));

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
                new FailedEventAddressesViewServiceException(innerException: serviceException);

            var expectedException =
                new EventAddressesViewServiceException(innerException: failedServiceException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventAddressV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            EventAddressesViewServiceException actualException =
                await Assert.ThrowsAsync<EventAddressesViewServiceException>(
                    async () => await this.eventAddressesViewService.RetrieveAllAddressesAsync(
                        TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()), Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
