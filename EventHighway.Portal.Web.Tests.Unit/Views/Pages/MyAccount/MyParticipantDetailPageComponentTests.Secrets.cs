// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Views.Pages.MyAccount;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.MyAccount
{
    public partial class MyParticipantDetailPageComponentTests
    {
        [Fact]
        public void ShouldAddSecretWhenStillAssociated()
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

            this.secretsViewServiceMock.Setup(service =>
                service.AddSecretAsync(
                    It.IsAny<EventParticipantSecretView>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new EventParticipantSecretView());

            IRenderedComponent<MyParticipantDetailPage> renderedPage =
                Render<MyParticipantDetailPage>(parameters =>
                    parameters.Add(page => page.ParticipantId, participantId));

            renderedPage
                .FindAll("button")
                .First(button => button.TextContent.Trim() == "Add Secret")
                .Click();

            // when
            renderedPage
                .FindAll(".modal button")
                .First(button => button.TextContent.Trim() == "Save")
                .Click();

            // then
            this.secretsViewServiceMock.Verify(service =>
                service.AddSecretAsync(
                    It.IsAny<EventParticipantSecretView>(), It.IsAny<CancellationToken>()),
                        Times.Once);
        }

        [Fact]
        public void ShouldDenyAddSecretWhenAssociationRevokedBeforeSubmit()
        {
            // given
            Guid userId = Guid.NewGuid();
            Guid participantId = Guid.NewGuid();
            AuthorizeUser(userId);

            EventParticipantView participant = CreateRandomParticipant(participantId);

            this.userEventParticipantsViewServiceMock.SetupSequence(service =>
                service.IsUserAssociatedWithParticipantAsync(
                    userId, participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true)
                        .ReturnsAsync(false);

            this.eventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveParticipantByIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(participant);

            IRenderedComponent<MyParticipantDetailPage> renderedPage =
                Render<MyParticipantDetailPage>(parameters =>
                    parameters.Add(page => page.ParticipantId, participantId));

            renderedPage
                .FindAll("button")
                .First(button => button.TextContent.Trim() == "Add Secret")
                .Click();

            // when
            renderedPage
                .FindAll(".modal button")
                .First(button => button.TextContent.Trim() == "Save")
                .Click();

            // then
            this.secretsViewServiceMock.Verify(service =>
                service.AddSecretAsync(
                    It.IsAny<EventParticipantSecretView>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            renderedPage.Markup.Should().Contain(
                "You do not have access to this Event Participant.");
        }
    }
}
