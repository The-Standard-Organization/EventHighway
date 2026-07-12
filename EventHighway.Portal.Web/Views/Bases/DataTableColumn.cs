// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public sealed class DataTableColumn<TItem>
    {
        public string Title { get; set; } = string.Empty;

        public Func<TItem, object?> Value { get; set; } = _ => null;

        public bool Sortable { get; set; } = true;

        public RenderFragment<TItem>? CellTemplate { get; set; }
    }
}
