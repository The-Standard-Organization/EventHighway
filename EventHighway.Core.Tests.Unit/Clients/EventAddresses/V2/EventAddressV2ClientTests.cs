// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using EventHighway.Core.Clients.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using EventHighway.Core.Services.Processings.EventAddresses.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V2
{
    public partial class EventAddressV2ClientTests
    {
        private readonly Mock<IEventAddressV2ProcessingService> eventAddressV2ProcessingServiceMock;
        private readonly IEventAddressV2Client eventAddressV2Client;

        public EventAddressV2ClientTests()
        {
            this.eventAddressV2ProcessingServiceMock =
                new Mock<IEventAddressV2ProcessingService>();

            this.eventAddressV2Client =
                new EventAddressV2Client(
                    eventAddressV2ProcessingService:
                        this.eventAddressV2ProcessingServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventAddressV2ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventAddressV2ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventAddressV2 CreateRandomEventAddressV2() =>
            CreateEventAddressV2Filler().Create();

        private static IQueryable<EventAddressV2> CreateRandomEventAddressV2s() =>
            CreateEventAddressV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<EventAddressV2> CreateEventAddressV2Filler()
        {
            var filler = new Filler<EventAddressV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(eventAddressV2 => eventAddressV2.Events)
                    .IgnoreIt()

                .OnProperty(eventAddressV2 => eventAddressV2.EventListenerV2s)
                    .IgnoreIt()

                .OnProperty(eventAddressV2 => eventAddressV2.ListenerEventV2s)
                    .IgnoreIt();

            return filler;
        }
    }
}
