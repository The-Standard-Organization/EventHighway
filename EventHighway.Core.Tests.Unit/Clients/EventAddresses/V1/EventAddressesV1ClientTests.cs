// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using EventHighway.Core.Clients.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;
using EventHighway.Core.Services.Foundations.EventAddresses.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V1
{
    public partial class EventAddressesV1ClientTests
    {
        private readonly Mock<IEventAddressV1Service> eventAddressV1ServiceMock;
        private readonly IEventAddressesV1Client eventAddressV1sClient;

        public EventAddressesV1ClientTests()
        {
            this.eventAddressV1ServiceMock =
                new Mock<IEventAddressV1Service>();

            this.eventAddressV1sClient =
                new EventAddressesV1Client(
                    eventAddressV1Service: this.eventAddressV1ServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventAddressV1ValidationException(
                    someMessage,
                    someInnerException),

                new EventAddressV1DependencyValidationException(
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

        private static EventAddressV1 CreateRandomEventAddressV1() =>
            CreateEventAddressV1Filler().Create();

        private static IQueryable<EventAddressV1> CreateRandomEventAddressV1s() =>
            CreateEventAddressV1Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<EventAddressV1> CreateEventAddressV1Filler()
        {
            var filler = new Filler<EventAddressV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(eventAddressV1 => eventAddressV1.Events)
                    .IgnoreIt()

                .OnProperty(eventAddressV1 => eventAddressV1.EventListenerV1s)
                    .IgnoreIt()

                .OnProperty(eventAddressV1 => eventAddressV1.ListenerEventV1s)
                    .IgnoreIt();

            return filler;
        }
    }
}
