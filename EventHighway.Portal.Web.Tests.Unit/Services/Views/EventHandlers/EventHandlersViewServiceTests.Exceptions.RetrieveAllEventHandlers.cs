// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventHandlers.V2.Exceptions;
using EventHighway.Portal.Web.Models.Views.EventHandlers.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.EventHandlers
{
    public partial class EventHandlersViewServiceTests
    {
        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            var someInnerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventHandlerV2ClientValidationException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            var someInnerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventHandlerV2ClientDependencyException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),

                new EventHandlerV2ClientServiceException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),
            };
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveAllIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            var expectedViewDependencyValidationException =
                new EventHandlersViewDependencyValidationException(
                    innerException: dependencyValidationException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            EventHandlersViewDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventHandlersViewDependencyValidationException>(
                    async () => await this.eventHandlersViewService
                        .RetrieveAllEventHandlersAsync(TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedViewDependencyValidationException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var expectedViewDependencyException =
                new EventHandlersViewDependencyException(
                    innerException: dependencyException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            EventHandlersViewDependencyException actualException =
                await Assert.ThrowsAsync<EventHandlersViewDependencyException>(
                    async () => await this.eventHandlersViewService
                        .RetrieveAllEventHandlersAsync(TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedViewDependencyException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceException = new System.Exception(message: GetRandomString());

            var failedServiceException =
                new FailedEventHandlersViewServiceException(
                    innerException: serviceException);

            var expectedViewServiceException =
                new EventHandlersViewServiceException(
                    innerException: failedServiceException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            EventHandlersViewServiceException actualException =
                await Assert.ThrowsAsync<EventHandlersViewServiceException>(
                    async () => await this.eventHandlersViewService
                        .RetrieveAllEventHandlersAsync(TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedViewServiceException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventHandlerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
