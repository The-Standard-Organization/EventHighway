// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Portal.Web.Views.Bases;

namespace EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards
{
    public class HealthRagTile
    {
        public string Grouping { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public StatTileVariant Variant { get; set; }
    }
}
