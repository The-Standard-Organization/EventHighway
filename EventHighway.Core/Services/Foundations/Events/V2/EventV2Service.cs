// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Foundations.Events.V2
{
    internal partial class EventV2Service : IEventV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IJsonBroker jsonBroker;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            IJsonBroker jsonBroker,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.jsonBroker = jsonBroker;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventV2> AddEventV2Async(EventV2 eventV2, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventV2OnAddAsync(eventV2);

            return await storageBroker.InsertEventV2Async(eventV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker.SelectAllEventV2sAsync(cancellationToken);
        });

        public ValueTask<EventV2> ModifyEventV2Async(EventV2 eventV2, CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventV2OnModifyAsync(eventV2);

            EventV2 maybeEventV2 =
                await this.storageBroker.SelectEventV2ByIdAsync(
                    eventV2.Id, cancellationToken);

            ValidateEventV2AgainstStorage(eventV2, maybeEventV2);

            return await storageBroker.UpdateEventV2Async(eventV2, cancellationToken);
        });

        public ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2Id(eventV2Id);

            EventV2 maybeEventV2 =
                await this.storageBroker.SelectEventV2ByIdAsync(eventV2Id, cancellationToken);

            ValidateEventV2Exists(maybeEventV2, eventV2Id);

            return await this.storageBroker.DeleteEventV2Async(maybeEventV2, cancellationToken);
        });

        public ValueTask BulkRemoveEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2sIsNotNull(eventV2s);

            await this.storageBroker.BulkDeleteEventV2sAsync(eventV2s, cancellationToken);
        });

        public ValueTask<string> RemoveVolatilePathsAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnRemoveVolatilePaths(eventV2);

            LoopDetection config =
                this.configurationBroker.GetLoopDetectionConfiguration();

            string[] volatilePaths =
                config?.VolatilePaths
                    ?.FirstOrDefault(vp => vp.EventAddressId == eventV2.EventAddressId)
                    ?.VolatileContentPaths
                ?? System.Array.Empty<string>();

            string content = eventV2.Content;

            if (!this.jsonBroker.IsValidJson(content))
                return content;

            foreach (string path in volatilePaths)
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
                foreach (var property in obj) sorted[property.Key] = property.Value;
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
