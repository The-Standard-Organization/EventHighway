// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class FormSwitch
    {
        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public bool Value { get; set; }

        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }

        private async Task OnChangeAsync(ChangeEventArgs args) =>
            await ValueChanged.InvokeAsync((bool)(args.Value ?? false));
    }
}
