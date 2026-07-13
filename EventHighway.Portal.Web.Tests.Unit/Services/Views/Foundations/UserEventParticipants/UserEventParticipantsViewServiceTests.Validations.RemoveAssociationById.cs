// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.UserEventParticipants.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveAssociationByIdWhenNotFoundAsync()
        {
            // given
            Guid inputAssociationId = GetRandomGuid();

            var expectedNotFoundException =
                new NotFoundUserEventParticipantsViewException();

            var expectedValidationException =
                new UserEventParticipantsViewValidationException(
                    innerException: expectedNotFoundException);

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectUserEventParticipantByIdAsync(inputAssociationId))
                    .ReturnsAsync((UserEventParticipant)null);

            // when
            ValueTask removeTask =
                this.userEventParticipantsViewService.RemoveAssociationByIdAsync(
                    inputAssociationId, TestContext.Current.CancellationToken);

            UserEventParticipantsViewValidationException actualException =
                await Assert.ThrowsAsync<UserEventParticipantsViewValidationException>(
                    removeTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedValidationException);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectUserEventParticipantByIdAsync(inputAssociationId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedValidationException))), Times.Once);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.DeleteUserEventParticipantAsync(
                    It.IsAny<UserEventParticipant>()), Times.Never);

            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
