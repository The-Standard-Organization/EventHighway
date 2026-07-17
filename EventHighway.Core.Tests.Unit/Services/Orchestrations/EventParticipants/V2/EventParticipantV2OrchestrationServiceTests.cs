// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Hashings;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2;
using EventHighway.Core.Services.Orchestrations.EventParticipants.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventParticipants.V2
{
    public partial class EventParticipantV2OrchestrationServiceTests
    {
        private readonly Mock<IEventParticipantV2Service> eventParticipantV2ServiceMock;
        private readonly Mock<IEventParticipantSecretV2Service> eventParticipantSecretV2ServiceMock;
        private readonly Mock<IHashBroker> hashBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventParticipantV2OrchestrationService eventParticipantV2OrchestrationService;

        public EventParticipantV2OrchestrationServiceTests()
        {
            this.eventParticipantV2ServiceMock =
                new Mock<IEventParticipantV2Service>();

            this.eventParticipantSecretV2ServiceMock =
                new Mock<IEventParticipantSecretV2Service>();

            this.hashBrokerMock =
                new Mock<IHashBroker>();

            this.dateTimeBrokerMock =
                new Mock<IDateTimeBroker>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.eventParticipantV2OrchestrationService =
                new EventParticipantV2OrchestrationService(
                    eventParticipantV2Service: this.eventParticipantV2ServiceMock.Object,
                    eventParticipantSecretV2Service: this.eventParticipantSecretV2ServiceMock.Object,
                    hashBroker: this.hashBrokerMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventParticipantV2ValidationException(
                    someMessage,
                    someInnerException),

                new EventParticipantV2DependencyValidationException(
                    someMessage,
                    someInnerException),

                new EventParticipantSecretV2ValidationException(
                    someMessage,
                    someInnerException),

                new EventParticipantSecretV2DependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventParticipantV2DependencyException(
                    someMessage,
                    someInnerException),

                new EventParticipantV2ServiceException(
                    someMessage,
                    someInnerException),

                new EventParticipantSecretV2DependencyException(
                    someMessage,
                    someInnerException),

                new EventParticipantSecretV2ServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventV2 CreateRandomEventV2() =>
            CreateEventV2Filler().Create();

        private static EventParticipantV2 CreateRandomEventParticipantV2() =>
            CreateEventParticipantV2Filler(dates: GetRandomDateTimeOffset()).Create();

        private static EventParticipantSecretV2 CreateRandomEventParticipantSecretV2() =>
            CreateEventParticipantSecretV2Filler(dates: GetRandomDateTimeOffset()).Create();

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())

                .OnProperty(eventV2 => eventV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(eventV2 => eventV2.ListenerEventV2s)
                    .IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }

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
