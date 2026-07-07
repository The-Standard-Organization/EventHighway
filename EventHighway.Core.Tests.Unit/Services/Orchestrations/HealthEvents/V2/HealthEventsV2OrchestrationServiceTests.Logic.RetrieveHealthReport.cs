// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthEvents.V2
{
    public partial class HealthEventsV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveActiveEventsAndListenersHealthCheckItemsOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            List<EventV2> randomEvents = CreateRandomEventV2s(count: GetRandomNumber());
            List<ListenerEventV2> randomListenerEvents = CreateRandomListenerEventV2s(count: GetRandomNumber());

            long totalEvents = randomEvents.Count;

            long totalActive = randomEvents
                .Count(@event => @event.Status == EventStatusV2.Active);

            long totalImmediate = randomEvents
                .Count(@event => @event.Status == EventStatusV2.Active
                    && @event.Type == EventTypeV2.Immediate);

            long totalScheduled = randomEvents
                .Count(@event => @event.Status == EventStatusV2.Active
                    && @event.Type == EventTypeV2.Scheduled);

            long totalQuarantined = randomEvents
                .Count(@event => @event.Status == EventStatusV2.Quarantined);

            long totalListenerEvents = randomListenerEvents.Count;

            long totalSuccess = randomListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Success);

            long totalError = randomListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error);

            long totalActiveRetries = randomListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts > 0);

            long totalDead = randomListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts == 0);

            var expectedHealthCheckItems = new List<HealthCheckItemV2>
            {
                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Events",
                    Value = totalEvents.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of events created.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Active",
                    Value = totalActive.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of active events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Immediate",
                    Value = totalImmediate.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of active immediate events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Scheduled",
                    Value = totalScheduled.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of active scheduled events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Total Quarantined",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of quarantined events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Loops Detected",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Number of events quarantined by loop detection.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Events",
                    Item = "Duplicates Blocked",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Number of events blocked as duplicates.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Total Listener Events",
                    Value = totalListenerEvents.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of listener events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Total Success",
                    Value = FormatRateValue(totalSuccess, totalListenerEvents),
                    Description = "Listener events that completed successfully.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Total Error",
                    Value = FormatRateValue(totalError, totalListenerEvents),
                    Description = "Listener events that ended in an error state.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Active (Retries Left)",
                    Value = totalActiveRetries.ToString(CultureInfo.InvariantCulture),
                    Description = "Errored listener events with retry attempts remaining.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Active Listeners",
                    Item = "Dead (No Retries)",
                    Value = totalDead.ToString(CultureInfo.InvariantCulture),
                    Description = "Errored listener events with no retry attempts remaining.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                }
            };

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEvents.AsQueryable());

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEvents.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.HealthCheckItems.Should().BeEquivalentTo(expectedHealthCheckItems);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static string FormatRateValue(long count, long total)
        {
            decimal rate = total == 0
                ? 0
                : (decimal)count * 100 / total;

            return $"{count.ToString(CultureInfo.InvariantCulture)} " +
                $"({rate.ToString("0.00", CultureInfo.InvariantCulture)}%)";
        }
    }
}
