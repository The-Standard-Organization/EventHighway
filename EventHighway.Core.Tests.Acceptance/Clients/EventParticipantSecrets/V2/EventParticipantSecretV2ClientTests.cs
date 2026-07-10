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

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipantSecrets.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class EventParticipantSecretV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public EventParticipantSecretV2ClientTests(ClientBroker clientBroker)
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = clientBroker;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private async ValueTask<IEnumerable<EventParticipantSecretV2>>
            CreateRandomEventParticipantSecretV2sAsync()
        {
            EventParticipantV2 randomEventParticipantV2 =
                await CreateAndPersistRandomEventParticipantV2Async();

            int randomCount = GetRandomNumber();
            var randomEventParticipantSecretV2s = new List<EventParticipantSecretV2>();

            for (int index = 1; index <= randomCount; index++)
            {
                EventParticipantSecretV2 randomEventParticipantSecretV2 =
                    CreateRandomEventParticipantSecretV2(randomEventParticipantV2.Id);

                await this.clientBroker.AddEventParticipantSecretV2Async(
                    randomEventParticipantSecretV2);

                randomEventParticipantSecretV2s.Add(randomEventParticipantSecretV2);
            }

            return randomEventParticipantSecretV2s;
        }

        private async ValueTask<EventParticipantSecretV2>
            CreateRandomEventParticipantSecretV2Async()
        {
            EventParticipantV2 randomEventParticipantV2 =
                await CreateAndPersistRandomEventParticipantV2Async();

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomEventParticipantV2.Id);

            await this.clientBroker.AddEventParticipantSecretV2Async(
                randomEventParticipantSecretV2);

            return randomEventParticipantSecretV2;
        }

        private async ValueTask<EventParticipantV2>
            CreateAndPersistRandomEventParticipantV2Async()
        {
            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2();

            return await this.clientBroker.AddEventParticipantV2Async(
                randomEventParticipantV2);
        }

        private static EventParticipantSecretV2 CreateRandomEventParticipantSecretV2(
            Guid participantId) =>
                CreateEventParticipantSecretV2Filler(participantId).Create();

        private static EventParticipantV2 CreateRandomEventParticipantV2() =>
            CreateEventParticipantV2Filler().Create();

        private static Filler<EventParticipantSecretV2> CreateEventParticipantSecretV2Filler(
            Guid participantId)
        {
            DateTimeOffset now = TruncateToMicroseconds(DateTimeOffset.UtcNow);
            var filler = new Filler<EventParticipantSecretV2>();

            filler.Setup()
                .OnProperty(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.Id).Use(() => Guid.NewGuid())

                .OnProperty(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.EventParticipantV2Id).Use(participantId)

                .OnProperty(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.IsActive).Use(true)

                .OnProperty(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.ActiveFrom).IgnoreIt()

                .OnProperty(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.ActiveTo).IgnoreIt()

                .OnProperty(eventParticipantSecretV2 =>
                    eventParticipantSecretV2.EventParticipantV2).IgnoreIt()

                .OnType<DateTimeOffset>().Use(valueToUse: now);

            return filler;
        }

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
