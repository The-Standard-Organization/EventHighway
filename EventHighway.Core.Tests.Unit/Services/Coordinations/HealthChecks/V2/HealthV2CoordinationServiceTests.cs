// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.HealthArchivedEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.HealthEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.HealthInfrastructures.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        private readonly Mock<IHealthInfrastructureV2OrchestrationService>
            healthInfrastructureV2OrchestrationServiceMock;

        private readonly Mock<IHealthEventsV2OrchestrationService> healthEventsV2OrchestrationServiceMock;

        private readonly Mock<IHealthArchivedEventsV2OrchestrationService>
            healthArchivedEventsV2OrchestrationServiceMock;

        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IHealthV2CoordinationService healthV2CoordinationService;

        public HealthV2CoordinationServiceTests()
        {
            this.healthInfrastructureV2OrchestrationServiceMock =
                new Mock<IHealthInfrastructureV2OrchestrationService>(MockBehavior.Strict);

            this.healthEventsV2OrchestrationServiceMock =
                new Mock<IHealthEventsV2OrchestrationService>(MockBehavior.Strict);

            this.healthArchivedEventsV2OrchestrationServiceMock =
                new Mock<IHealthArchivedEventsV2OrchestrationService>(MockBehavior.Strict);

            this.configurationBrokerMock = new Mock<IConfigurationBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.healthV2CoordinationService =
                new HealthV2CoordinationService(
                    healthInfrastructureV2OrchestrationService:
                        this.healthInfrastructureV2OrchestrationServiceMock.Object,

                    healthEventsV2OrchestrationService:
                        this.healthEventsV2OrchestrationServiceMock.Object,

                    healthArchivedEventsV2OrchestrationService:
                        this.healthArchivedEventsV2OrchestrationServiceMock.Object,

                    configurationBroker: this.configurationBrokerMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyValidationError" });

            return new TheoryData<Xeption>
            {
                new HealthInfrastructureV2OrchestrationDependencyValidationException(
                    someMessage, someInnerException),

                new HealthEventsV2OrchestrationDependencyValidationException(someMessage, someInnerException),

                new HealthArchivedEventsV2OrchestrationDependencyValidationException(
                    someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new HealthInfrastructureV2OrchestrationDependencyException(someMessage, someInnerException),
                new HealthInfrastructureV2OrchestrationServiceException(someMessage, someInnerException),
                new HealthEventsV2OrchestrationDependencyException(someMessage, someInnerException),
                new HealthEventsV2OrchestrationServiceException(someMessage, someInnerException),
                new HealthArchivedEventsV2OrchestrationDependencyException(someMessage, someInnerException),
                new HealthArchivedEventsV2OrchestrationServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

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

        private static string BuildExpectedWindowLabel(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd)
        {
            switch (period)
            {
                case TrafficPeriodV2.Day:
                    return $"{windowStart.ToString("dd MMM yyyy HH:00", CultureInfo.InvariantCulture)} – " +
                        $"{windowEnd.ToString("dd MMM yyyy HH:00", CultureInfo.InvariantCulture)}";

                default:
                    return $"{windowStart.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)} – " +
                        $"{windowEnd.AddDays(-1).ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}";
            }
        }

        private static HealthCheckItemV2 CreateHealthCheckItem(string grouping, string item, string value)
        {
            return new HealthCheckItemV2
            {
                Grouping = grouping,
                Item = item,
                Value = value,
                Description = $"{grouping}-{item}",
                StatusCode = (int)HealthStatusV2.NA,
                Status = nameof(HealthStatusV2.NA)
            };
        }

        private static HealthCheckItemV2 CreateScoredHealthCheckItem(
            string grouping, string item, string value, HealthStatusV2 status)
        {
            HealthCheckItemV2 healthCheckItem = CreateHealthCheckItem(grouping, item, value);
            healthCheckItem.StatusCode = (int)status;
            healthCheckItem.Status = status.ToString();

            return healthCheckItem;
        }

        private static List<(DateTimeOffset Start, string Label)> GetExpectedBucketStarts(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd)
        {
            var bucketStarts = new List<(DateTimeOffset Start, string Label)>();

            switch (period)
            {
                case TrafficPeriodV2.Week:
                    for (int day = 0; day < 7; day++)
                    {
                        DateTimeOffset start = windowStart.AddDays(day);
                        bucketStarts.Add((start, start.ToString("ddd", CultureInfo.InvariantCulture)));
                    }

                    break;

                case TrafficPeriodV2.Month:
                    for (DateTimeOffset start = windowStart; start < windowEnd; start = start.AddDays(1))
                    {
                        bucketStarts.Add((start, start.ToString("dd", CultureInfo.InvariantCulture)));
                    }

                    break;

                case TrafficPeriodV2.Year:
                    for (int month = 0; month < 12; month++)
                    {
                        DateTimeOffset start = windowStart.AddMonths(month);
                        bucketStarts.Add((start, start.ToString("MMM", CultureInfo.InvariantCulture)));
                    }

                    break;

                // The custom tests use a 5-day span, which resolves to daily buckets.
                case TrafficPeriodV2.Custom:
                    for (DateTimeOffset start = windowStart; start < windowEnd; start = start.AddDays(1))
                    {
                        bucketStarts.Add((start, start.ToString("dd MMM", CultureInfo.InvariantCulture)));
                    }

                    break;

                default:
                    for (int hour = 0; hour < 24; hour++)
                    {
                        DateTimeOffset start = windowStart.AddHours(hour);
                        bucketStarts.Add((start, start.ToString("HH:00", CultureInfo.InvariantCulture)));
                    }

                    break;
            }

            return bucketStarts;
        }

        private static TrafficBucketV2 CreateRandomTrafficBucket(DateTimeOffset periodStart) =>
            new TrafficBucketV2
            {
                PeriodStart = periodStart,
                Events = GetRandomNumber(),
                ImmediateEvents = GetRandomNumber(),
                ScheduledEvents = GetRandomNumber(),
                ListenerEvents = GetRandomNumber(),
                Success = GetRandomNumber(),
                Errors = GetRandomNumber(),
                Pending = GetRandomNumber(),
                Replays = GetRandomNumber()
            };

        private static TrafficSnapshotV2 CreateRandomTrafficSnapshot(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            IEnumerable<TrafficBucketV2> buckets) =>
            new TrafficSnapshotV2
            {
                Period = period,
                WindowStart = windowStart,
                WindowEnd = windowEnd,
                TotalEvents = GetRandomNumber(),
                TotalListenerEvents = GetRandomNumber(),
                TotalSuccess = GetRandomNumber(),
                TotalErrors = GetRandomNumber(),
                TotalPending = GetRandomNumber(),
                TotalReplays = GetRandomNumber(),
                Buckets = buckets
            };

        private static EventAddressUsageV2 CreateNameAddressUsage(Guid eventAddressV2Id, string name) =>
            new EventAddressUsageV2
            {
                EventAddressV2Id = eventAddressV2Id,
                Name = name,
                Description = GetRandomString(),
                ActiveListeners = GetRandomNumber()
            };

        private static ParticipantUsageV2 CreateNameParticipantUsage(Guid eventParticipantV2Id, string name) =>
            new ParticipantUsageV2
            {
                EventParticipantV2Id = eventParticipantV2Id,
                Name = name,
                IsActive = true,
                OwnedListeners = GetRandomNumber()
            };
    }
}
