// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using EventHighway.Portal.Web.Views.Pages.Admin;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Users.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class UserDetailPageComponentTests
    {
        [Fact]
        public void ShouldRenderUserDetailsAndRoles()
        {
            // given
            var roles = new List<string> { "Administrators", "Users" };
            UserView randomUser = CreateRandomUser(roles);
            var allRoleNames = new List<string> { "Administrators", "Users" };

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllRoleNamesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(allRoleNames);

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomUser);

            // when
            IRenderedComponent<UserDetailPage> renderedUserDetailPage =
                Render<UserDetailPage>(parameters =>
                    parameters.Add(page => page.UserId, randomUser.Id));

            // then
            renderedUserDetailPage.Markup.Should().Contain(randomUser.UserName);
            renderedUserDetailPage.Markup.Should().Contain("Administrators");
            renderedUserDetailPage.FindAll("li.list-group-item").Should().HaveCount(roles.Count);
        }

        [Fact]
        public void ShouldRemoveRoleWhenRemoveClicked()
        {
            // given
            var roles = new List<string> { "Administrators", "Users" };
            UserView randomUser = CreateRandomUser(roles);
            var allRoleNames = new List<string> { "Administrators", "Users" };

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllRoleNamesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(allRoleNames);

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(
                    randomUser.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomUser);

            this.usersViewServiceMock.Setup(service =>
                service.RemoveUserFromRoleAsync(
                    randomUser.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .Returns(ValueTask.CompletedTask);

            IRenderedComponent<UserDetailPage> renderedUserDetailPage =
                Render<UserDetailPage>(parameters =>
                    parameters.Add(page => page.UserId, randomUser.Id));

            // when
            renderedUserDetailPage.FindAll("li.list-group-item button")[0].Click();

            // then
            this.usersViewServiceMock.Verify(service =>
                service.RemoveUserFromRoleAsync(
                    randomUser.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()),
                        Times.Once);
        }

        [Fact]
        public void ShouldRenderErrorWhenViewServiceThrows()
        {
            // given
            Guid userId = Guid.NewGuid();

            var serviceException =
                new UsersViewServiceException(
                    innerException: new Xeption(message: GetRandomString()));

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllRoleNamesAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            IRenderedComponent<UserDetailPage> renderedUserDetailPage =
                Render<UserDetailPage>(parameters =>
                    parameters.Add(page => page.UserId, userId));

            // then
            renderedUserDetailPage.FindAll("div.alert-danger").Should().NotBeEmpty();
        }
    }
}
