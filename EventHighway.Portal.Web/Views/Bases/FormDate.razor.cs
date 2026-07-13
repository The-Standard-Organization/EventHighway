// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class FormDate
    {
        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public DateTimeOffset? Value { get; set; }

        [Parameter]
        public EventCallback<DateTimeOffset?> ValueChanged { get; set; }

        private string? FormattedValue =>
            Value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        private async Task OnChangeAsync(ChangeEventArgs args)
        {
            string? rawValue = args.Value?.ToString();

            DateTimeOffset? parsedValue =
                DateTimeOffset.TryParse(
                    rawValue,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out DateTimeOffset value)
                    ? value
                    : null;

            await ValueChanged.InvokeAsync(parsedValue);
        }
    }
}
