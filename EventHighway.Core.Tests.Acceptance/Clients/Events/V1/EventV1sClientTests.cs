// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V1
{
    public partial class EventV1sClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public EventV1sClientTests()
        {
            string connectionString = String.Concat(
                "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDb;",
                "Trusted_Connection=True;MultipleActiveResultSets=true");

            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = new ClientBroker(connectionString);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private async ValueTask<IQueryable<EventListenerV1>> CreateRandomEventListenerV1sAsync(
            Guid eventAddressV1Id,
            string inputMockEndpoint)
        {
            int randomNumber = GetRandomNumber();
            var randomEventListenerV1s = new List<EventListenerV1>();

            for (int index = 0; index < randomNumber; index++)
            {
                EventListenerV1 randomPostedEntitlementV1 =
                    await RegisterEventListenerV1Async(
                        eventAddressV1Id,
                        inputMockEndpoint);

                randomEventListenerV1s.Add(item: randomPostedEntitlementV1);
            }

            return randomEventListenerV1s.AsQueryable();
        }

        private async ValueTask<EventListenerV1> RegisterEventListenerV1Async(
            Guid eventAddressV1Id,
            string inputMockEndpoint)
        {
            EventListenerV1 randomEventListenerV1 =
                CreateRandomEventListenerV1(
                    eventAddressV1Id,
                    inputMockEndpoint);

            await this.clientBroker.RegisterEventListenerV1Async(
                randomEventListenerV1);

            return randomEventListenerV1;
        }

        private async ValueTask<IQueryable<EventV1>> CreateRandomEventV1sAsync(
            Guid eventAddressV1Id)
        {
            int randomNumber = GetRandomNumber();
            var randomEventV1s = new List<EventV1>();

            for (int index = 0; index < randomNumber; index++)
            {
                DateTimeOffset scheduledDate =
                    DateTimeOffset.Now.AddSeconds(seconds: 1);

                EventV1 randomPostedEntitlementV1 =
                    await SubmitEventV1Async(
                        eventAddressV1Id,
                        scheduledDate);

                randomEventV1s.Add(item: randomPostedEntitlementV1);
            }

            return randomEventV1s.AsQueryable();
        }

        private async ValueTask<EventV1> SubmitEventV1Async(
            Guid eventAddressV1Id,
            DateTimeOffset scheduledDate)
        {
            EventV1 randomEventV1 =
                CreateRandomEventV1(
                    eventAddressV1Id,
                    scheduledDate);

            await this.clientBroker.SubmitEventV1Async(
                randomEventV1);

            return randomEventV1;
        }

        private static EventListenerV1 CreateRandomEventListenerV1(
            Guid eventAddressV1Id,
            string inputMockEndpoint)
        {
            return CreateEventListenerV1Filler(
                eventAddressV1Id,
                inputMockEndpoint)
                    .Create();
        }

        private static EventV1 CreateRandomEventV1(
            Guid eventAddressV1Id,
            DateTimeOffset scheduledDate)
        {
            return CreateEventV1Filler(
                eventAddressV1Id,
                scheduledDate)
                    .Create();
        }

        private async ValueTask<EventAddressV1> CreateRandomEventAddressV1Async()
        {
            EventAddressV1 randomEventAddressV1 =
                CreateEventAddressV1Filler().Create();

            await this.clientBroker.RegisterEventAddressV1Async(
                randomEventAddressV1);

            return randomEventAddressV1;
        }

        private static Filler<EventListenerV1> CreateEventListenerV1Filler(
            Guid eventAddressV1Id,
            string inputMockEndpoint)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventListenerV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)

                .OnProperty(eventListenerV1 =>
                    eventListenerV1.EventAddressId).Use(eventAddressV1Id)

                .OnProperty(eventListenerV1 =>
                    eventListenerV1.Endpoint).Use(inputMockEndpoint)

                .OnProperty(eventListenerV1 =>
                    eventListenerV1.EventAddress).IgnoreIt()

                .OnProperty(eventListenerV1 =>
                    eventListenerV1.ListenerEvents).IgnoreIt();

            return filler;
        }

        private static Filler<EventV1> CreateEventV1Filler(
            Guid eventAddressV1Id,
            DateTimeOffset scheduledDate)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventV1>();

            filler.Setup()
                .OnProperty(eventV1 =>
                    eventV1.EventAddress).IgnoreIt()

                .OnProperty(eventV1 =>
                    eventV1.ListenerEvents).IgnoreIt()

                .OnProperty(eventV1 =>
                    eventV1.EventAddressId).Use(eventAddressV1Id)

                .OnProperty(eventV1 =>
                    eventV1.ScheduledDate).Use(scheduledDate)

                .OnType<DateTimeOffset>().Use(now);

            return filler;
        }

        private static Filler<EventAddressV1> CreateEventAddressV1Filler()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventAddressV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)

                .OnProperty(eventAddressV1 => eventAddressV1.Events)
                    .IgnoreIt()

                .OnProperty(eventAddressV1 => eventAddressV1.EventListeners)
                    .IgnoreIt()

                .OnProperty(eventAddressV1 => eventAddressV1.ListenerEvents)
                    .IgnoreIt();

            return filler;
        }
    }
}
