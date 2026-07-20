// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EventHighway.Core.Brokers.Jsons
{
    internal class JsonBroker : IJsonBroker
    {
        public string Serialize<T>(T value) =>
            JsonSerializer.Serialize(value);

        public T Deserialize<T>(string json) =>
            JsonSerializer.Deserialize<T>(json);

        public string GetJsonPropertyValue(string json, string propertyName)
        {
            using JsonDocument document = JsonDocument.Parse(json);

            return TryResolveProperty(document.RootElement, propertyName, out JsonElement element)
                ? GetElementValue(element)
                : null;
        }

        public bool CheckIfPropertyExist(string json, string propertyName)
        {
            using JsonDocument document = JsonDocument.Parse(json);
            return TryResolveProperty(document.RootElement, propertyName, out _);
        }

        private static bool TryResolveProperty(
            JsonElement rootElement,
            string propertyName,
            out JsonElement resolvedElement)
        {
            // An exact property name match takes precedence over dot-path traversal so
            // properties whose names contain dots keep resolving as they always have.
            if (rootElement.TryGetProperty(propertyName, out resolvedElement))
                return true;

            resolvedElement = rootElement;

            foreach (string pathSegment in propertyName.Split('.'))
            {
                if (resolvedElement.ValueKind != JsonValueKind.Object
                    || !resolvedElement.TryGetProperty(pathSegment, out resolvedElement))
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetElementValue(JsonElement element) =>
            element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Null => null,
                JsonValueKind.Undefined => null,
                _ => element.GetRawText()
            };

        public bool IsValidJson(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            try
            {
                JsonDocument.Parse(content);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public string RemoveNode(string json, string path)
        {
            JsonObject obj = JsonNode.Parse(json).AsObject();
            obj.Remove(path);
            return obj.ToJsonString();
        }
    }
}
