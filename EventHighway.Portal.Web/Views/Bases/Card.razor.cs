// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class Card
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public string CssClass { get; set; } = string.Empty;

        [Parameter]
        public RenderFragment? HeaderContent { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public RenderFragment? FooterContent { get; set; }
    }
}
