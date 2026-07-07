// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Globalization;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.HealthArchivedEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthEvents.V2;
using EventHighway.Core.Services.Orchestrations.HealthInfrastructures.V2;
using Moq;
using Tynamix.ObjectFiller;

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

        private static DateTimeOffset GetRandomPeriodAlignedWindowStart(TrafficPeriodV2 period)
        {
            DateTimeOffset randomDate = GetRandomDateTimeOffset();

            switch (period)
            {
                case TrafficPeriodV2.Month:
                    return new DateTimeOffset(randomDate.Year, randomDate.Month, 1, 0, 0, 0, TimeSpan.Zero);

                case TrafficPeriodV2.Year:
                    return new DateTimeOffset(randomDate.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);

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
                    return new DateTimeOffset(windowStart.Year, windowStart.Month, 1, 0, 0, 0, TimeSpan.Zero)
                        .AddMonths(1);

                case TrafficPeriodV2.Year:
                    return new DateTimeOffset(windowStart.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);

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
                case TrafficPeriodV2.Week:
                    return $"{windowStart.ToString("dd MMM", CultureInfo.InvariantCulture)} – " +
                        $"{windowEnd.AddDays(-1).ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}";

                case TrafficPeriodV2.Month:
                    return windowStart.ToString("MMM yyyy", CultureInfo.InvariantCulture);

                case TrafficPeriodV2.Year:
                    return windowStart.Year.ToString(CultureInfo.InvariantCulture);

                default:
                    return windowStart.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
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
    }
}
