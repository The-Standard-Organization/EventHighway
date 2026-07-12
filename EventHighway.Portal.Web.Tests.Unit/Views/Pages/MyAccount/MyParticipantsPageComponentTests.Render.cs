// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Views.Pages.MyAccount;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.UserEventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.MyAccount
{
    public partial class MyParticipantsPageComponentTests
    {
        [Fact]
        public void ShouldRenderOnlyAssociatedParticipants()
        {
            // given
            Guid userId = Guid.NewGuid();
            Guid participantId = Guid.NewGuid();
            AuthorizeUser(userId);

            UserEventParticipantView association =
                CreateRandomAssociationView(userId, participantId);

            EventParticipantView participant = CreateRandomParticipant(participantId);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByUserIdAsync(
                    userId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserEventParticipantView> { association });

            this.eventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveParticipantByIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(participant);

            // when
            IRenderedComponent<MyParticipantsPage> renderedPage =
                Render<MyParticipantsPage>();

            // then
            renderedPage.Markup.Should().Contain(participant.Name);
            renderedPage.Markup.Should().Contain("VIEW");

            this.userEventParticipantsViewServiceMock.Verify(service =>
                service.RetrieveAssociationsByUserIdAsync(
                    userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ShouldRenderEmptyStateWhenNoAssociations()
        {
            // given
            Guid userId = Guid.NewGuid();
            AuthorizeUser(userId);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByUserIdAsync(
                    userId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserEventParticipantView>());

            // when
            IRenderedComponent<MyParticipantsPage> renderedPage =
                Render<MyParticipantsPage>();

            // then
            renderedPage.Markup.Should().Contain(
                "You do not currently have any Event Participant Associations. "
                    + "Contact Support if you think this is incorrect.");
        }

        [Fact]
        public void ShouldNotGiveAdministratorExtraParticipantsWithoutAssociation()
        {
            // given
            Guid adminUserId = Guid.NewGuid();
            AuthorizeUser(adminUserId, "Administrators");

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByUserIdAsync(
                    adminUserId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserEventParticipantView>());

            // when
            IRenderedComponent<MyParticipantsPage> renderedPage =
                Render<MyParticipantsPage>();

            // then
            renderedPage.Markup.Should().Contain(
                "You do not currently have any Event Participant Associations.");

            this.eventParticipantsViewServiceMock.Verify(service =>
                service.RetrieveAllParticipantsAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
