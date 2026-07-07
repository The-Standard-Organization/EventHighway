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
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.HealthArchivedEvents.V2
{
    public partial class HealthArchivedEventsV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveArchivedEventsAndListenersHealthCheckItemsOnRetrieveHealthReportV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 inputPeriod = GetRandomEnum<TrafficPeriodV2>();
            DateTimeOffset inputWindowStart = GetRandomDateTimeOffset();

            List<EventArchiveV2> randomArchivedEvents =
                CreateRandomEventArchiveV2s(count: GetRandomNumber());

            List<ListenerEventArchiveV2> randomArchivedListenerEvents =
                CreateRandomListenerEventArchiveV2s(count: GetRandomNumber());

            long totalEvents = randomArchivedEvents.Count;

            long totalQuarantined = randomArchivedEvents
                .Count(archivedEvent => archivedEvent.Status == EventArchiveStatusV2.Quarantined);

            long totalListenerEvents = randomArchivedListenerEvents.Count;

            long totalSuccess = randomArchivedListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Success);

            long totalError = randomArchivedListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error);

            long totalActiveRetries = randomArchivedListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts > 0);

            long totalDead = randomArchivedListenerEvents
                .Count(listenerEvent => listenerEvent.Status == ListenerEventArchiveStatusV2.Error
                    && listenerEvent.RemainingRetryAttempts == 0);

            var expectedHealthCheckItems = new List<HealthCheckItemV2>
            {
                new HealthCheckItemV2
                {
                    Grouping = "Archived Events",
                    Item = "Total Events",
                    Value = totalEvents.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of archived events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Events",
                    Item = "Total Quarantined",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of quarantined archived events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Events",
                    Item = "Loops Detected",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Number of archived events quarantined by loop detection.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Events",
                    Item = "Duplicates Blocked",
                    Value = totalQuarantined.ToString(CultureInfo.InvariantCulture),
                    Description = "Number of archived events blocked as duplicates.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Total Listener Events",
                    Value = totalListenerEvents.ToString(CultureInfo.InvariantCulture),
                    Description = "Total number of archived listener events.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Total Success",
                    Value = FormatRateValue(totalSuccess, totalListenerEvents),
                    Description = "Archived listener events that completed successfully.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Total Error",
                    Value = FormatRateValue(totalError, totalListenerEvents),
                    Description = "Archived listener events that ended in an error state.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Active (Retries Left)",
                    Value = totalActiveRetries.ToString(CultureInfo.InvariantCulture),
                    Description = "Errored archived listener events with retry attempts remaining.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                },

                new HealthCheckItemV2
                {
                    Grouping = "Archived Listeners",
                    Item = "Dead (No Retries)",
                    Value = totalDead.ToString(CultureInfo.InvariantCulture),
                    Description = "Errored archived listener events with no retry attempts remaining.",
                    StatusCode = (int)HealthStatusV2.NA,
                    Status = nameof(HealthStatusV2.NA)
                }
            };

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedEvents.AsQueryable());

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomArchivedListenerEvents.AsQueryable());

            // when
            HealthReportV2 actualHealthReport =
                await this.healthArchivedEventsV2OrchestrationService
                    .RetrieveHealthReportV2Async(
                        inputPeriod, inputWindowStart, randomCancellationToken);

            // then
            actualHealthReport.Period.Should().Be(inputPeriod);
            actualHealthReport.WindowStart.Should().Be(inputWindowStart);
            actualHealthReport.HealthCheckItems.Should().BeEquivalentTo(expectedHealthCheckItems);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
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
