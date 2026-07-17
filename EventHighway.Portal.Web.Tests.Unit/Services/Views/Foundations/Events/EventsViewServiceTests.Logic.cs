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
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
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
            IReadOnlyList<EventV2> storageEvents = new List<EventV2>
            {
                CreateRandomEvent(EventStatusV2.Quarantined),
                CreateRandomEvent(EventStatusV2.Quarantined)
            };

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

            EventV2 oldest = CreateRandomEvent(baseDate.AddDays(-2));
            EventV2 middle = CreateRandomEvent(baseDate.AddDays(-1));
            EventV2 newest = CreateRandomEvent(baseDate);

            IReadOnlyList<EventV2> storageEvents =
                new List<EventV2> { oldest, newest, middle };

            IQueryable<ListenerEventV2> storageListenerEvents = new List<ListenerEventV2>
            {
                CreateListenerEvent(newest.Id, ListenerEventStatusV2.Success),
                CreateListenerEvent(newest.Id, ListenerEventStatusV2.Error),
                CreateListenerEvent(middle.Id, ListenerEventStatusV2.Success)
            }.AsQueryable();

            List<EventView> expectedViews = new[] { newest, middle, oldest }
                .Select(@event => MapToView(@event, storageListenerEvents))
                .ToList();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventV2sWithEventAddressV2Async(
                    It.Is<EventV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(storageEvents);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageListenerEvents);

            // when
            List<EventView> actualViews =
                await this.eventsViewService.RetrieveAllEventsAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(
                expectedViews, options => options.WithStrictOrdering());

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventV2sWithEventAddressV2Async(
                    It.Is<EventV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventByIdAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventV2 otherEvent = CreateRandomEvent(baseDate.AddDays(-1));
            EventV2 targetEvent = CreateRandomEvent(baseDate);
            Guid eventId = targetEvent.Id;

            IReadOnlyList<EventV2> storageEvents =
                new List<EventV2> { otherEvent, targetEvent };

            IQueryable<ListenerEventV2> storageListenerEvents = new List<ListenerEventV2>
            {
                CreateListenerEvent(targetEvent.Id, ListenerEventStatusV2.Success),
                CreateListenerEvent(otherEvent.Id, ListenerEventStatusV2.Error)
            }.AsQueryable();

            EventView expectedView = MapToView(targetEvent, storageListenerEvents);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventV2sWithEventAddressV2Async(
                    It.Is<EventV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(storageEvents);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageListenerEvents);

            // when
            EventView actualView =
                await this.eventsViewService.RetrieveEventByIdAsync(
                    eventId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventV2sWithEventAddressV2Async(
                    It.Is<EventV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
