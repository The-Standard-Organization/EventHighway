// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Models.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAssociationsByUserIdAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            Guid otherUserId = GetRandomGuid();

            EventParticipantV2 randomParticipant =
                CreateRandomParticipant(GetRandomGuid());

            UserEventParticipant matchingAssociation =
                CreateRandomAssociation(inputUserId, randomParticipant.Id);

            UserEventParticipant nonMatchingAssociation =
                CreateRandomAssociation(otherUserId, GetRandomGuid());

            var associations = new List<UserEventParticipant>
            {
                matchingAssociation,
                nonMatchingAssociation
            };

            var expectedView = new UserEventParticipantView
            {
                Id = matchingAssociation.Id,
                UserId = inputUserId,
                EventParticipantId = randomParticipant.Id,
                EventParticipantName = randomParticipant.Name
            };

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectAllUserEventParticipants())
                    .Returns(associations.AsQueryable());

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    randomParticipant.Id, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(randomParticipant);

            // when
            List<UserEventParticipantView> actualViews =
                await this.userEventParticipantsViewService
                    .RetrieveAssociationsByUserIdAsync(
                        inputUserId, TestContext.Current.CancellationToken);

            // then
            actualViews.Should().ContainSingle();
            actualViews[0].Should().BeEquivalentTo(expectedView);

            this.userEventParticipantBrokerMock.Verify(broker =>
                broker.SelectAllUserEventParticipants(), Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    randomParticipant.Id, It.IsAny<System.Threading.CancellationToken>()),
                        Times.Once);

            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.identityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
