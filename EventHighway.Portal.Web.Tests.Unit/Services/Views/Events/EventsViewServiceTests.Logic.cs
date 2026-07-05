// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Portal.Web.Models.Views.Events;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Events
{
    public partial class EventsViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveArchivableEventCountAsync()
        {
            // given
            IQueryable<EventV2> storageEvents = new[]
            {
                CreateRandomEvent(EventStatusV2.Quarantined, remainingRetryAttempts: 3),
                CreateRandomEvent(EventStatusV2.Quarantined, remainingRetryAttempts: 0),
                CreateRandomEvent(EventStatusV2.Active, remainingRetryAttempts: 0),
                CreateRandomEvent(EventStatusV2.Active, remainingRetryAttempts: 0),
                CreateRandomEvent(EventStatusV2.Active, remainingRetryAttempts: 2)
            }.AsQueryable();

            int expectedCount = 2;

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageEvents);

            // when
            int actualCount =
                await this.eventsViewService.RetrieveArchivableEventCountAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualCount.Should().Be(expectedCount);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
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

            IQueryable<EventV2> storageEvents =
                new[] { oldest, newest, middle }.AsQueryable();

            List<EventView> expectedViews =
                new[] { newest, middle, oldest }.Select(MapToView).ToList();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventV2sWithEventAddressV2Async(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageEvents);

            // when
            List<EventView> actualViews =
                await this.eventsViewService.RetrieveAllEventsAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(
                expectedViews, options => options.WithStrictOrdering());

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventV2sWithEventAddressV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventByIdAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventV2 targetEvent = CreateRandomEvent(baseDate);
            Guid eventId = targetEvent.Id;

            IQueryable<EventV2> storageEvents = new[]
            {
                CreateRandomEvent(baseDate.AddDays(-1)),
                targetEvent,
                CreateRandomEvent(baseDate.AddDays(-2))
            }.AsQueryable();

            EventView expectedView = MapToView(targetEvent);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventV2sWithEventAddressV2Async(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageEvents);

            // when
            EventView actualView =
                await this.eventsViewService.RetrieveEventByIdAsync(
                    eventId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventV2sWithEventAddressV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
