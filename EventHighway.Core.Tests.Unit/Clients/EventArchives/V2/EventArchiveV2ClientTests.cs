// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using EventHighway.Core.Clients.EventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventArchives.V2
{
    public partial class EventArchiveV2ClientTests
    {
        private readonly Mock<IEventArchiveV2Service> eventArchiveV2ServiceMock;
        private int archiveServiceResolutionCount;
        private readonly IEventArchiveV2Client eventArchiveV2Client;

        public EventArchiveV2ClientTests()
        {
            this.eventArchiveV2ServiceMock =
                new Mock<IEventArchiveV2Service>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped(_ =>
            {
                this.archiveServiceResolutionCount++;

                return this.eventArchiveV2ServiceMock.Object;
            });

            this.eventArchiveV2Client =
                new EventArchiveV2Client(
                    serviceProvider: serviceCollection.BuildServiceProvider());
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventArchiveV2ValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2DependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventArchiveV2 CreateRandomEventArchiveV2() =>
            CreateEventArchiveV2Filler().Create();

        private static IQueryable<EventArchiveV2> CreateRandomEventArchiveV2s() =>
            CreateEventArchiveV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<EventArchiveV2> CreateEventArchiveV2Filler()
        {
            var filler = new Filler<EventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())

                .OnProperty(eventArchiveV2 => eventArchiveV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.EventAddressV2)
                    .IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }
    }
}
