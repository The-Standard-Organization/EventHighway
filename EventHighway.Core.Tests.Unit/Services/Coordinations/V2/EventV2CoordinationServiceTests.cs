// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.Events.V2;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Services.Orchestrations.Events.V2;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        private readonly Mock<IEventV2OrchestrationService> eventV2OrchestrationServiceMock;
        private readonly Mock<IEventListenerV2OrchestrationService> eventListenerV2OrchestrationServiceMock;
        private readonly Mock<IJsonBroker> jsonBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ICompareLogic compareLogic;
        private readonly IEventV2CoordinationService eventV2CoordinationService;

        public EventV2CoordinationServiceTests()
        {
            this.eventV2OrchestrationServiceMock =
                new Mock<IEventV2OrchestrationService>();

            this.eventListenerV2OrchestrationServiceMock =
                new Mock<IEventListenerV2OrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.jsonBrokerMock =
                new Mock<IJsonBroker>();

            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>(
                behavior: MockBehavior.Strict);

            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            var compareConfiguration = new ComparisonConfig();

            compareConfiguration.IgnoreProperty<ListenerEventV2>(listenerEventV2 =>
                listenerEventV2.Id);

            this.compareLogic = new CompareLogic(compareConfiguration);

            this.eventV2CoordinationService =
                new EventV2CoordinationService(
                    eventV2OrchestrationService: this.eventV2OrchestrationServiceMock.Object,
                    eventListenerV2OrchestrationService: this.eventListenerV2OrchestrationServiceMock.Object,
                    jsonBroker: this.jsonBrokerMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new EventV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV2OrchestrationDependencyValidationException(
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
                new EventV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),

                new EventListenerV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventListenerV2OrchestrationServiceException(
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

        public static TheoryData<DateTimeOffset, DateTimeOffset?, string> InvalidContentWithScheduledDates()
        {
            DateTimeOffset fixedDateTimeOffset =
                new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

            return new TheoryData<DateTimeOffset, DateTimeOffset?, string>
            {
                { fixedDateTimeOffset, fixedDateTimeOffset.AddDays(-2), null },
                { fixedDateTimeOffset, null, null },
                { fixedDateTimeOffset, fixedDateTimeOffset.AddDays(-2), "not valid json" },
                { fixedDateTimeOffset, null, "not valid json" },
            };
        }

        public static TheoryData<DateTimeOffset, DateTimeOffset?> ScheduledDates()
        {
            DateTimeOffset fixedDateTimeOffset =
                new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

            return new TheoryData<DateTimeOffset, DateTimeOffset?>
            {
                {
                    fixedDateTimeOffset,
                    fixedDateTimeOffset.AddDays(-2)
                },
                {
                    fixedDateTimeOffset,
                    null
                }
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static IQueryable<EventV2> CreateRandomEventV2s()
        {
            return CreateEventV2Filler()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static EventV2 CreateRandomEventV2() =>
            CreateEventV2Filler().Create();

        private static IQueryable<EventListenerV2> CreateRandomEventListenerV2s() =>
            CreateEventListenerV2Filler().Create(count: GetRandomNumber())
                .Select(l => { l.PromotedProperties = null; l.FilterCriteria = null; return l; })
                    .AsQueryable();

        private static IQueryable<EventListenerV2> CreateRandomEventListenerV2s(int count) =>
            CreateEventListenerV2Filler().Create(count)
                .Select(l => { l.PromotedProperties = null; l.FilterCriteria = null; return l; })
                    .AsQueryable();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private Expression<Func<ListenerEventV2, bool>> SameListenerEventAs(
           ListenerEventV2 expectedListenerEventV2)
        {
            return actualListenerEventV2 =>
                this.compareLogic.Compare(
                    expectedListenerEventV2,
                    actualListenerEventV2)
                        .AreEqual;
        }

        private Expression<Func<EventCallV2, bool>> SameEventCallAs(
           EventCallV2 expectedEventCallV2)
        {
            return actualEventCallV2 =>
                this.compareLogic.Compare(
                    expectedEventCallV2,
                    actualEventCallV2)
                        .AreEqual;
        }

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(eventV2 =>
                    eventV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.ListenerEventV2s).IgnoreIt()

                .OnType<EventAddressV2>().IgnoreIt()
                .OnType<ListenerEventV2>().IgnoreIt();

            return filler;
        }

        private static IEnumerable<string> SplitPromotedPropertyKeys(string promotedProperties) =>
            string.IsNullOrWhiteSpace(promotedProperties)
                ? Array.Empty<string>()
                : promotedProperties.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private static Filler<EventListenerV2> CreateEventListenerV2Filler()
        {
            var filler = new Filler<EventListenerV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventListenerV2 =>
                    eventListenerV2.ListenerEventV2s).IgnoreIt()

                .OnType<EventAddressV2>().IgnoreIt()
                .OnType<EventV2>().IgnoreIt();

            return filler;
        }
    }
}
