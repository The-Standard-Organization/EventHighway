// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class DataTable<TItem>
    {
        [Parameter]
        public IEnumerable<TItem> Items { get; set; } = new List<TItem>();

        [Parameter]
        public IReadOnlyList<DataTableColumn<TItem>> Columns { get; set; } =
            new List<DataTableColumn<TItem>>();

        [Parameter]
        public bool Searchable { get; set; } = true;

        [Parameter]
        public int PageSize { get; set; } = 10;

        [Parameter]
        public RenderFragment<TItem>? RowActions { get; set; }

        [Parameter]
        public Func<TItem, string?>? RowClass { get; set; }

        [Parameter]
        public RenderFragment? ExtraFilters { get; set; }

        public string SearchTerm { get; private set; } = string.Empty;

        public DataTableColumn<TItem>? SortColumn { get; private set; }

        public bool SortAscending { get; private set; } = true;

        public int CurrentPage { get; private set; } = 1;

        private IEnumerable<TItem> FilteredItems =>
            string.IsNullOrWhiteSpace(SearchTerm)
                ? Items
                : Items.Where(item =>
                    Columns.Any(column =>
                        (column.Value(item)?.ToString() ?? string.Empty)
                            .Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));

        private IEnumerable<TItem> SortedItems
        {
            get
            {
                if (SortColumn is null)
                {
                    return FilteredItems;
                }

                return SortAscending
                    ? FilteredItems.OrderBy(item => SortColumn.Value(item))
                    : FilteredItems.OrderByDescending(item => SortColumn.Value(item));
            }
        }

        public IReadOnlyList<TItem> PagedItems =>
            SortedItems
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

        public int PageCount =>
            Math.Max(1, (int)Math.Ceiling(FilteredItems.Count() / (double)PageSize));

        private void OnSearchChanged(ChangeEventArgs args)
        {
            SearchTerm = args.Value?.ToString() ?? string.Empty;
            CurrentPage = 1;
        }

        private void ToggleSort(DataTableColumn<TItem> column)
        {
            if (SortColumn == column)
            {
                SortAscending = !SortAscending;
            }
            else
            {
                SortColumn = column;
                SortAscending = true;
            }

            CurrentPage = 1;
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }

        private void NextPage()
        {
            if (CurrentPage < PageCount)
            {
                CurrentPage++;
            }
        }
    }
}
