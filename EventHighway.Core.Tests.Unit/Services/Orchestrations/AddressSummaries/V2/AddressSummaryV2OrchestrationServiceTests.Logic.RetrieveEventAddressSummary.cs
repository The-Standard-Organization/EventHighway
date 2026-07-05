// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.AddressSummaries.V2
{
    public partial class AddressSummaryV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventAddressSummaryV2ForWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            var expectedWindowEnd = new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero);
            Guid addressAId = Guid.NewGuid();
            Guid addressBId = Guid.NewGuid();
            string addressAName = GetRandomString();
            string addressADescription = GetRandomString();

            var addresses = new[]
            {
                CreateEventAddressV2(addressAId, addressAName, addressADescription),
                CreateEventAddressV2(addressBId, GetRandomString(), GetRandomString())
            }.AsQueryable();

            var listeners = new[]
            {
                CreateEventListenerV2ForAddress(addressAId),
                CreateEventListenerV2ForAddress(addressAId),
                CreateEventListenerV2ForAddress(addressBId)
            }.AsQueryable();

            var events = new[]
            {
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(1), "h1", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(2), "h1", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(3), "h2", 0, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(4), "h3", 5, EventStatusV2.Quarantined),
                CreateEventV2ForAddress(addressAId, windowStart.AddDays(-1), "h4", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressBId, windowStart.AddHours(1), "b1", 5, EventStatusV2.Active)
            }.AsQueryable();

            var listenerEvents = new[]
            {
                CreateListenerEventV2ForAddress(addressAId, windowStart.AddHours(1), ListenerEventStatusV2.Success),
                CreateListenerEventV2ForAddress(addressAId, windowStart.AddHours(2), ListenerEventStatusV2.Success),
                CreateListenerEventV2ForAddress(addressAId, windowStart.AddHours(3), ListenerEventStatusV2.Success),
                CreateListenerEventV2ForAddress(addressAId, windowStart.AddHours(5), ListenerEventStatusV2.Error),
                CreateListenerEventV2ForAddress(addressAId, windowStart.AddDays(-1), ListenerEventStatusV2.Success)
            }.AsQueryable();

            var archivedEvents = new[]
            {
                CreateEventArchiveV2ForAddress(addressAId, windowStart.AddHours(6)),
                CreateEventArchiveV2ForAddress(addressAId, windowStart.AddHours(7)),
                CreateEventArchiveV2ForAddress(addressAId, windowStart.AddDays(-2))
            }.AsQueryable();

            var archivedListenerEvents = new[]
            {
                CreateListenerEventArchiveV2ForAddress(addressAId, windowStart.AddHours(8))
            }.AsQueryable();

            var addressesWithListeners = AttachEventListenerV2s(addresses, listeners);
            var eventsWithListenerEvents = AttachListenerEventV2s(events, listenerEvents);
            var archivesWithListenerArchives =
                AttachListenerEventArchiveV2s(archivedEvents, archivedListenerEvents);

            SetupAddressSummaryFoundationMocks(
                randomCancellationToken,
                addressesWithListeners,
                eventsWithListenerEvents,
                archivesWithListenerArchives);

            // when
            var actualSummaries =
                (await this.addressSummaryV2OrchestrationService
                    .RetrieveEventAddressSummaryV2Async(
                        TrafficPeriodV2.Day, windowStart, randomCancellationToken))
                    .ToList();

            // then
            actualSummaries.Should().HaveCount(2);

            EventAddressSummaryV2 summaryA =
                actualSummaries.Single(s => s.EventAddressV2Id == addressAId);

            summaryA.Name.Should().Be(addressAName);
            summaryA.Description.Should().Be(addressADescription);
            summaryA.Period.Should().Be(TrafficPeriodV2.Day);
            summaryA.WindowStart.Should().Be(windowStart);
            summaryA.WindowEnd.Should().Be(expectedWindowEnd);
            summaryA.WindowLabel.Should().Be("24 Jun 2026");
            summaryA.TotalActiveEvents.Should().Be(4);
            summaryA.TotalArchivedEvents.Should().Be(2);
            summaryA.TotalListenerEvents.Should().Be(4);
            summaryA.TotalArchivedListenerEvents.Should().Be(1);
            summaryA.ActiveListeners.Should().Be(2);
            summaryA.DeadEvents.Should().Be(0);
            summaryA.LoopsDetected.Should().Be(1);
            summaryA.ErrorRate.Should().Be(25m);
            summaryA.DuplicateRate.Should().Be(25m);
            summaryA.Status.Should().Be(HealthStatusV2.Red);
            summaryA.LastActivity.Should().Be(windowStart.AddHours(5));

            EventAddressSummaryV2 summaryB =
                actualSummaries.Single(s => s.EventAddressV2Id == addressBId);

            summaryB.TotalActiveEvents.Should().Be(1);
            summaryB.ActiveListeners.Should().Be(1);

            VerifyAddressSummaryFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnEmptyEventAddressSummaryV2WhenNoAddressesExistAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            var emptyAddresses = Array.Empty<EventAddressV2>().AsQueryable();
            var emptyEvents = Array.Empty<EventV2>().AsQueryable();
            var emptyListeners = Array.Empty<EventListenerV2>().AsQueryable();
            var emptyListenerEvents = Array.Empty<ListenerEventV2>().AsQueryable();
            var emptyArchivedEvents = Array.Empty<EventArchiveV2>().AsQueryable();
            var emptyArchivedListenerEvents = Array.Empty<ListenerEventArchiveV2>().AsQueryable();

            SetupAddressSummaryFoundationMocks(
                randomCancellationToken,
                AttachEventListenerV2s(emptyAddresses, emptyListeners),
                AttachListenerEventV2s(emptyEvents, emptyListenerEvents),
                AttachListenerEventArchiveV2s(emptyArchivedEvents, emptyArchivedListenerEvents));

            // when
            var actualSummaries =
                await this.addressSummaryV2OrchestrationService
                    .RetrieveEventAddressSummaryV2Async(
                        TrafficPeriodV2.Day, windowStart, randomCancellationToken);

            // then
            actualSummaries.Should().BeEmpty();

            VerifyAddressSummaryFoundationMocksOnce(randomCancellationToken);
        }
    }
}
