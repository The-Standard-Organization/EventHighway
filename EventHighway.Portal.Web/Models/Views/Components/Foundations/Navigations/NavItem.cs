// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Portal.Web.Models.Views.Components.Foundations.Navigations
{
    public sealed record NavItem(
        string Title,
        string Icon,
        string Href,
        string[]? Roles = null,
        bool RequiresAuth = false,
        NavItem[]? Children = null,
        bool ExactMatch = false);
}
