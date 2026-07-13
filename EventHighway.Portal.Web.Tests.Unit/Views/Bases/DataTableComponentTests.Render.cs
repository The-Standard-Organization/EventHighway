// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class DataTableComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given
            List<Sample> samples = CreateSamples(count: 3);
            List<DataTableColumn<Sample>> columns = CreateColumns();

            // when
            IRenderedComponent<DataTable<Sample>> renderedDataTable =
                Render<DataTable<Sample>>(parameters => parameters
                    .Add(dataTable => dataTable.Items, samples)
                    .Add(dataTable => dataTable.Columns, columns));

            // then
            renderedDataTable.Instance.CurrentPage.Should().Be(1);
            renderedDataTable.Instance.SearchTerm.Should().BeEmpty();
            renderedDataTable.Instance.SortColumn.Should().BeNull();
        }

        [Fact]
        public void ShouldRenderRowsAndHeadersForItems()
        {
            // given
            List<Sample> samples = CreateSamples(count: 3);
            List<DataTableColumn<Sample>> columns = CreateColumns();

            // when
            IRenderedComponent<DataTable<Sample>> renderedDataTable =
                Render<DataTable<Sample>>(parameters => parameters
                    .Add(dataTable => dataTable.Items, samples)
                    .Add(dataTable => dataTable.Columns, columns));

            // then
            renderedDataTable.FindAll("tbody tr").Should().HaveCount(3);

            renderedDataTable.FindAll("thead th")
                .Select(header => header.TextContent.Trim())
                .Should().Contain(new[] { "Name", "Value" });

            renderedDataTable.Markup.Should().Contain(samples[0].Name);
        }

        [Fact]
        public void ShouldFilterRowsWhenSearching()
        {
            // given
            var samples = new List<Sample>
            {
                new Sample("Alpha", 1),
                new Sample("Beta", 2),
                new Sample("Gamma", 3),
            };

            List<DataTableColumn<Sample>> columns = CreateColumns();

            IRenderedComponent<DataTable<Sample>> renderedDataTable =
                Render<DataTable<Sample>>(parameters => parameters
                    .Add(dataTable => dataTable.Items, samples)
                    .Add(dataTable => dataTable.Columns, columns));

            // when
            renderedDataTable.Find("input.datatable-search").Input("Beta");

            // then
            renderedDataTable.FindAll("tbody tr").Should().HaveCount(1);
            renderedDataTable.Markup.Should().Contain("Beta");
            renderedDataTable.Instance.SearchTerm.Should().Be("Beta");
        }

        [Fact]
        public void ShouldSortRowsWhenSortableHeaderClicked()
        {
            // given
            var samples = new List<Sample>
            {
                new Sample("Charlie", 3),
                new Sample("Alice", 1),
                new Sample("Bob", 2),
            };

            List<DataTableColumn<Sample>> columns = CreateColumns();

            IRenderedComponent<DataTable<Sample>> renderedDataTable =
                Render<DataTable<Sample>>(parameters => parameters
                    .Add(dataTable => dataTable.Items, samples)
                    .Add(dataTable => dataTable.Columns, columns));

            // when (ascending)
            renderedDataTable.FindAll("thead th")[0].Click();

            // then
            renderedDataTable.Instance.SortColumn!.Title.Should().Be("Name");
            renderedDataTable.Instance.SortAscending.Should().BeTrue();
            renderedDataTable.FindAll("tbody tr td")[0].TextContent.Trim().Should().Be("Alice");

            // when (toggle to descending)
            renderedDataTable.FindAll("thead th")[0].Click();

            // then
            renderedDataTable.Instance.SortAscending.Should().BeFalse();
            renderedDataTable.FindAll("tbody tr td")[0].TextContent.Trim().Should().Be("Charlie");
        }

        [Fact]
        public void ShouldPageRowsWhenItemsExceedPageSize()
        {
            // given
            List<Sample> samples = CreateSamples(count: 5);
            List<DataTableColumn<Sample>> columns = CreateColumns();

            IRenderedComponent<DataTable<Sample>> renderedDataTable =
                Render<DataTable<Sample>>(parameters => parameters
                    .Add(dataTable => dataTable.Items, samples)
                    .Add(dataTable => dataTable.Columns, columns)
                    .Add(dataTable => dataTable.PageSize, 2));

            // then (first page)
            renderedDataTable.FindAll("tbody tr").Should().HaveCount(2);
            renderedDataTable.Instance.PageCount.Should().Be(3);

            // when (next page)
            renderedDataTable.Find("button.datatable-next").Click();

            // then
            renderedDataTable.Instance.CurrentPage.Should().Be(2);
            renderedDataTable.FindAll("tbody tr").Should().HaveCount(2);
        }
    }
}
