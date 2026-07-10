// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.ClientV2.SubstrateApp.Models.MediaItems
{
    public class MediaItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Movie" or "Series"
        public List<string> Genres { get; set; } = new();
        public double Rating { get; set; }
    }
}
