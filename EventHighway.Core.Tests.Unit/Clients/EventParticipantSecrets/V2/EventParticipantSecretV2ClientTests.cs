// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Clients.EventParticipantSecrets.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        private readonly Mock<IEventParticipantSecretV2Service> eventParticipantSecretV2ServiceMock;
        private int eventParticipantSecretServiceResolutionCount;
        private readonly IEventParticipantSecretV2Client eventParticipantSecretV2Client;

        public EventParticipantSecretV2ClientTests()
        {
            this.eventParticipantSecretV2ServiceMock =
                new Mock<IEventParticipantSecretV2Service>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped(_ =>
            {
                this.eventParticipantSecretServiceResolutionCount++;

                return this.eventParticipantSecretV2ServiceMock.Object;
            });

            this.eventParticipantSecretV2Client =
                new EventParticipantSecretV2Client(
                    serviceProvider: serviceCollection.BuildServiceProvider());
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventParticipantSecretV2 CreateRandomEventParticipantSecretV2() =>
            CreateEventParticipantSecretV2Filler(dates: GetRandomDateTimeOffset()).Create();

        private static IEnumerable<EventParticipantSecretV2> CreateRandomEventParticipantSecretV2s() =>
            CreateEventParticipantSecretV2Filler(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber());

        private static Filler<EventParticipantSecretV2> CreateEventParticipantSecretV2Filler(DateTimeOffset dates)
        {
            var filler = new Filler<EventParticipantSecretV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.IsActive)
                    .Use(true)

                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.ActiveFrom)
                    .IgnoreIt()

                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.ActiveTo)
                    .IgnoreIt()

                .OnProperty(eventParticipantSecretV2 => eventParticipantSecretV2.EventParticipantV2)
                    .IgnoreIt();

            return filler;
        }
    }
}
