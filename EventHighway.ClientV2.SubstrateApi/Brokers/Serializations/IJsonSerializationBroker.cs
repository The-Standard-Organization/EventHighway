// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.Serializations
{
    public interface IJsonSerializationBroker
    {
        ValueTask<string> SerializeAsync<T>(T value);
        ValueTask<T> DeserializeAsync<T>(string value);

        /// <summary>
        /// Re-writes a JSON document with indentation. Content that is not JSON throws — what to
        /// show in its place is the calling service's decision, not the broker's.
        /// </summary>
        ValueTask<string> PrettifyAsync(string value);
    }
}
