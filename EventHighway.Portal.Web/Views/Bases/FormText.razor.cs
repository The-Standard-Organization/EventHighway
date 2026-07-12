// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class FormText
    {
        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string?> ValueChanged { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        private async Task OnInputAsync(ChangeEventArgs args) =>
            await ValueChanged.InvokeAsync(args.Value?.ToString());
    }
}
