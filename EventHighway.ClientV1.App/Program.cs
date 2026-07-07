// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.EventAddresses;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners;
using EventHighway.Core.Models.Services.Foundations.Events;
using EventHighway.SqlServer.Brokers;

namespace EventHighway.ClientV1.App
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string inputConnectionString = String.Concat(
                "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;",
                "Trusted_Connection=True;MultipleActiveResultSets=true");

            SqlServerStorageBrokerProvider storageProvider =
                new SqlServerStorageBrokerProvider(inputConnectionString);

            var eventHighwayClient = new EventHighwayClient(storageProvider);

            await eventHighwayClient.V2.EventAddressV2Client.RegisterEventAddressV2Async(
                eventAddressV2: new EventAddressV2
                {
                    Id = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32"),
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow,
                    Name = "Test",
                    Description = "Some Desc."
                });


            await eventHighwayClient.EventAddresses.RegisterEventAddressAsync(
                eventAddress: new EventAddress
                {
                    Id = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32"),
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow,
                    Name = "Test",
                    Description = "Some Desc."
                });

            await eventHighwayClient.EventListeners.RegisterEventListenerAsync(
                eventListener: new EventListener
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow,
                    Endpoint = "https://localhost:7056/api/tests",
                    EventAddressId = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32")
                });

            await eventHighwayClient.EventListeners.RegisterEventListenerAsync(
                eventListener: new EventListener
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow,
                    Endpoint = "https://localhost:7104/api/tests",
                    EventAddressId = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32")
                });

            await eventHighwayClient.Events.SubmitEventAsync(
                @event: new Event
                {
                    Content = "{ \"name\": \"Test\" }",
                    EventAddressId = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32"),
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow
                });
        }
    }
}
