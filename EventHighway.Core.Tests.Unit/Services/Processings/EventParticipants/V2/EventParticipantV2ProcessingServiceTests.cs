// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Processings.EventParticipants.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventParticipants.V2
{
    public partial class EventParticipantV2ProcessingServiceTests
    {
        private readonly Mock<IEventParticipantV2Service> eventParticipantV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventParticipantV2ProcessingService eventParticipantV2ProcessingService;

        public EventParticipantV2ProcessingServiceTests()
        {
            this.eventParticipantV2ServiceMock =
                new Mock<IEventParticipantV2Service>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.eventParticipantV2ProcessingService =
                new EventParticipantV2ProcessingService(
                    eventParticipantV2Service: eventParticipantV2ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
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
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static IQueryable<EventParticipantV2> CreateRandomEventParticipantV2s() =>
            CreateEventParticipantV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static EventParticipantV2 CreateRandomEventParticipantV2() =>
            CreateEventParticipantV2Filler().Create();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<EventParticipantV2> CreateEventParticipantV2Filler()
        {
            var filler = new Filler<EventParticipantV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

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
