// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.Serializations
{
    public interface IJsonSerializationBroker
    {
        ValueTask<string> SerializeAsync<T>(T value);
        ValueTask<T> DeserializeAsync<T>(string value);
    }
}
