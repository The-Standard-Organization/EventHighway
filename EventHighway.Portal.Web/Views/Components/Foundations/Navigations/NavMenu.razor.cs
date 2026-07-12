// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using EventHighway.Portal.Web.Models.Views.Components.Foundations.Navigations;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Components.Foundations.Navigations
{
    public partial class NavMenu
    {
        [Parameter]
        public IReadOnlyList<NavItem> Items { get; set; } =
            NavMenuProvider.GetNavMenu();
    }
}
