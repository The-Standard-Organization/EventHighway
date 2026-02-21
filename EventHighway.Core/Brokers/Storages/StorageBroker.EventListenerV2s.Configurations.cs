// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private void ConfigureEventListenersV2(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventListenerV2>()
                .HasOne(eventListenerV2 => eventListenerV2.EventAddress)
                .WithMany(eventAddressV2 => eventAddressV2.EventListeners)
                .HasForeignKey(eventListenerV2 => eventListenerV2.EventAddressId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EventListenerV2>()
                .Property(e => e.HandlerParams)
                .HasConversion(
                    dictionary => JsonSerializer.Serialize(dictionary, JsonOptions),
                    json => string.IsNullOrWhiteSpace(json)
                        ? new Dictionary<string, string>()
                        : JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions)
                          ?? new Dictionary<string, string>())
                .Metadata.SetValueComparer(CreateDictionaryComparer());
        }

        internal static readonly JsonSerializerOptions JsonOptions =
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

        internal static ValueComparer<Dictionary<string, string>> CreateDictionaryComparer() =>
            new(
                (left, right) => DictionaryEquals(left, right),
                dictionary => DictionaryHashCode(dictionary),
                dictionary => DictionarySnapshot(dictionary));

        internal static bool DictionaryEquals(
            Dictionary<string, string>? left,
            Dictionary<string, string>? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null || left.Count != right.Count)
            {
                return false;
            }

            return left.OrderBy(kv => kv.Key)
                       .SequenceEqual(right.OrderBy(kv => kv.Key));
        }

        internal static int DictionaryHashCode(Dictionary<string, string> dictionary)
        {
            unchecked
            {
                int hash = 17;

                foreach (var (key, value) in dictionary.OrderBy(kv => kv.Key))
                {
                    hash = (hash * 23) + key.GetHashCode();
                    hash = (hash * 23) + value.GetHashCode();
                }

                return hash;
            }
        }

        internal static Dictionary<string, string> DictionarySnapshot(
            Dictionary<string, string> dictionary) =>
            dictionary.ToDictionary(entry => entry.Key, entry => entry.Value);
    }
}
