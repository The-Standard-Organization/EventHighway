// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventParticipants
{
    public partial class EventParticipantsViewServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveAllIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            var expectedViewDependencyValidationException =
                new EventParticipantsViewDependencyValidationException(
                    innerException: dependencyValidationException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            EventParticipantsViewDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventParticipantsViewDependencyValidationException>(
                    async () => await this.eventParticipantsViewService
                        .RetrieveAllParticipantsAsync(TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedViewDependencyValidationException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(It.IsAny<CancellationToken>()),
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
                new EventParticipantsViewDependencyException(
                    innerException: dependencyException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            EventParticipantsViewDependencyException actualException =
                await Assert.ThrowsAsync<EventParticipantsViewDependencyException>(
                    async () => await this.eventParticipantsViewService
                        .RetrieveAllParticipantsAsync(TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedViewDependencyException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(It.IsAny<CancellationToken>()),
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
                new FailedEventParticipantsViewServiceException(
                    innerException: serviceException);

            var expectedViewServiceException =
                new EventParticipantsViewServiceException(
                    innerException: failedServiceException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            EventParticipantsViewServiceException actualException =
                await Assert.ThrowsAsync<EventParticipantsViewServiceException>(
                    async () => await this.eventParticipantsViewService
                        .RetrieveAllParticipantsAsync(TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedViewServiceException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
