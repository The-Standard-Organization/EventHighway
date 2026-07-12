// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthArchivedEvents.V2
{
    public partial class HealthArchivedEventsV2OrchestrationServiceTests
    {
        private readonly Mock<IEventArchiveV2Service> eventArchiveV2ServiceMock;
        private readonly Mock<IListenerEventArchiveV2Service> listenerEventArchiveV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;

        private readonly IHealthArchivedEventsV2OrchestrationService
            healthArchivedEventsV2OrchestrationService;

        public HealthArchivedEventsV2OrchestrationServiceTests()
        {
            this.eventArchiveV2ServiceMock = new Mock<IEventArchiveV2Service>();
            this.listenerEventArchiveV2ServiceMock = new Mock<IListenerEventArchiveV2Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.healthArchivedEventsV2OrchestrationService =
                new HealthArchivedEventsV2OrchestrationService(
                    eventArchiveV2Service: this.eventArchiveV2ServiceMock.Object,
                    listenerEventArchiveV2Service: this.listenerEventArchiveV2ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyValidationError" });

            return new TheoryData<Xeption>
            {
                new EventArchiveV2ValidationException(someMessage, someInnerException),
                new EventArchiveV2DependencyValidationException(someMessage, someInnerException),
                new ListenerEventArchiveV2ValidationException(someMessage, someInnerException),
                new ListenerEventArchiveV2DependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventArchiveV2DependencyException(someMessage, someInnerException),
                new EventArchiveV2ServiceException(someMessage, someInnerException),
                new ListenerEventArchiveV2DependencyException(someMessage, someInnerException),
                new ListenerEventArchiveV2ServiceException(someMessage, someInnerException),
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

        private static List<EventArchiveV2> CreateRandomEventArchiveV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new EventArchiveV2
                {
                    Id = GetRandomId(),
                    EventAddressV2Id = GetRandomId(),
                    EventParticipantV2Id = GetRandomId(),
                    Status = GetRandomEnum<EventArchiveStatusV2>(),
                    Type = GetRandomEnum<EventArchiveTypeV2>(),
                    ContentHash = GetRandomString(),
                    ArchivedDate = GetRandomDateTimeOffset()
                })
                .ToList();

        private static List<ListenerEventArchiveV2> CreateRandomListenerEventArchiveV2s(int count) =>
            Enumerable.Range(start: 0, count: count)
                .Select(item => new ListenerEventArchiveV2
                {
                    Id = GetRandomId(),
                    EventAddressV2Id = GetRandomId(),
                    EventV2Id = GetRandomId(),
                    EventParticipantV2Id = GetRandomId(),
                    Status = GetRandomEnum<ListenerEventArchiveStatusV2>(),
                    RemainingRetryAttempts = GetRandomRetryCount(),
                    ArchivedDate = GetRandomDateTimeOffset()
                })
                .ToList();

        private static EventArchiveV2 CreateRandomEventArchiveV2WithArchivedDate(DateTimeOffset archivedDate)
        {
            EventArchiveV2 eventArchiveV2 = CreateRandomEventArchiveV2s(count: 1).Single();
            eventArchiveV2.ArchivedDate = archivedDate;

            return eventArchiveV2;
        }

        private static ListenerEventArchiveV2 CreateRandomListenerEventArchiveV2WithArchivedDate(
            DateTimeOffset archivedDate)
        {
            ListenerEventArchiveV2 listenerEventArchiveV2 = CreateRandomListenerEventArchiveV2s(count: 1).Single();
            listenerEventArchiveV2.ArchivedDate = archivedDate;

            return listenerEventArchiveV2;
        }
    }
}
