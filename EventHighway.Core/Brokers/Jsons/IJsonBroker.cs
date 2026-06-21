// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Brokers.Jsons
{
    internal interface IJsonBroker
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string json);
        string GetJsonPropertyValue(string json, string propertyName);
        bool CheckIfPropertyExist(string json, string propertyName);

        bool IsValidJson(string content);
        string RemoveNode(string json, string path);
    }
}
