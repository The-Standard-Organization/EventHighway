// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class FormSelect
    {
        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string?> ValueChanged { get; set; }

        [Parameter]
        public IReadOnlyList<SelectOption> Options { get; set; } = new List<SelectOption>();

        private async Task OnChangeAsync(ChangeEventArgs args) =>
            await ValueChanged.InvokeAsync(args.Value?.ToString());
    }
}
