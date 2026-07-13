// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Bases
{
    public partial class StatTile
    {
        [Parameter]
        public StatTileVariant Variant { get; set; } = StatTileVariant.Na;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public string? Icon { get; set; }

        // Rich gradient RAG variants (styled in StatTile.razor.css): deep crimson (Red), dark
        // amber (Amber), forest green (Green), and a neutral slate (Na) — each with a subtle top
        // sheen and matching border glow.
        public string VariantCssClass =>
            Variant switch
            {
                StatTileVariant.Green => "rag-green",
                StatTileVariant.Amber => "rag-amber",
                StatTileVariant.Red => "rag-red",
                _ => "rag-na"
            };
    }
}
