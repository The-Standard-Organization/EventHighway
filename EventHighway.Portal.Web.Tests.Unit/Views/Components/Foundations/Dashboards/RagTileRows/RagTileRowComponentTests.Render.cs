// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Views.Bases;
using EventHighway.Portal.Web.Views.Components.Foundations.Dashboards;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Components.Foundations.Dashboards
{
    public partial class RagTileRowComponentTests
    {
        [Fact]
        public void ShouldInitializeComponentInLoadingState()
        {
            // given
            var pendingSource = new TaskCompletionSource<List<HealthRagTile>>();

            this.healthViewServiceMock.Setup(service =>
                service.RetrieveHealthRagTilesAsync(
                    It.IsAny<TrafficPeriodV2>(),
                    It.IsAny<System.DateTimeOffset>(),
                    It.IsAny<System.DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<List<HealthRagTile>>(pendingSource.Task));

            // when
            IRenderedComponent<RagTileRow> renderedRagTileRow = Render<RagTileRow>();

            // then
            renderedRagTileRow.Instance.State.Should().Be(RagTileRowState.Loading);
            renderedRagTileRow.FindAll("div.spinner-border").Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldRenderTilesWhenLoaded()
        {
            // given
            List<HealthRagTile> randomTiles = CreateRandomRagTiles();

            this.healthViewServiceMock.Setup(service =>
                service.RetrieveHealthRagTilesAsync(
                    It.IsAny<TrafficPeriodV2>(),
                    It.IsAny<System.DateTimeOffset>(),
                    It.IsAny<System.DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(randomTiles);

            // when
            IRenderedComponent<RagTileRow> renderedRagTileRow = Render<RagTileRow>();

            // then
            renderedRagTileRow.Instance.State.Should().Be(RagTileRowState.Content);
            renderedRagTileRow.FindAll("div.stat-tile").Should().HaveCount(randomTiles.Count);

            this.healthViewServiceMock.Verify(service =>
                service.RetrieveHealthRagTilesAsync(
                    It.IsAny<TrafficPeriodV2>(),
                    It.IsAny<System.DateTimeOffset>(),
                    It.IsAny<System.DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void ShouldRenderErrorWhenViewServiceThrowsDependencyException()
        {
            // given
            var dependencyException =
                new HealthViewDependencyException(
                    innerException: new Xeption(message: GetRandomString()));

            this.healthViewServiceMock.Setup(service =>
                service.RetrieveHealthRagTilesAsync(
                    It.IsAny<TrafficPeriodV2>(),
                    It.IsAny<System.DateTimeOffset>(),
                    It.IsAny<System.DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(dependencyException);

            // when
            IRenderedComponent<RagTileRow> renderedRagTileRow = Render<RagTileRow>();

            // then
            renderedRagTileRow.Instance.State.Should().Be(RagTileRowState.Error);
            renderedRagTileRow.FindAll("div.alert-danger").Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldGroupTilesByGroupingWithHeadings()
        {
            // given
            var tiles = new List<HealthRagTile>
            {
                new HealthRagTile
                {
                    Grouping = "Event Addresses", Label = "Total",
                    Value = "3", Variant = StatTileVariant.Na
                },
                new HealthRagTile
                {
                    Grouping = "Active Events", Label = "Total",
                    Value = "7", Variant = StatTileVariant.Na
                },
                new HealthRagTile
                {
                    Grouping = "Active Events", Label = "Immediate",
                    Value = "1", Variant = StatTileVariant.Na
                },
            };

            this.healthViewServiceMock.Setup(service =>
                service.RetrieveHealthRagTilesAsync(
                    It.IsAny<TrafficPeriodV2>(),
                    It.IsAny<System.DateTimeOffset>(),
                    It.IsAny<System.DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(tiles);

            // when
            IRenderedComponent<RagTileRow> renderedRagTileRow = Render<RagTileRow>();

            // then
            var headings = renderedRagTileRow.FindAll("h6.rag-group-title");

            headings.Should().HaveCount(2);

            System.Linq.Enumerable
                .Select(headings, heading => heading.TextContent.Trim())
                .Should().ContainInOrder("Event Addresses", "Active Events");
        }
    }
}
