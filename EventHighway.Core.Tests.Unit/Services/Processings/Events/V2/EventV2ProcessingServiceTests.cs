// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.Retries;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        private readonly Mock<IEventV2Service> eventV2ServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventV2ProcessingService eventV2ProcessingService;

        public EventV2ProcessingServiceTests()
        {
            this.eventV2ServiceMock = new Mock<IEventV2Service>();
            this.configurationBrokerMock = new Mock<IConfigurationBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventV2ProcessingService =
                new EventV2ProcessingService(
                    eventV2Service: this.eventV2ServiceMock.Object,
                    configurationBroker: this.configurationBrokerMock.Object,
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
                new EventV2ValidationException(
                    someMessage,
                    someInnerException),

                new EventV2DependencyValidationException(
                    someMessage,
                    someInnerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventV2DependencyException(
                    someMessage,
                    someInnerException),

                new EventV2ServiceException(
                    someMessage,
                    someInnerException)
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

        private static int GetNegativeRandomNumber() =>
            -1 * GetRandomNumber();

        private static RetryConfiguration CreateRandomRetryConfiguration()
        {
            return new RetryConfiguration
            {
                RetryAttemptsAllowed = GetRandomNumber(),
                RetryBackoffMaxMinutes = GetRandomNumber(),
                DeadAfterMinutes = GetRandomNumber()
            };
        }

        private static EventV2 CreateRandomEventV2()
        {
            return CreateEventV2Filler(
                dates: TruncateToMicroseconds(GetRandomDateTimeOffset()),
                eventV2Type: EventTypeV2.Immediate)
                    .Create();
        }

        private static EventV2 CreateRandomEventV2(EventTypeV2 eventV2Type)
        {
            return CreateEventV2Filler(
                dates: TruncateToMicroseconds(GetRandomDateTimeOffset()),
                eventV2Type: eventV2Type)
                    .Create();
        }

        private static IQueryable<EventV2> CreateRandomEventV2s()
        {
            return CreateEventV2Filler(
                dates: TruncateToMicroseconds(GetRandomDateTimeOffset()),
                eventV2Type: EventTypeV2.Immediate)
                    .Create(count: GetRandomNumber())
                        .AsQueryable();
        }

        private static IQueryable<EventV2> CreateRandomEventV2s(
            DateTimeOffset dates,
            EventTypeV2 eventV2Type)
        {
            return CreateEventV2Filler(
                dates,
                eventV2Type)
                    .Create(count: GetRandomNumber())
                        .AsQueryable();
        }

        private static DateTimeOffset GetRandomDateTimeOffset()
        {
            return new DateTimeRange(
                earliestDate: DateTime.UnixEpoch)
                    .GetValue();
        }

        private static Filler<EventV2> CreateEventV2Filler(
            DateTimeOffset dates,
            EventTypeV2 eventV2Type)
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnType<DateTimeOffset?>()
                    .Use(dates)

                .OnType<EventTypeV2>().Use(eventV2Type)

                .OnProperty(eventV2 => eventV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(eventV2 => eventV2.ListenerEventV2s)
                    .IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }
        private static DateTimeOffset TruncateToMicroseconds(
            DateTimeOffset dateTimeOffset)
        {
            long ticksToRemove =
                dateTimeOffset.Ticks % TimeSpan.TicksPerMicrosecond;

            return dateTimeOffset.AddTicks(-ticksToRemove);
        }
    }
}
