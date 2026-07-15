// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Tests.Acceptance.Brokers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.ArchivingEvents.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class ArchivingEventV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;

        public ArchivingEventV2ClientTests(ClientBroker clientBroker)
        {
            this.wireMockServer = WireMockServer.Start();
            this.clientBroker = clientBroker;
        }

        private async ValueTask<EventAddressV2> CreateRandomEventAddressV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                CreateEventAddressV2Filler().Create();

            await this.clientBroker.RegisterEventAddressV2Async(
                randomEventAddressV2);

            return randomEventAddressV2;
        }

        private async ValueTask<EventV2> SubmitDeadEventV2Async(Guid eventAddressV2Id)
        {
            EventV2 randomEventV2 =
                CreateDeadEventV2Filler(eventAddressV2Id).Create();

            await this.clientBroker.SubmitEventV2Async(randomEventV2);

            return randomEventV2;
        }

        private static Filler<EventAddressV2> CreateEventAddressV2Filler()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventAddressV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)

                .OnProperty(eventAddressV2 => eventAddressV2.EventV2s)
                    .IgnoreIt()

                .OnProperty(eventAddressV2 => eventAddressV2.EventListenerV2s)
                    .IgnoreIt()

                .OnProperty(eventAddressV2 => eventAddressV2.ListenerEventV2s)
                    .IgnoreIt()

                .OnProperty(eventAddressV2 => eventAddressV2.EventArchiveV2s)
                    .IgnoreIt();

            return filler;
        }

        private static Filler<EventV2> CreateDeadEventV2Filler(Guid eventAddressV2Id)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnProperty(eventV2 =>
                    eventV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.ListenerEventV2s).IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.EventAddressV2Id).Use(eventAddressV2Id)

                .OnProperty(eventV2 =>
                    eventV2.ScheduledDate).Use(now.AddSeconds(seconds: -1))

                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(eventV2 => eventV2.EventParticipantV2Id).IgnoreIt()
                .OnProperty(eventV2 => eventV2.EventParticipantV2Secret).IgnoreIt()
                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }
    }
}
