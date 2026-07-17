// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Events;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.Events
{
    public partial class EventsViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveArchivableEventCountAsync()
        {
            // given
            IQueryable<EventV2> storageEvents = new[]
            {
                CreateRandomEvent(EventStatusV2.Quarantined),
                CreateRandomEvent(EventStatusV2.Quarantined)
            }.AsQueryable();

            int expectedCount = 2;

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventV2sAsync(
                    It.Is<EventV2Query>(query =>
                        query.Status == EventStatusV2.Quarantined
                            && query.Take == 1000),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(storageEvents);

            // when
            int actualCount =
                await this.eventsViewService.RetrieveArchivableEventCountAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualCount.Should().Be(expectedCount);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventV2sAsync(
                    It.Is<EventV2Query>(query =>
                        query.Status == EventStatusV2.Quarantined
                            && query.Take == 1000),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveAllEventsMostRecentFirstAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventV2Summary oldest = CreateRandomEventSummary(baseDate.AddDays(-2));
            EventV2Summary middle = CreateRandomEventSummary(baseDate.AddDays(-1));
            EventV2Summary newest = CreateRandomEventSummary(baseDate);

            List<EventV2Summary> storageEventSummaries =
                new List<EventV2Summary> { oldest, newest, middle };

            List<EventView> expectedViews =
                new[] { newest, middle, oldest }.Select(MapToView).ToList();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventV2SummariesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageEventSummaries);

            // when
            List<EventView> actualViews =
                await this.eventsViewService.RetrieveAllEventsAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(
                expectedViews, options => options.WithStrictOrdering());

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventV2SummariesAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventByIdAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventV2Summary targetEventSummary = CreateRandomEventSummary(baseDate);
            Guid eventId = targetEventSummary.Id;

            EventView expectedView = MapToView(targetEventSummary);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventV2SummaryByIdAsync(eventId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(targetEventSummary);

            // when
            EventView actualView =
                await this.eventsViewService.RetrieveEventByIdAsync(
                    eventId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventV2SummaryByIdAsync(eventId, It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
