// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipants.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class EventParticipantV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public EventParticipantV2ClientTests(ClientBroker clientBroker)
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = clientBroker;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private async ValueTask<IEnumerable<EventParticipantV2>>
            CreateRandomEventParticipantV2sAsync()
        {
            int randomCount = GetRandomNumber();
            var randomEventParticipantV2s = new List<EventParticipantV2>();

            for (int index = 1; index <= randomCount; index++)
            {
                EventParticipantV2 randomEventParticipantV2 =
                    CreateRandomEventParticipantV2();

                await this.clientBroker.AddEventParticipantV2Async(
                    randomEventParticipantV2);

                randomEventParticipantV2s.Add(randomEventParticipantV2);
            }

            return randomEventParticipantV2s;
        }

        private async ValueTask<EventParticipantV2>
            CreateRandomEventParticipantV2Async()
        {
            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2();

            await this.clientBroker.AddEventParticipantV2Async(
                randomEventParticipantV2);

            return randomEventParticipantV2;
        }

        private static EventParticipantV2 CreateRandomEventParticipantV2() =>
            CreateEventParticipantV2Filler().Create();

        private static Filler<EventParticipantV2> CreateEventParticipantV2Filler()
        {
            DateTimeOffset now = TruncateToMicroseconds(DateTimeOffset.UtcNow);
            var filler = new Filler<EventParticipantV2>();

            filler.Setup()
                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.Id).Use(() => Guid.NewGuid())

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.IsActive).Use(true)

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.ActiveFrom).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.ActiveTo).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.EventV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.EventArchiveV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.EventListenerV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.ListenerEventV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.ListenerEventArchiveV2s).IgnoreIt()

                .OnProperty(eventParticipantV2 =>
                    eventParticipantV2.EventParticipantSecretV2s).IgnoreIt()

                .OnType<DateTimeOffset>().Use(valueToUse: now);

            return filler;
        }

        private static DateTimeOffset TruncateToMicroseconds(
            DateTimeOffset dateTimeOffset)
        {
            long ticksToRemove = dateTimeOffset.Ticks % TimeSpan.TicksPerMicrosecond;

            return dateTimeOffset.AddTicks(-ticksToRemove);
        }
    }
}
