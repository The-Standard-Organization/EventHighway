// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Components.Pages.MyAccount;
using EventHighway.Portal.Web.Models.Views.EventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.MyAccount
{
    public partial class MyParticipantDetailPageComponentTests
    {
        [Fact]
        public void ShouldRenderReadOnlyParticipantWhenAssociated()
        {
            // given
            Guid userId = Guid.NewGuid();
            Guid participantId = Guid.NewGuid();
            AuthorizeUser(userId);

            EventParticipantView participant = CreateRandomParticipant(participantId);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.IsUserAssociatedWithParticipantAsync(
                    userId, participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);

            this.eventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveParticipantByIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(participant);

            // when
            IRenderedComponent<MyParticipantDetailPage> renderedPage =
                Render<MyParticipantDetailPage>(parameters =>
                    parameters.Add(page => page.ParticipantId, participantId));

            // then
            renderedPage.Markup.Should().Contain(participant.Name);
            renderedPage.Markup.Should().Contain("Add Secret");

            renderedPage.FindAll("button")
                .Select(button => button.TextContent.Trim())
                .Should().NotContain("Delete");

            renderedPage.FindAll("button")
                .Select(button => button.TextContent.Trim())
                .Should().NotContain("Edit");
        }

        [Fact]
        public void ShouldDenyAccessWhenNotAssociated()
        {
            // given
            Guid userId = Guid.NewGuid();
            Guid participantId = Guid.NewGuid();
            AuthorizeUser(userId);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.IsUserAssociatedWithParticipantAsync(
                    userId, participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(false);

            // when
            IRenderedComponent<MyParticipantDetailPage> renderedPage =
                Render<MyParticipantDetailPage>(parameters =>
                    parameters.Add(page => page.ParticipantId, participantId));

            // then
            renderedPage.Markup.Should().Contain(
                "You do not have access to this Event Participant.");

            this.eventParticipantsViewServiceMock.Verify(service =>
                service.RetrieveParticipantByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

            this.secretsViewServiceMock.Verify(service =>
                service.RetrieveSecretsByParticipantAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
