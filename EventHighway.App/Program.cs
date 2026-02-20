// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Services.Foundations.Events.V1;

namespace EventHighway.App
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string inputConnectionString = String.Concat(
                "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB2;",
                "Trusted_Connection=True;MultipleActiveResultSets=true");

            var eventHighwayClient = new EventHighwayClient(inputConnectionString);

            //await eventHighwayClient.EventAddressV1s.RegisterEventAddressV1Async(
            //    eventAddressV1: new EventAddressV1
            //    {
            //        Id = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32"),
            //        CreatedDate = DateTimeOffset.UtcNow,
            //        UpdatedDate = DateTimeOffset.UtcNow,
            //        Name = "Test",
            //        Description = "Some Desc."
            //    });

            //await eventHighwayClient.EventListenerV1s.RegisterEventListenerV1Async(
            //    eventListenerV1: new EventListenerV1
            //    {
            //        Id = Guid.NewGuid(),
            //        CreatedDate = DateTimeOffset.UtcNow,
            //        UpdatedDate = DateTimeOffset.UtcNow,
            //        Endpoint = "https://localhost:7056/api/tests",
            //        EventAddressId = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32")
            //    });

            //await eventHighwayClient.EventListeners.RegisterEventListenerAsync(
            //    eventListener: new EventListener
            //    {
            //        Id = Guid.NewGuid(),
            //        CreatedDate = DateTimeOffset.UtcNow,
            //        UpdatedDate = DateTimeOffset.UtcNow,
            //        Endpoint = "https://localhost:7104/api/tests",
            //        EventAddressId = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32")
            //    });

            //await eventHighwayClient.Events.SubmitEventAsync(
            //    @event: new Event
            //    {
            //        Content = "{ \"name\": \"Test\" }",
            //        EventAddressId = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32"),
            //        Id = Guid.NewGuid(),
            //        CreatedDate = DateTimeOffset.UtcNow,
            //        UpdatedDate = DateTimeOffset.UtcNow
            //    });

            await eventHighwayClient.EventV1s.SubmitEventV1Async(
                eventV1: new EventV1
                {
                    Content = "{ \"name\": \"Test\" }",
                    EventAddressId = Guid.Parse(input: "d3b3b3b3-0b3b-4b3b-8b3b-0b3b3b3b3b32"),
                    Id = Guid.NewGuid(),
                    RetryAttempts = 5,
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow
                });
        }
    }
}
