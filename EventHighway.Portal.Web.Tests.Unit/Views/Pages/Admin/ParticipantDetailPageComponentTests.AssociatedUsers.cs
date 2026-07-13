// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using EventHighway.Portal.Web.Views.Pages.Admin;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class ParticipantDetailPageComponentTests
    {
        private static AngleSharp.Dom.IElement FindUserSearchInput(
            IRenderedComponent<ParticipantDetailPage> renderedComponent) =>
            renderedComponent.FindAll("div.mb-3")
                .First(container =>
                    container.QuerySelector("label")?.TextContent == "Find User")
                .QuerySelector("input");

        [Fact]
        public void ShouldRenderAssociatedUsers()
        {
            // given
            Guid participantId = Guid.NewGuid();
            EventParticipantView participant = CreateRandomParticipant(participantId);

            UserEventParticipantView association =
                CreateRandomAssociationView(participantId);

            this.eventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveParticipantByIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(participant);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByParticipantIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserEventParticipantView> { association });

            // when
            IRenderedComponent<ParticipantDetailPage> renderedPage =
                Render<ParticipantDetailPage>(parameters =>
                    parameters.Add(page => page.ParticipantId, participantId));

            // then
            renderedPage.Markup.Should().Contain("Associated Users");
            renderedPage.Markup.Should().Contain(association.UserName);
            renderedPage.Markup.Should().Contain(association.UserEmail);

            this.userEventParticipantsViewServiceMock.Verify(service =>
                service.RetrieveAssociationsByParticipantIdAsync(
                    participantId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ShouldRenderEmptyStateWhenNoAssociatedUsers()
        {
            // given
            Guid participantId = Guid.NewGuid();
            EventParticipantView participant = CreateRandomParticipant(participantId);

            this.eventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveParticipantByIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(participant);

            // when
            IRenderedComponent<ParticipantDetailPage> renderedPage =
                Render<ParticipantDetailPage>(parameters =>
                    parameters.Add(page => page.ParticipantId, participantId));

            // then
            renderedPage.Markup.Should().Contain(
                "No users are associated with this participant.");
        }

        [Fact]
        public void ShouldRemoveAssociatedUserWhenRemoveClicked()
        {
            // given
            Guid participantId = Guid.NewGuid();
            EventParticipantView participant = CreateRandomParticipant(participantId);

            UserEventParticipantView association =
                CreateRandomAssociationView(participantId);

            this.eventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveParticipantByIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(participant);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByParticipantIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserEventParticipantView> { association });

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RemoveAssociationByIdAsync(
                    association.Id, It.IsAny<CancellationToken>()))
                        .Returns(ValueTask.CompletedTask);

            IRenderedComponent<ParticipantDetailPage> renderedPage =
                Render<ParticipantDetailPage>(parameters =>
                    parameters.Add(page => page.ParticipantId, participantId));

            // when
            renderedPage.FindAll("li.list-group-item")
                .First(row => row.TextContent.Contains(association.UserName))
                .QuerySelector("button")
                .Click();

            // then
            this.userEventParticipantsViewServiceMock.Verify(service =>
                service.RemoveAssociationByIdAsync(
                    association.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ShouldAddAssociatedUserWhenAddClicked()
        {
            // given
            Guid participantId = Guid.NewGuid();
            EventParticipantView participant = CreateRandomParticipant(participantId);
            UserView user = CreateRandomUserView();

            this.eventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveParticipantByIdAsync(
                    participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(participant);

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllUsersAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<UserView> { user });

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.AddAssociationAsync(
                    user.Id, participantId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new UserEventParticipantView());

            IRenderedComponent<ParticipantDetailPage> renderedPage =
                Render<ParticipantDetailPage>(parameters =>
                    parameters.Add(page => page.ParticipantId, participantId));

            FindUserSearchInput(renderedPage).Input(user.UserName);

            renderedPage
                .FindAll("button")
                .First(button => button.TextContent.Trim() == "Search")
                .Click();

            // when
            renderedPage.FindAll("li.list-group-item")
                .First(row => row.TextContent.Contains(user.UserName))
                .QuerySelector("button")
                .Click();

            // then
            this.userEventParticipantsViewServiceMock.Verify(service =>
                service.AddAssociationAsync(
                    user.Id, participantId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
