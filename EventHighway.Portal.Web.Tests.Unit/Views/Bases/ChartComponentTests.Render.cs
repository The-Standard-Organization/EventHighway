// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class ChartComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<Chart> renderedChart = Render<Chart>();

            // then
            renderedChart.Instance.ChartType.Should().Be("line");
            renderedChart.Instance.ElementId.Should().StartWith("chart-");
            renderedChart.Find("canvas").Should().NotBeNull();
        }

        [Fact]
        public void ShouldRenderChartViaJsInteropOnAfterRender()
        {
            // given
            var labels = new List<string> { "Mon", "Tue" };
            var datasets = new List<ChartDataset>
            {
                new ChartDataset { Label = "Events", Data = new List<double> { 1, 2 } }
            };

            // when
            IRenderedComponent<Chart> renderedChart =
                Render<Chart>(parameters => parameters
                    .Add(chart => chart.ChartType, "bar")
                    .Add(chart => chart.Labels, labels)
                    .Add(chart => chart.Datasets, datasets));

            // then
            this.JSInterop.VerifyInvoke("eventHighwayCharts.render");
        }
    }
}
