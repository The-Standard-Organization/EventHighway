// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using Bunit;
using Bunit.TestDoubles;
using EventHighway.Portal.Web.Views.Components.Foundations.Layouts;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Components.Foundations.Layouts
{
    public partial class HeaderComponentTests
    {
        [Fact]
        public void ShouldRenderLoginLinkForAnonymousUser()
        {
            // given
            AddAuthorization();

            // when
            IRenderedComponent<Header> renderedHeader = Render<Header>();

            // then
            renderedHeader.FindAll("a")
                .Select(anchor => anchor.GetAttribute("href"))
                .Should().Contain("Account/Login");

            renderedHeader.Markup.Should().NotContain("Logout");
        }

        [Fact]
        public void ShouldRenderProfileDropdownForAuthenticatedUser()
        {
            // given
            BunitAuthorizationContext authorizationContext = AddAuthorization();
            authorizationContext.SetAuthorized("admin");

            // when
            IRenderedComponent<Header> renderedHeader = Render<Header>();

            // then
            renderedHeader.Markup.Should().Contain("admin");
            renderedHeader.Markup.Should().Contain("Logout");

            renderedHeader.FindAll("a")
                .Select(anchor => anchor.GetAttribute("href"))
                .Should().Contain(new[]
                {
                    "Account/Manage",
                    "Account/Manage/ChangePassword"
                });

            renderedHeader.FindAll("a")
                .Select(anchor => anchor.GetAttribute("href"))
                .Should().NotContain("Account/Login");
        }
    }
}
