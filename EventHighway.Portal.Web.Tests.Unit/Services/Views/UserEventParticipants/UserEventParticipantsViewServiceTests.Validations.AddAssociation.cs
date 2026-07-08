// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Models.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Foundations.Users;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddAssociationWhenUserNotFoundAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            Guid inputParticipantId = GetRandomGuid();

            var expectedNotFoundException =
                new NotFoundUserEventParticipantsViewException();

            var expectedValidationException =
                new UserEventParticipantsViewValidationException(
                    innerException: expectedNotFoundException);

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(inputUserId))
                    .ReturnsAsync((AppUser)null);

            // when
            ValueTask<Models.Views.UserEventParticipants.UserEventParticipantView> addTask =
                this.userEventParticipantsViewService.AddAssociationAsync(
                    inputUserId, inputParticipantId, TestContext.Current.CancellationToken);

            UserEventParticipantsViewValidationException actualException =
                await Assert.ThrowsAsync<UserEventParticipantsViewValidationException>(
                    addTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedValidationException);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(inputUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedValidationException))), Times.Once);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.InsertUserEventParticipantAsync(
                    It.IsAny<UserEventParticipant>()), Times.Never);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

            this.identityBrokerMock.VerifyNoOtherCalls();
            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
