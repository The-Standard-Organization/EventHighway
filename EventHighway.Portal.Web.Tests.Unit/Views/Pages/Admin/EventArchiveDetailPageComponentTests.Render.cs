// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using EventHighway.Portal.Web.Views.Pages.Admin;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEventArchives;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class EventArchiveDetailPageComponentTests
    {
        private void SetupPage(
            EventArchiveView archive,
            List<ListenerEventArchiveView> listenerArchives)
        {
            this.eventArchivesViewServiceMock.Setup(service =>
                service.RetrieveEventArchiveByIdAsync(
                    archive.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(archive);

            this.listenerEventArchivesViewServiceMock.Setup(service =>
                service.RetrieveListenerEventArchivesByEventArchiveIdAsync(
                    archive.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(listenerArchives);
        }

        [Fact]
        public void ShouldRenderListenerStatusBadgesAndRowColors()
        {
            // given
            EventArchiveView archive = CreateRandomArchive();

            var listenerArchives = new List<ListenerEventArchiveView>
            {
                CreateListenerArchive(archive.Id, "Success"),
                CreateListenerArchive(archive.Id, "Error")
            };

            SetupPage(archive, listenerArchives);

            // when
            IRenderedComponent<EventArchiveDetailPage> renderedPage =
                Render<EventArchiveDetailPage>(parameters =>
                    parameters.Add(page => page.EventArchiveId, archive.Id));

            // then
            renderedPage.FindAll("span.badge.text-bg-success").Should().NotBeEmpty();
            renderedPage.FindAll("span.badge.text-bg-danger").Should().NotBeEmpty();
            renderedPage.FindAll("tr.table-success").Should().NotBeEmpty();
            renderedPage.FindAll("tr.table-danger").Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldConfirmAndReplayListenerEventArchive()
        {
            // given
            EventArchiveView archive = CreateRandomArchive();

            ListenerEventArchiveView row =
                CreateListenerArchive(archive.Id, "Error");

            SetupPage(archive, new List<ListenerEventArchiveView> { row });

            this.replayViewServiceMock.Setup(service =>
                service.ReplayListenerEventArchiveAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .Returns(ValueTask.CompletedTask);

            IRenderedComponent<EventArchiveDetailPage> renderedPage =
                Render<EventArchiveDetailPage>(parameters =>
                    parameters.Add(page => page.EventArchiveId, archive.Id));

            // when — click the row's Replay button, then confirm
            renderedPage.FindAll("button")
                .First(button => button.TextContent.Trim() == "Replay")
                .Click();

            renderedPage.Markup.Should().Contain("loop detection");

            renderedPage.FindAll("button")
                .First(button => button.TextContent.Trim() == "OK")
                .Click();

            // then
            this.replayViewServiceMock.Verify(service =>
                service.ReplayListenerEventArchiveAsync(
                    row.EventArchiveV2Id,
                    row.EventAddressV2Id,
                    row.EventListenerV2Id,
                    It.IsAny<CancellationToken>()),
                        Times.Once);
        }
    }
}
