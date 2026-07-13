// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class Button
    {
        [Parameter]
        public string Color { get; set; } = "primary";

        [Parameter]
        public string Type { get; set; } = "button";

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public string CssClass { get; set; } = string.Empty;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public EventCallback OnClick { get; set; }
    }
}
