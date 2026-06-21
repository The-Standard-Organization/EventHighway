// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Loggings;

namespace EventHighway.Core.Services.Foundations.VolatilePaths
{
    internal partial class VolatilePathService : IVolatilePathService
    {
        private readonly IJsonBroker jsonBroker;
        private readonly ILoggingBroker loggingBroker;

        public VolatilePathService(
            IJsonBroker jsonBroker,
            ILoggingBroker loggingBroker)
        {
            this.jsonBroker = jsonBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<string> RemoveVolatilePathsAsync(
            string content,
            string[] volatileContentPaths) =>
        TryCatch(async () =>
        {
            ValidateOnRemoveVolatilePaths(content, volatileContentPaths);

            if (!this.jsonBroker.IsValidJson(content))
                return content;

            foreach (string path in volatileContentPaths)
                content = this.jsonBroker.RemoveNode(content, path);

            return Canonicalize(content);
        });

        private static string Canonicalize(string json)
        {
            JsonNode node = JsonNode.Parse(json);
            return SerializeSorted(node);
        }

        private static string SerializeSorted(JsonNode node)
        {
            if (node is JsonObject obj)
            {
                var sorted = new SortedDictionary<string, JsonNode>();

                foreach (var property in obj)
                    sorted[property.Key] = property.Value;

                var result = new JsonObject();

                foreach (var kvp in sorted)
                    result[kvp.Key] = JsonNode.Parse(SerializeSorted(kvp.Value));

                return result.ToJsonString();
            }

            if (node is JsonArray arr)
            {
                var resultArr = new JsonArray();

                foreach (JsonNode item in arr)
                    resultArr.Add(JsonNode.Parse(SerializeSorted(item)));

                return resultArr.ToJsonString();
            }

            return node?.ToJsonString() ?? "null";
        }
    }
}
