// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventArchives
{
    public partial class EventArchivesViewServiceTests
    {
        [Fact]
        public async Task ShouldArchiveProcessedEventsAsync()
        {
            // given . when
            await this.eventArchivesViewService.ArchiveProcessedEventsAsync(
                TestContext.Current.CancellationToken);

            // then
            this.eventHighwayBrokerMock.Verify(broker =>
                broker.ArchiveEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPurgeArchivesOlderThanAsync()
        {
            // given
            DateTimeOffset olderThan = GetRandomDateTimeOffset();

            // when
            await this.eventArchivesViewService.PurgeArchivesOlderThanAsync(
                olderThan, TestContext.Current.CancellationToken);

            // then
            this.eventHighwayBrokerMock.Verify(broker =>
                broker.PurgeEventArchiveV2sAsync(
                    olderThan, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveAllEventArchivesMostRecentFirstAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventArchiveV2Summary oldest = CreateRandomEventArchiveSummary(baseDate.AddDays(-2));
            EventArchiveV2Summary middle = CreateRandomEventArchiveSummary(baseDate.AddDays(-1));
            EventArchiveV2Summary newest = CreateRandomEventArchiveSummary(baseDate);

            List<EventArchiveV2Summary> storageEventArchiveSummaries =
                new List<EventArchiveV2Summary> { oldest, newest, middle };

            List<EventArchiveView> expectedViews =
                new[] { newest, middle, oldest }.Select(MapToView).ToList();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventArchiveV2SummariesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageEventArchiveSummaries);

            // when
            List<EventArchiveView> actualViews =
                await this.eventArchivesViewService.RetrieveAllEventArchivesAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(
                expectedViews, options => options.WithStrictOrdering());

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventArchiveV2SummariesAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventArchiveByIdAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventArchiveV2Summary targetEventArchiveSummary =
                CreateRandomEventArchiveSummary(baseDate);

            Guid eventArchiveId = targetEventArchiveSummary.Id;
            EventArchiveView expectedView = MapToView(targetEventArchiveSummary);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveEventArchiveV2SummaryByIdAsync(
                    eventArchiveId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(targetEventArchiveSummary);

            // when
            EventArchiveView actualView =
                await this.eventArchivesViewService.RetrieveEventArchiveByIdAsync(
                    eventArchiveId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveEventArchiveV2SummaryByIdAsync(
                    eventArchiveId, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
