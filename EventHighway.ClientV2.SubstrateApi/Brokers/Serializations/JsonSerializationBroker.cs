// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.Serializations
{
    public sealed class JsonSerializationBroker : IJsonSerializationBroker
    {
        // Numbers are written as JSON strings so they can be used as promoted properties
        // (promotion reads JSON values as strings) and read back by handlers.
        private static readonly JsonSerializerOptions Options = new()
        {
            NumberHandling =
                JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString
        };

        private static readonly JsonSerializerOptions IndentedOptions = new()
        {
            WriteIndented = true
        };

        public ValueTask<string> SerializeAsync<T>(T value) =>
            ValueTask.FromResult(JsonSerializer.Serialize(value, Options));

        public ValueTask<T> DeserializeAsync<T>(string value) =>
            ValueTask.FromResult(JsonSerializer.Deserialize<T>(value, Options)!);

        public ValueTask<string> PrettifyAsync(string value)
        {
            using var jsonDocument = JsonDocument.Parse(value);

            return ValueTask.FromResult(
                JsonSerializer.Serialize(jsonDocument.RootElement, IndentedOptions));
        }
    }
}
