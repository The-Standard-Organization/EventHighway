// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class Modal
    {
        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public RenderFragment? FooterContent { get; set; }

        // Optional Bootstrap modal size (e.g. "lg", "xl"). Empty renders the default width.
        [Parameter]
        public string Size { get; set; } = string.Empty;

        // When true, the body scrolls within the modal instead of growing the page.
        [Parameter]
        public bool Scrollable { get; set; }

        private string SizeClass =>
            string.IsNullOrWhiteSpace(Size) ? string.Empty : $"modal-{Size}";

        private string ScrollableClass =>
            Scrollable ? "modal-dialog-scrollable" : string.Empty;
    }
}
