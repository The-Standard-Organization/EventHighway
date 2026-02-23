// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions;
using EventHighway.Core.Services.Foundations.Events.V1;
using EventHighway.Core.Services.Processings.Events.V1;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V1
{
    public partial class EventV1ProcessingServiceTests
    {
        private readonly Mock<IEventV1Service> eventV1ServiceMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventV1ProcessingService eventV1ProcessingService;

        public EventV1ProcessingServiceTests()
        {
            this.eventV1ServiceMock = new Mock<IEventV1Service>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventV1ProcessingService =
                new EventV1ProcessingService(
                    eventV1Service: this.eventV1ServiceMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1ValidationException(
                    someMessage,
                    someInnerException),

                new EventV1DependencyValidationException(
                    someMessage,
                    someInnerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1DependencyException(
                    someMessage,
                    someInnerException),

                new EventV1ServiceException(
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

        private static EventV1 CreateRandomEventV1()
        {
            return CreateEventV1Filler(
                dates: GetRandomDateTimeOffset(),
                eventV1Type: EventV1Type.Immediate)
                    .Create();
        }

        private static EventV1 CreateRandomEventV1(EventV1Type eventV1Type)
        {
            return CreateEventV1Filler(
                dates: GetRandomDateTimeOffset(),
                eventV1Type: eventV1Type)
                    .Create();
        }

        private static IQueryable<EventV1> CreateRandomEventV1s()
        {
            return CreateEventV1Filler(
                dates: GetRandomDateTimeOffset(),
                eventV1Type: EventV1Type.Immediate)
                    .Create(count: GetRandomNumber())
                        .AsQueryable();
        }

        private static IQueryable<EventV1> CreateRandomEventV1s(
            DateTimeOffset dates,
            EventV1Type eventV1Type)
        {
            return CreateEventV1Filler(
                dates,
                eventV1Type)
                    .Create(count: GetRandomNumber())
                        .AsQueryable();
        }

        private static DateTimeOffset GetRandomDateTimeOffset()
        {
            return new DateTimeRange(
                earliestDate: DateTime.UnixEpoch)
                    .GetValue();
        }

        private static Filler<EventV1> CreateEventV1Filler(
            DateTimeOffset dates,
            EventV1Type eventV1Type)
        {
            var filler = new Filler<EventV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnType<DateTimeOffset?>()
                    .Use(dates)

                .OnType<EventV1Type>().Use(eventV1Type);

            return filler;
        }
    }
}
