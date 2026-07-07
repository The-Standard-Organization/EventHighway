// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Components.Pages.Admin;
using EventHighway.Portal.Web.Models.Views.EventArchives;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.Admin
{
    public partial class EventArchivesPageComponentTests
    {
        [Fact]
        public void ShouldRenderStatusBadgesAndRowColors()
        {
            // given
            EventArchiveView quarantined = CreateArchive(
                "Quarantined", listenerEventCount: 0, succeededListenerEventCount: 0);

            EventArchiveView partialSuccess = CreateArchive(
                "Active", listenerEventCount: 3, succeededListenerEventCount: 2);

            this.eventArchivesViewServiceMock.Setup(service =>
                service.RetrieveAllEventArchivesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<EventArchiveView> { quarantined, partialSuccess });

            // when
            IRenderedComponent<EventArchivesPage> renderedPage = Render<EventArchivesPage>();

            // then
            renderedPage.Markup.Should().Contain(quarantined.EventAddressName);
            renderedPage.Markup.Should().Contain("Partial Success");
            renderedPage.Markup.Should().Contain("2/3");
            renderedPage.FindAll("tr.table-danger").Should().NotBeEmpty();
            renderedPage.FindAll("tr.table-warning").Should().NotBeEmpty();
            renderedPage.FindAll("span.badge.text-bg-danger").Should().NotBeEmpty();
            renderedPage.FindAll("span.badge.text-bg-warning").Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldFilterArchivesByStatus()
        {
            // given
            EventArchiveView quarantined = CreateArchive(
                "Quarantined", listenerEventCount: 0, succeededListenerEventCount: 0);

            EventArchiveView success = CreateArchive(
                "Active", listenerEventCount: 3, succeededListenerEventCount: 3);

            this.eventArchivesViewServiceMock.Setup(service =>
                service.RetrieveAllEventArchivesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<EventArchiveView> { quarantined, success });

            IRenderedComponent<EventArchivesPage> renderedPage = Render<EventArchivesPage>();

            // when — Type select is [0], Status select is [1]
            renderedPage.FindAll("select")[1].Change("Quarantined");

            // then
            renderedPage.Markup.Should().Contain(quarantined.EventAddressName);
            renderedPage.Markup.Should().NotContain(success.EventAddressName);
        }
    }
}
