// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Foundations.UserEventParticipants;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldRemoveAssociationByIdAsync()
        {
            // given
            Guid inputAssociationId = GetRandomGuid();

            UserEventParticipant existingAssociation =
                CreateRandomAssociation(GetRandomGuid(), GetRandomGuid());

            existingAssociation.Id = inputAssociationId;

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectUserEventParticipantByIdAsync(inputAssociationId))
                    .ReturnsAsync(existingAssociation);

            // when
            await this.userEventParticipantsViewService.RemoveAssociationByIdAsync(
                inputAssociationId, TestContext.Current.CancellationToken);

            // then
            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectUserEventParticipantByIdAsync(inputAssociationId), Times.Once);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.DeleteUserEventParticipantAsync(existingAssociation), Times.Once);

            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
