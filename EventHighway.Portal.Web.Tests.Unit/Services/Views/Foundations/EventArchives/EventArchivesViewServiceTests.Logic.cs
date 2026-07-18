// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
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

            EventArchiveV2 oldest = CreateRandomEventArchive(baseDate.AddDays(-2));
            EventArchiveV2 middle = CreateRandomEventArchive(baseDate.AddDays(-1));
            EventArchiveV2 newest = CreateRandomEventArchive(baseDate);

            IReadOnlyList<EventArchiveV2> storageEventArchives =
                new List<EventArchiveV2> { oldest, newest, middle };

            IReadOnlyList<ListenerEventArchiveV2> storageListenerEventArchives =
                new List<ListenerEventArchiveV2>
                {
                    CreateListenerEventArchive(newest.Id, ListenerEventArchiveStatusV2.Success),
                    CreateListenerEventArchive(newest.Id, ListenerEventArchiveStatusV2.Error),
                    CreateListenerEventArchive(middle.Id, ListenerEventArchiveStatusV2.Success)
                };

            List<EventArchiveView> expectedViews = new[] { newest, middle, oldest }
                .Select(eventArchive => MapToView(eventArchive, storageListenerEventArchives))
                .ToList();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                    It.Is<EventArchiveV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(storageEventArchives);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllListenerEventArchiveV2sAsync(
                    It.Is<ListenerEventArchiveV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageListenerEventArchives);

            // when
            List<EventArchiveView> actualViews =
                await this.eventArchivesViewService.RetrieveAllEventArchivesAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(
                expectedViews, options => options.WithStrictOrdering());

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                    It.Is<EventArchiveV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllListenerEventArchiveV2sAsync(
                    It.Is<ListenerEventArchiveV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventArchiveByIdAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventArchiveV2 otherEventArchive = CreateRandomEventArchive(baseDate.AddDays(-1));
            EventArchiveV2 targetEventArchive = CreateRandomEventArchive(baseDate);
            Guid eventArchiveId = targetEventArchive.Id;

            IReadOnlyList<EventArchiveV2> storageEventArchives =
                new List<EventArchiveV2> { otherEventArchive, targetEventArchive };

            IReadOnlyList<ListenerEventArchiveV2> storageListenerEventArchives =
                new List<ListenerEventArchiveV2>
                {
                    CreateListenerEventArchive(
                        targetEventArchive.Id, ListenerEventArchiveStatusV2.Success),

                    CreateListenerEventArchive(
                        otherEventArchive.Id, ListenerEventArchiveStatusV2.Error)
                };

            EventArchiveView expectedView =
                MapToView(targetEventArchive, storageListenerEventArchives);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                    It.Is<EventArchiveV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(storageEventArchives);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllListenerEventArchiveV2sAsync(
                    It.Is<ListenerEventArchiveV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageListenerEventArchives);

            // when
            EventArchiveView actualView =
                await this.eventArchivesViewService.RetrieveEventArchiveByIdAsync(
                    eventArchiveId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                    It.Is<EventArchiveV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllListenerEventArchiveV2sAsync(
                    It.Is<ListenerEventArchiveV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
