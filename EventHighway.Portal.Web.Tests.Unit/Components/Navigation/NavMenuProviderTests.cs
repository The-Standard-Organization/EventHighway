// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using EventHighway.Portal.Web.Components.Navigation;
using EventHighway.Portal.Web.Models.Views.Navigations;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Navigation
{
    public class NavMenuProviderTests
    {
        [Fact]
        public void ShouldReturnStatusDashboardAsFirstNavItem()
        {
            // given . when
            IReadOnlyList<NavItem> navMenu = NavMenuProvider.GetNavMenu();

            // then
            navMenu.Should().NotBeEmpty();
            navMenu[0].Title.Should().Be("Dashboard - Status");
            navMenu[0].Href.Should().Be("");
            navMenu[0].Roles.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnStatsDashboardAsSecondNavItem()
        {
            // given . when
            IReadOnlyList<NavItem> navMenu = NavMenuProvider.GetNavMenu();

            // then
            navMenu[1].Title.Should().Be("Dashboard - Stats");
            navMenu[1].Href.Should().Be("stats");
            navMenu[1].Roles.Should().BeNull();
        }

        [Fact]
        public void ShouldGateAdminGroupToAdministrators()
        {
            // given . when
            IReadOnlyList<NavItem> navMenu = NavMenuProvider.GetNavMenu();

            // then
            NavItem adminGroup =
                navMenu.Single(item => item.Title == "Admin");

            adminGroup.Roles.Should().Contain("Administrators");
            adminGroup.Children.Should().NotBeNullOrEmpty();

            adminGroup.Children!.Select(child => child.Href).Should().Contain(new[]
            {
                "admin/participants",
                "admin/event-addresses",
                "admin/events",
                "admin/event-archives",
                "admin/replay",
                "admin/users"
            });
        }

        [Fact]
        public void ShouldReturnMyAccountGroupRequiringAuthentication()
        {
            // given . when
            IReadOnlyList<NavItem> navMenu = NavMenuProvider.GetNavMenu();

            // then
            NavItem myAccountGroup =
                navMenu.Single(item => item.Title == "My Account");

            myAccountGroup.RequiresAuth.Should().BeTrue();
            myAccountGroup.Roles.Should().BeNull();
            myAccountGroup.Children.Should().NotBeNullOrEmpty();

            myAccountGroup.Children!.Select(child => child.Href).Should().ContainInOrder(
                "Account/Manage",
                "Account/Manage/Email",
                "Account/Manage/ChangePassword",
                "Account/Manage/TwoFactorAuthentication",
                "Account/Manage/Passkeys",
                "my/participants",
                "Account/Manage/PersonalData");

            NavItem profileChild =
                myAccountGroup.Children!.Single(child => child.Href == "Account/Manage");

            profileChild.ExactMatch.Should().BeTrue();
        }
    }
}
