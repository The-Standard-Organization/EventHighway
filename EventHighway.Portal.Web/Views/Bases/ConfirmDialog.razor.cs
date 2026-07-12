// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class ConfirmDialog
    {
        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public string Title { get; set; } = "Are you sure?";

        [Parameter]
        public string? Message { get; set; }

        [Parameter]
        public string ConfirmText { get; set; } = "OK";

        [Parameter]
        public string CancelText { get; set; } = "Cancel";

        [Parameter]
        public string ConfirmColor { get; set; } = "danger";

        [Parameter]
        public EventCallback OnConfirm { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }
    }
}
