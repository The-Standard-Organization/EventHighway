// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApi
{
    // Rating is written as a JSON string so it can be used as a promoted property
    // (promotion reads JSON values as strings) and read back into a double by handlers.
    internal static class MediaItemSerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            NumberHandling =
                JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString
        };

        public static string Serialize(MediaItem mediaItem) =>
            JsonSerializer.Serialize(mediaItem, Options);

        public static MediaItem Deserialize(string content) =>
            JsonSerializer.Deserialize<MediaItem>(content, Options) ?? new MediaItem();
    }
}
