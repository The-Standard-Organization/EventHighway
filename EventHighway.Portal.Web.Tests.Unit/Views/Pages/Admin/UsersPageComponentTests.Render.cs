// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
    public partial class UsersPageComponentTests
    {
        [Fact]
        public void ShouldDisplayLoadingSpinnerInitially()
        {
            // given
            var pendingSource = new TaskCompletionSource<List<UserView>>();

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllUsersAsync(It.IsAny<CancellationToken>()))
                    .Returns(new ValueTask<List<UserView>>(pendingSource.Task));

            // when
            IRenderedComponent<UsersPage> renderedUsersPage = Render<UsersPage>();

            // then
            renderedUsersPage.FindAll("div.spinner-border").Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldRenderUsersWhenLoaded()
        {
            // given
            List<UserView> randomUsers = CreateRandomUsers(count: 3);

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllUsersAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomUsers);

            // when
            IRenderedComponent<UsersPage> renderedUsersPage = Render<UsersPage>();

            // then
            foreach (UserView user in randomUsers)
            {
                renderedUsersPage.Markup.Should().Contain(user.UserName);
            }

            this.usersViewServiceMock.Verify(service =>
                service.RetrieveAllUsersAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public void ShouldRenderErrorWhenViewServiceThrows()
        {
            // given
            var serviceException =
                new UsersViewServiceException(
                    innerException: new Xeption(message: GetRandomString()));

            this.usersViewServiceMock.Setup(service =>
                service.RetrieveAllUsersAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            IRenderedComponent<UsersPage> renderedUsersPage = Render<UsersPage>();

            // then
            renderedUsersPage.FindAll("div.alert-danger").Should().NotBeEmpty();
        }
    }
}
