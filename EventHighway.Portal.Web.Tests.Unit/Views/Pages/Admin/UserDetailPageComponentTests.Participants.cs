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
    public partial class UserDetailPageComponentTests
    {
        private static AngleSharp.Dom.IElement FindSearchInput(
            IRenderedComponent<UserDetailPage> renderedComponent) =>
            renderedComponent.FindAll("div.mb-3")
                .First(container =>
                    container.QuerySelector("label")?.TextContent == "Find Participant")
                .QuerySelector("input");

        [Fact]
        public void ShouldRenderParticipantAssociations()
        {
            // given
            UserView randomUser = CreateRandomUser(new List<string> { "Users" });

            UserEventParticipantView association =
                CreateRandomAssociationView(randomUser.Id);

            var associations = new List<UserEventParticipantView> { association };

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllRoleNamesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<string> { "Users" });

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomUser);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByUserIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(associations);

            // when
            IRenderedComponent<UserDetailPage> renderedUserDetailPage =
                Render<UserDetailPage>(parameters =>
                    parameters.Add(page => page.UserId, randomUser.Id));

            // then
            renderedUserDetailPage.Markup.Should().Contain("Event Participants");
            renderedUserDetailPage.Markup.Should().Contain(association.EventParticipantName);

            this.userEventParticipantsViewServiceMock.Verify(service =>
                service.RetrieveAssociationsByUserIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ShouldRenderEmptyStateWhenNoAssociations()
        {
            // given
            UserView randomUser = CreateRandomUser(new List<string> { "Users" });

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllRoleNamesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<string> { "Users" });

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomUser);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByUserIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserEventParticipantView>());

            // when
            IRenderedComponent<UserDetailPage> renderedUserDetailPage =
                Render<UserDetailPage>(parameters =>
                    parameters.Add(page => page.UserId, randomUser.Id));

            // then
            renderedUserDetailPage.Markup.Should().Contain(
                "This user has no participant associations.");
        }

        [Fact]
        public void ShouldRemoveAssociationWhenRemoveClicked()
        {
            // given
            UserView randomUser = CreateRandomUser(new List<string> { "Users" });

            UserEventParticipantView association =
                CreateRandomAssociationView(randomUser.Id);

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllRoleNamesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<string> { "Users" });

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomUser);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAssociationsByUserIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<UserEventParticipantView> { association });

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.RemoveAssociationByIdAsync(
                    association.Id, It.IsAny<CancellationToken>()))
                        .Returns(ValueTask.CompletedTask);

            IRenderedComponent<UserDetailPage> renderedUserDetailPage =
                Render<UserDetailPage>(parameters =>
                    parameters.Add(page => page.UserId, randomUser.Id));

            // when
            renderedUserDetailPage.FindAll("li.list-group-item")
                .First(row => row.TextContent.Contains(association.EventParticipantName))
                .QuerySelector("button")
                .Click();

            // then
            this.userEventParticipantsViewServiceMock.Verify(service =>
                service.RemoveAssociationByIdAsync(
                    association.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ShouldSearchParticipantsByName()
        {
            // given
            UserView randomUser = CreateRandomUser(new List<string> { "Users" });
            EventParticipantView participant = CreateRandomParticipantView();

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllRoleNamesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<string> { "Users" });

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomUser);

            this.eventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveAllParticipantsAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<EventParticipantView> { participant });

            IRenderedComponent<UserDetailPage> renderedUserDetailPage =
                Render<UserDetailPage>(parameters =>
                    parameters.Add(page => page.UserId, randomUser.Id));

            FindSearchInput(renderedUserDetailPage).Input(participant.Name);

            // when
            renderedUserDetailPage
                .FindAll("button")
                .First(button => button.TextContent.Trim() == "Search")
                .Click();

            // then
            renderedUserDetailPage.Markup.Should().Contain(participant.Name);

            this.eventParticipantsViewServiceMock.Verify(service =>
                service.RetrieveAllParticipantsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ShouldAddAssociationWhenAddClicked()
        {
            // given
            UserView randomUser = CreateRandomUser(new List<string> { "Users" });
            EventParticipantView participant = CreateRandomParticipantView();

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllRoleNamesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<string> { "Users" });

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomUser);

            this.eventParticipantsViewServiceMock.Setup(service =>
                service.RetrieveParticipantByIdAsync(
                    participant.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(participant);

            this.userEventParticipantsViewServiceMock.Setup(service =>
                service.AddAssociationAsync(
                    randomUser.Id, participant.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new UserEventParticipantView());

            IRenderedComponent<UserDetailPage> renderedUserDetailPage =
                Render<UserDetailPage>(parameters =>
                    parameters.Add(page => page.UserId, randomUser.Id));

            FindSearchInput(renderedUserDetailPage).Input(participant.Id.ToString());

            renderedUserDetailPage
                .FindAll("button")
                .First(button => button.TextContent.Trim() == "Search")
                .Click();

            // when
            renderedUserDetailPage.FindAll("li.list-group-item")
                .First(row => row.TextContent.Contains(participant.Name))
                .QuerySelector("button")
                .Click();

            // then
            this.userEventParticipantsViewServiceMock.Verify(service =>
                service.AddAssociationAsync(
                    randomUser.Id, participant.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
