// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Components.Pages.Admin;
using EventHighway.Portal.Web.Models.Views.EventParticipants;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants;
using EventHighway.Portal.Web.Models.Views.Users;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.Admin
{
    public partial class UserDetailPageComponentTests
    {
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
    }
}
