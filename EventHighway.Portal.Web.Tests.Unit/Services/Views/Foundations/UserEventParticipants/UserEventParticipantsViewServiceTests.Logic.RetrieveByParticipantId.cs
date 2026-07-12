// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.UserEventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAssociationsByParticipantIdAsync()
        {
            // given
            Guid inputParticipantId = GetRandomGuid();
            Guid otherParticipantId = GetRandomGuid();

            AppUser randomUser = CreateRandomUser(GetRandomGuid());

            UserEventParticipant matchingAssociation =
                CreateRandomAssociation(randomUser.Id, inputParticipantId);

            UserEventParticipant nonMatchingAssociation =
                CreateRandomAssociation(GetRandomGuid(), otherParticipantId);

            var associations = new List<UserEventParticipant>
            {
                matchingAssociation,
                nonMatchingAssociation
            };

            var expectedView = new UserEventParticipantView
            {
                Id = matchingAssociation.Id,
                UserId = randomUser.Id,
                UserName = randomUser.UserName,
                UserEmail = randomUser.Email,
                EventParticipantId = inputParticipantId
            };

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectAllUserEventParticipants())
                    .Returns(associations.AsQueryable());

            this.identityBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(randomUser.Id))
                    .ReturnsAsync(randomUser);

            // when
            List<UserEventParticipantView> actualViews =
                await this.userEventParticipantsViewService
                    .RetrieveAssociationsByParticipantIdAsync(
                        inputParticipantId, TestContext.Current.CancellationToken);

            // then
            actualViews.Should().ContainSingle();
            actualViews[0].Should().BeEquivalentTo(expectedView);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectAllUserEventParticipants(), Times.Once);

            this.identityBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(randomUser.Id), Times.Once);

            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.identityBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
