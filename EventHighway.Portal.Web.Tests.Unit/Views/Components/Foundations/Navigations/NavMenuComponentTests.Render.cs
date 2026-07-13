// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using Bunit;
using Bunit.TestDoubles;
using EventHighway.Portal.Web.Views.Components.Foundations.Navigations;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Components.Foundations.Navigations
{
    public partial class NavMenuComponentTests
    {
        [Fact]
        public void ShouldRenderDashboardLinkForAnonymousUser()
        {
            // given
            AddAuthorization();

            // when
            IRenderedComponent<NavMenu> renderedNavMenu = Render<NavMenu>();

            // then
            renderedNavMenu.FindAll("a")
                .Select(anchor => anchor.GetAttribute("href"))
                .Should().Contain("");

            renderedNavMenu.Markup.Should().Contain("Dashboard");
        }

        [Fact]
        public void ShouldRenderAdminGroupForAdministrators()
        {
            // given
            BunitAuthorizationContext authorizationContext = AddAuthorization();
            authorizationContext.SetAuthorized("admin");
            authorizationContext.SetRoles("Administrators");

            // when
            IRenderedComponent<NavMenu> renderedNavMenu = Render<NavMenu>();

            // then
            renderedNavMenu.FindAll("a")
                .Select(anchor => anchor.GetAttribute("href"))
                .Should().Contain(new[]
                {
                    "admin/participants",
                    "admin/users"
                });
        }

        [Fact]
        public void ShouldHideAdminGroupForAnonymousUser()
        {
            // given
            AddAuthorization();

            // when
            IRenderedComponent<NavMenu> renderedNavMenu = Render<NavMenu>();

            // then
            renderedNavMenu.FindAll("a")
                .Select(anchor => anchor.GetAttribute("href"))
                .Should().NotContain("admin/participants");
        }

        [Fact]
        public void ShouldRenderMyAccountGroupForAuthenticatedUser()
        {
            // given
            BunitAuthorizationContext authorizationContext = AddAuthorization();
            authorizationContext.SetAuthorized("user");

            // when
            IRenderedComponent<NavMenu> renderedNavMenu = Render<NavMenu>();

            // then
            renderedNavMenu.FindAll("a")
                .Select(anchor => anchor.GetAttribute("href"))
                .Should().Contain(new[]
                {
                    "Account/Manage",
                    "my/participants"
                });

            renderedNavMenu.Markup.Should().Contain("My Account");
        }

        [Fact]
        public void ShouldHideMyAccountGroupForAnonymousUser()
        {
            // given
            AddAuthorization();

            // when
            IRenderedComponent<NavMenu> renderedNavMenu = Render<NavMenu>();

            // then
            renderedNavMenu.FindAll("a")
                .Select(anchor => anchor.GetAttribute("href"))
                .Should().NotContain("my/participants");

            renderedNavMenu.Markup.Should().NotContain("My Account");
        }
    }
}
