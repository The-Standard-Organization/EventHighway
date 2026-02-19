// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Clients.Events.V1;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Services.Coordinations.Events.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.Events.V1
{
    public partial class EventV1sClientV1Tests
    {
        private readonly Mock<IEventV1CoordinationServiceV1> eventV1CoordinationServiceV1Mock;
        private readonly IEventV1sClientV1 eventV1SClientV1;

        public EventV1sClientV1Tests()
        {
            this.eventV1CoordinationServiceV1Mock =
                new Mock<IEventV1CoordinationServiceV1>();

            this.eventV1SClientV1 =
                new EventV1sClientV1(
                    eventV1CoordinationServiceV1:
                        this.eventV1CoordinationServiceV1Mock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1CoordinationValidationException(
                    someMessage,
                    someInnerException),

                new EventV1CoordinationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventV1 CreateRandomEventV1() =>
            CreateEventV1Filler().Create();

        private static Filler<EventV1> CreateEventV1Filler()
        {
            var filler = new Filler<EventV1>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(eventV1 =>
                    eventV1.EventAddress).IgnoreIt()

                .OnProperty(eventV1 =>
                    eventV1.ListenerEvents).IgnoreIt();

            return filler;
        }
    }
}
