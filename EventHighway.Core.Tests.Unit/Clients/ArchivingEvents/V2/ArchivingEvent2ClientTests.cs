// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using EventHighway.Core.Clients.ArchivingEvents.V2;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEvent2ClientTests
    {
        private readonly Mock<IArchivingEvent2OrchestrationService> archivingEvent2OrchestrationServiceMock;
        private readonly IArchivingEvent2Client archivingEvent2Client;

        public ArchivingEvent2ClientTests()
        {
            this.archivingEvent2OrchestrationServiceMock =
                new Mock<IArchivingEvent2OrchestrationService>();

            this.archivingEvent2Client =
                new ArchivingEvent2Client(
                    archivingEvent2OrchestrationService:
                        this.archivingEvent2OrchestrationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new ArchivingEvent2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new ArchivingEvent2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventV2 CreateRandomEventV2() =>
            CreateEventV2Filler().Create();

        private static IQueryable<EventV2> CreateRandomEventV2s() =>
            CreateEventV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(() => GetRandomDateTimeOffset())

                .OnProperty(eventV2 => eventV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(eventV2 => eventV2.ListenerEventV2s)
                    .IgnoreIt();

            return filler;
        }
    }
}
