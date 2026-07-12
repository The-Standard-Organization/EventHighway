// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace EventHighway.Portal.Web.Views.Bases
{
    public sealed class ChartDataset
    {
        public string Label { get; set; } = string.Empty;

        public IReadOnlyList<double> Data { get; set; } = new List<double>();

        // Line/point colour (hex). For bar/doughnut charts more than one colour may be supplied,
        // one per data point.
        public IReadOnlyList<string> Colors { get; set; } = new List<string>();

        // Renders the line as a dashed stroke (used for the secondary "Listener Events" series).
        public bool Dashed { get; set; }

        // Fills the area under a line with a soft gradient (line charts only).
        public bool Fill { get; set; } = true;
    }
}
