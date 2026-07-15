// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventParticipantSecrets
{
    public partial class EventParticipantSecretsViewServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            var expectedException =
                new EventParticipantSecretsViewDependencyValidationException(
                    innerException: dependencyValidationException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantSecretV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            EventParticipantSecretsViewDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventParticipantSecretsViewDependencyValidationException>(
                    async () => await this.eventParticipantSecretsViewService
                        .RetrieveSecretsByParticipantAsync(
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
                new EventParticipantSecretsViewDependencyException(
                    innerException: dependencyException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantSecretV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            EventParticipantSecretsViewDependencyException actualException =
                await Assert.ThrowsAsync<EventParticipantSecretsViewDependencyException>(
                    async () => await this.eventParticipantSecretsViewService
                        .RetrieveSecretsByParticipantAsync(
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
                new FailedEventParticipantSecretsViewServiceException(
                    innerException: serviceException);

            var expectedException =
                new EventParticipantSecretsViewServiceException(
                    innerException: failedServiceException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantSecretV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            EventParticipantSecretsViewServiceException actualException =
                await Assert.ThrowsAsync<EventParticipantSecretsViewServiceException>(
                    async () => await this.eventParticipantSecretsViewService
                        .RetrieveSecretsByParticipantAsync(
                            Guid.NewGuid(), TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()), Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
