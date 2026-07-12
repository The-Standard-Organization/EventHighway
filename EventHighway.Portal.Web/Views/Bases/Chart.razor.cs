// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class Chart : IAsyncDisposable
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter]
        public string ChartType { get; set; } = "line";

        [Parameter]
        public IReadOnlyList<string> Labels { get; set; } = new List<string>();

        [Parameter]
        public IReadOnlyList<ChartDataset> Datasets { get; set; } = new List<ChartDataset>();

        [Parameter]
        public int Height { get; set; } = 300;

        public string ElementId { get; } = "chart-" + Guid.NewGuid().ToString("N");

        private string? lastRenderedSignature;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Only (re)draw the underlying Chart.js chart when the data actually changes. The
            // dashboard re-renders this component every second (auto-refresh countdown); without
            // this guard the chart would be destroyed and rebuilt on every tick, causing flicker.
            string signature = ComputeSignature();

            if (signature == lastRenderedSignature)
            {
                return;
            }

            lastRenderedSignature = signature;

            await this.JSRuntime.InvokeVoidAsync(
                "eventHighwayCharts.render", ElementId, BuildConfig());
        }

        private string ComputeSignature() =>
            ChartType + "|" + string.Join(",", Labels) + "|" + string.Join(";",
                Datasets.Select(dataset =>
                    dataset.Label + ":" + string.Join(",", dataset.Data)));

        private object BuildConfig() =>
            new
            {
                type = ChartType,
                data = new
                {
                    labels = Labels,
                    datasets = Datasets.Select(dataset => new
                    {
                        label = dataset.Label,
                        data = dataset.Data,
                        borderColor = dataset.Colors.Count > 0 ? dataset.Colors[0] : "#321fdb",
                        backgroundColor = dataset.Colors,
                        borderDash = dataset.Dashed ? new[] { 6, 4 } : Array.Empty<int>(),
                        borderWidth = 2,
                        fill = ChartType == "line" && dataset.Fill,
                        tension = 0.4,
                        pointRadius = ChartType == "line" ? 3 : 0,
                        pointHoverRadius = 5,
                        pointBackgroundColor = "#fff",
                        pointBorderColor = dataset.Colors.Count > 0 ? dataset.Colors[0] : "#321fdb",
                        pointBorderWidth = 2
                    })
                },
                options = new
                {
                    responsive = true,
                    maintainAspectRatio = false,
                    interaction = new { intersect = false, mode = "index" },
                    plugins = new
                    {
                        legend = new
                        {
                            display = Datasets.Count > 1,
                            position = "top",
                            labels = new { usePointStyle = true, boxWidth = 8 }
                        }
                    },
                    scales = new
                    {
                        x = new { grid = new { display = false } },
                        y = new { beginAtZero = true, grid = new { color = "rgba(0,0,0,0.05)" } }
                    }
                }
            };

        public async ValueTask DisposeAsync()
        {
            try
            {
                await this.JSRuntime.InvokeVoidAsync("eventHighwayCharts.destroy", ElementId);
            }
            catch
            {
                // ignored — circuit may already be gone during disposal
            }
        }
    }
}
