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
        public async Task ShouldSetNotFoundNameOnRetrieveAssociationsByUserIdWhenParticipantMissingAsync()
        {
            // given
            Guid inputUserId = GetRandomGuid();
            Guid missingParticipantId = GetRandomGuid();

            UserEventParticipant orphanAssociation =
                CreateRandomAssociation(inputUserId, missingParticipantId);

            var associations = new List<UserEventParticipant> { orphanAssociation };

            var expectedView = new UserEventParticipantView
            {
                Id = orphanAssociation.Id,
                UserId = inputUserId,
                EventParticipantId = missingParticipantId,
                EventParticipantName = "(participant not found)"
            };

            this.userEventParticipantBrokerMock.Setup(broker =>
                broker.SelectAllUserEventParticipants())
                    .Returns(associations.AsQueryable());

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventParticipantV2ByIdAsync(
                    missingParticipantId, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync((EventParticipantV2)null);

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
                    missingParticipantId, It.IsAny<System.Threading.CancellationToken>()),
                        Times.Once);

            this.userEventParticipantBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.identityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
