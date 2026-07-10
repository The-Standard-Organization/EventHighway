// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Clients.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Processings.EventParticipants.V2;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        private readonly Mock<IEventParticipantV2ProcessingService> eventParticipantV2ProcessingServiceMock;
        private readonly IEventParticipantV2Client eventParticipantV2Client;

        public EventParticipantV2ClientTests()
        {
            this.eventParticipantV2ProcessingServiceMock =
                new Mock<IEventParticipantV2ProcessingService>();

            this.eventParticipantV2Client =
                new EventParticipantV2Client(
                    eventParticipantV2ProcessingService:
                        this.eventParticipantV2ProcessingServiceMock.Object);
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventParticipantV2 CreateRandomEventParticipantV2() =>
            CreateEventParticipantV2Filler(dates: GetRandomDateTimeOffset()).Create();

        private static IEnumerable<EventParticipantV2> CreateRandomEventParticipantV2s() =>
            CreateEventParticipantV2Filler(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber());

        private static Filler<EventParticipantV2> CreateEventParticipantV2Filler(DateTimeOffset dates)
        {
            var filler = new Filler<EventParticipantV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnProperty(eventParticipantV2 => eventParticipantV2.IsActive)
                    .Use(true)

                .OnProperty(eventParticipantV2 => eventParticipantV2.ActiveFrom)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.ActiveTo)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.EventV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.EventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.EventListenerV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.ListenerEventV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.EventParticipantSecretV2s)
                    .IgnoreIt();

            return filler;
        }
    }
}
