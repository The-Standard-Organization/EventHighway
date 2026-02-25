// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using EventHighway.Core.Services.Coordinations.Events.V1;
using EventHighway.Core.Services.Orchestrations.EventListeners.V1;
using EventHighway.Core.Services.Orchestrations.Events.V1;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V1
{
    public partial class EventV1CoordinationServiceTests
    {
        private readonly Mock<IEventV1OrchestrationService> eventV1OrchestrationServiceMock;
        private readonly Mock<IEventListenerV1OrchestrationService> eventListenerV1OrchestrationServiceMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ICompareLogic compareLogic;
        private readonly IEventV1CoordinationService eventV1CoordinationService;

        public EventV1CoordinationServiceTests()
        {
            this.eventV1OrchestrationServiceMock =
                new Mock<IEventV1OrchestrationService>();

            this.eventListenerV1OrchestrationServiceMock =
                new Mock<IEventListenerV1OrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>(
                behavior: MockBehavior.Strict);

            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            var compareConfiguration = new ComparisonConfig();

            compareConfiguration.IgnoreProperty<ListenerEventV1>(listenerEventV1 =>
                listenerEventV1.Id);

            this.compareLogic = new CompareLogic(compareConfiguration);

            this.eventV1CoordinationService =
                new EventV1CoordinationService(
                    eventV1OrchestrationService: this.eventV1OrchestrationServiceMock.Object,
                    eventListenerV1OrchestrationService: this.eventListenerV1OrchestrationServiceMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> EventV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventV1OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException)
            };
        }

        public static TheoryData<Xeption> EventV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV1OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventV1OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventListenerV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventListenerV1OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV1OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException)
            };
        }

        public static TheoryData<Xeption> EventListenerV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventListenerV1OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventListenerV1OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Exception> PlainException()
        {
            return new TheoryData<Exception> 
            { 
                new Exception() 
            };
        }

        public static TheoryData<DateTimeOffset, DateTimeOffset?> ScheduledDates()
        {
            int randomNegativeDays = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            DateTimeOffset scheduledNegativeDate =
                randomDateTimeOffset.AddDays(randomNegativeDays);

            return new TheoryData<DateTimeOffset, DateTimeOffset?>
            {
                {
                    randomDateTimeOffset,
                    scheduledNegativeDate
                },
                {
                    randomDateTimeOffset,
                    null
                }
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static IQueryable<EventV1> CreateRandomEventV1s()
        {
            return CreateEventV1Filler()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static EventV1 CreateRandomEventV1(int retryAttemtps = 0) =>
            CreateEventV1Filler(retryAttemtps).Create();

        private static IQueryable<EventListenerV1> CreateRandomEventListenerV1s() =>
            CreateEventListenerV1Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private Expression<Func<ListenerEventV1, bool>> SameListenerEventAs(
           ListenerEventV1 expectedListenerEventV1)
        {
            return actualListenerEventV1 =>
                this.compareLogic.Compare(
                    expectedListenerEventV1,
                    actualListenerEventV1)
                        .AreEqual;
        }

        private Expression<Func<EventCallV1, bool>> SameEventCallAs(
           EventCallV1 expectedEventCallV1)
        {
            return actualEventCallV1 =>
                this.compareLogic.Compare(
                    expectedEventCallV1,
                    actualEventCallV1)
                        .AreEqual;
        }

        private static Filler<EventV1> CreateEventV1Filler(
            int retryAttempts = 0)
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
                    eventV1.RetryAttempts).Use(retryAttempts)

                .OnProperty(eventV1 =>
                    eventV1.ListenerEvents).IgnoreIt();

            return filler;
        }

        private static Filler<EventListenerV1> CreateEventListenerV1Filler()
        {
            var filler = new Filler<EventListenerV1>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(eventListenerV1 =>
                    eventListenerV1.EventAddress).IgnoreIt()

                .OnProperty(eventListenerV1 =>
                    eventListenerV1.ListenerEvents).IgnoreIt();

            return filler;
        }
    }
}
