// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Portal.Web.Models.Views.Components.Foundations.Navigations;

namespace EventHighway.Portal.Web.Views.Components.Foundations.Navigations
{
    public static class NavMenuProvider
    {
        public static IReadOnlyList<NavItem> GetNavMenu() =>
            new[]
            {
                new NavItem(
                    Title: "Dashboard - Status",
                    Icon: "cil-speedometer",
                    Href: ""),

                new NavItem(
                    Title: "Dashboard - Stats",
                    Icon: "cil-chart-line",
                    Href: "stats"),

                new NavItem(
                    Title: "Admin",
                    Icon: "cil-settings",
                    Href: "",
                    Roles: new[] { "Administrators" },
                    RequiresAuth: true,
                    Children: new[]
                    {
                        new NavItem("Events", "cil-bolt", "admin/events",
                            Roles: new[] { "Administrators" }, RequiresAuth: true),

                        new NavItem("Archived Events", "cil-storage", "admin/event-archives",
                            Roles: new[] { "Administrators" }, RequiresAuth: true),

                        new NavItem("Replay", "cil-loop-circular", "admin/replay",
                            Roles: new[] { "Administrators" }, RequiresAuth: true),

                        new NavItem("Event Participants", "cil-people", "admin/participants",
                            Roles: new[] { "Administrators" }, RequiresAuth: true),

                        new NavItem("Event Address", "cil-location-pin", "admin/event-addresses",
                            Roles: new[] { "Administrators" }, RequiresAuth: true),

                        new NavItem("Users", "cil-user", "admin/users",
                            Roles: new[] { "Administrators" }, RequiresAuth: true),
                    }),

                new NavItem(
                    Title: "My Account",
                    Icon: "cil-user",
                    Href: "",
                    RequiresAuth: true,
                    Children: new[]
                    {
                        new NavItem("Profile", "cil-user", "Account/Manage",
                            RequiresAuth: true, ExactMatch: true),

                        new NavItem("Email", "cil-envelope-closed", "Account/Manage/Email",
                            RequiresAuth: true),

                        new NavItem("Password", "cil-lock-locked", "Account/Manage/ChangePassword",
                            RequiresAuth: true),

                        new NavItem("Two-factor Authentication", "cil-shield-alt",
                            "Account/Manage/TwoFactorAuthentication", RequiresAuth: true),

                        new NavItem("Passkeys", "cil-fingerprint", "Account/Manage/Passkeys",
                            RequiresAuth: true),

                        new NavItem("Participant Management", "cil-people", "my/participants",
                            RequiresAuth: true),

                        new NavItem("Personal Data", "cil-file", "Account/Manage/PersonalData",
                            RequiresAuth: true),
                    })
            };
    }
}
