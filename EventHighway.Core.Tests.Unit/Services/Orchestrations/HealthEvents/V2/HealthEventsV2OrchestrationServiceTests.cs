// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthEvents.V2
{
    public partial class HealthEventsV2OrchestrationServiceTests
    {
        private readonly Mock<IEventV2Service> eventV2ServiceMock;
        private readonly Mock<IListenerEventV2Service> listenerEventV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IHealthEventsV2OrchestrationService healthEventsV2OrchestrationService;

        public HealthEventsV2OrchestrationServiceTests()
        {
            this.eventV2ServiceMock = new Mock<IEventV2Service>();
            this.listenerEventV2ServiceMock = new Mock<IListenerEventV2Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.healthEventsV2OrchestrationService =
                new HealthEventsV2OrchestrationService(
                    eventV2Service: this.eventV2ServiceMock.Object,
                    listenerEventV2Service: this.listenerEventV2ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyValidationError" });

            return new TheoryData<Xeption>
            {
                new EventV2ValidationException(someMessage, someInnerException),
                new EventV2DependencyValidationException(someMessage, someInnerException),
                new ListenerEventV2ValidationException(someMessage, someInnerException),
                new ListenerEventV2DependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventV2DependencyException(someMessage, someInnerException),
                new EventV2ServiceException(someMessage, someInnerException),
                new ListenerEventV2DependencyException(someMessage, someInnerException),
                new ListenerEventV2ServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static int GetRandomRetryCount() =>
            new IntRange(min: 0, max: 4).GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static T GetRandomEnum<T>() where T : struct, Enum
        {
            T[] enumValues = Enum.GetValues<T>();

            return enumValues[new IntRange(min: 0, max: enumValues.Length - 1).GetValue()];
        }

        // Custom is excluded: it requires an explicit window end and is exercised by dedicated tests.
        private static TrafficPeriodV2 GetRandomTrafficPeriod()
        {
            TrafficPeriodV2[] standardPeriods = new[]
            {
                TrafficPeriodV2.Day,
                TrafficPeriodV2.Week,
                TrafficPeriodV2.Month,
                TrafficPeriodV2.Year
            };

            return standardPeriods[new IntRange(min: 0, max: standardPeriods.Length - 1).GetValue()];
        }

        private static DateTimeOffset GetRandomPeriodAlignedWindowStart(TrafficPeriodV2 period)
        {
            DateTimeOffset randomDate = GetRandomDateTimeOffset();

            switch (period)
            {
                case TrafficPeriodV2.Year:
                    return new DateTimeOffset(randomDate.Year, randomDate.Month, 1, 0, 0, 0, TimeSpan.Zero);

                case TrafficPeriodV2.Day:
                    return new DateTimeOffset(
                        randomDate.Year, randomDate.Month, randomDate.Day, randomDate.Hour, 0, 0, TimeSpan.Zero);

                default:
                    return new DateTimeOffset(
                        randomDate.Year, randomDate.Month, randomDate.Day, 0, 0, 0, TimeSpan.Zero);
            }
        }

        private static DateTimeOffset GetWindowEnd(TrafficPeriodV2 period, DateTimeOffset windowStart)
        {
            switch (period)
            {
                case TrafficPeriodV2.Week:
                    return windowStart.AddDays(7);

                case TrafficPeriodV2.Month:
                    return windowStart.AddMonths(1);

                case TrafficPeriodV2.Year:
                    return windowStart.AddYears(1);

                default:
                    return windowStart.AddHours(24);
            }
        }

        private static List<EventV2> CreateRandomEventV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new EventV2
                {
                    Id = GetRandomId(),
                    EventAddressV2Id = GetRandomId(),
                    EventParticipantV2Id = GetRandomId(),
                    Status = GetRandomEnum<EventStatusV2>(),
                    Type = GetRandomEnum<EventTypeV2>(),
                    ContentHash = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset()
                })
                .ToList();

        private static List<ListenerEventV2> CreateRandomListenerEventV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new ListenerEventV2
                {
                    Id = GetRandomId(),
                    EventAddressV2Id = GetRandomId(),
                    EventV2Id = GetRandomId(),
                    EventParticipantV2Id = GetRandomId(),
                    Status = GetRandomEnum<ListenerEventStatusV2>(),
                    RemainingRetryAttempts = GetRandomRetryCount(),
                    CreatedDate = GetRandomDateTimeOffset()
                })
                .ToList();

        private static EventV2 CreateRandomEventV2WithCreatedDate(DateTimeOffset createdDate)
        {
            EventV2 eventV2 = CreateRandomEventV2s(count: 1).Single();
            eventV2.CreatedDate = createdDate;

            return eventV2;
        }

        private static ListenerEventV2 CreateRandomListenerEventV2WithCreatedDate(DateTimeOffset createdDate)
        {
            ListenerEventV2 listenerEventV2 = CreateRandomListenerEventV2s(count: 1).Single();
            listenerEventV2.CreatedDate = createdDate;

            return listenerEventV2;
        }
    }
}
