// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAssociationsByUserIdWhenErrorOccursAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            var serviceException = new Exception();

            var failedServiceException =
                new FailedUserEventParticipantsViewServiceException(
                    innerException: serviceException);

            var expectedServiceException =
                new UserEventParticipantsViewServiceException(
                    innerException: failedServiceException);

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectAllUserEventParticipants())
                    .Throws(serviceException);

            // when
            ValueTask<List<UserEventParticipantView>> retrieveTask =
                this.userEventParticipantsViewService.RetrieveAssociationsByUserIdAsync(
                    inputUserId, TestContext.Current.CancellationToken);

            UserEventParticipantsViewServiceException actualException =
                await Assert.ThrowsAsync<UserEventParticipantsViewServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedServiceException);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectAllUserEventParticipants(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedServiceException))), Times.Once);

            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAssociationsByParticipantIdWhenErrorOccursAsync()
        {
            // given
            Guid inputParticipantId = GetRandomGuid();
            var serviceException = new Exception();

            var failedServiceException =
                new FailedUserEventParticipantsViewServiceException(
                    innerException: serviceException);

            var expectedServiceException =
                new UserEventParticipantsViewServiceException(
                    innerException: failedServiceException);

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectAllUserEventParticipants())
                    .Throws(serviceException);

            // when
            ValueTask<List<UserEventParticipantView>> retrieveTask =
                this.userEventParticipantsViewService.RetrieveAssociationsByParticipantIdAsync(
                    inputParticipantId, TestContext.Current.CancellationToken);

            UserEventParticipantsViewServiceException actualException =
                await Assert.ThrowsAsync<UserEventParticipantsViewServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedServiceException);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectAllUserEventParticipants(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedServiceException))), Times.Once);

            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
