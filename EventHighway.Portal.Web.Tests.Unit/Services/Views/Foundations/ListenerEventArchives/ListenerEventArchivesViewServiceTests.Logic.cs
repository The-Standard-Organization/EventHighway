// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEventArchives;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.ListenerEventArchives
{
    public partial class ListenerEventArchivesViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventArchivesMostRecentFirstAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            ListenerEventArchiveV2 oldest =
                CreateRandomListenerEventArchive(baseDate.AddDays(-2));

            ListenerEventArchiveV2 middle =
                CreateRandomListenerEventArchive(baseDate.AddDays(-1));

            ListenerEventArchiveV2 newest =
                CreateRandomListenerEventArchive(baseDate);

            IQueryable<ListenerEventArchiveV2> storageListenerEventArchives =
                new[] { oldest, newest, middle }.AsQueryable();

            List<ListenerEventArchiveView> expectedViews =
                new[] { newest, middle, oldest }.Select(MapToView).ToList();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageListenerEventArchives);

            // when
            List<ListenerEventArchiveView> actualViews =
                await this.listenerEventArchivesViewService
                    .RetrieveAllListenerEventArchivesAsync(
                        TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(
                expectedViews, options => options.WithStrictOrdering());

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveListenerEventArchivesByEventArchiveIdMostRecentFirstAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();
            Guid eventArchiveId = Guid.NewGuid();

            ListenerEventArchiveV2 oldest =
                CreateRandomListenerEventArchive(baseDate.AddDays(-2));
            oldest.EventArchiveV2Id = eventArchiveId;

            ListenerEventArchiveV2 newest =
                CreateRandomListenerEventArchive(baseDate);
            newest.EventArchiveV2Id = eventArchiveId;

            ListenerEventArchiveV2 otherArchive =
                CreateRandomListenerEventArchive(baseDate.AddDays(-1));

            IQueryable<ListenerEventArchiveV2> storageListenerEventArchives =
                new[] { oldest, otherArchive, newest }.AsQueryable();

            List<ListenerEventArchiveView> expectedViews =
                new[] { newest, oldest }.Select(MapToView).ToList();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageListenerEventArchives);

            // when
            List<ListenerEventArchiveView> actualViews =
                await this.listenerEventArchivesViewService
                    .RetrieveListenerEventArchivesByEventArchiveIdAsync(
                        eventArchiveId, TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(
                expectedViews, options => options.WithStrictOrdering());

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveListenerEventArchiveByIdAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            ListenerEventArchiveV2 targetListenerEventArchive =
                CreateRandomListenerEventArchive(baseDate);

            Guid listenerEventArchiveId = targetListenerEventArchive.Id;

            IQueryable<ListenerEventArchiveV2> storageListenerEventArchives = new[]
            {
                CreateRandomListenerEventArchive(baseDate.AddDays(-1)),
                targetListenerEventArchive,
                CreateRandomListenerEventArchive(baseDate.AddDays(-2))
            }.AsQueryable();

            ListenerEventArchiveView expectedView = MapToView(targetListenerEventArchive);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageListenerEventArchives);

            // when
            ListenerEventArchiveView actualView =
                await this.listenerEventArchivesViewService
                    .RetrieveListenerEventArchiveByIdAsync(
                        listenerEventArchiveId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
