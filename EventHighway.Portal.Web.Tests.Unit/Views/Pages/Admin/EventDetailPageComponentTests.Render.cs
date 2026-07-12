// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Views.Pages.Admin;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Events;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class EventDetailPageComponentTests
    {
        private void SetupPage(EventView eventView, List<ListenerEventView> listenerEvents)
        {
            this.eventsViewServiceMock.Setup(service =>
                service.RetrieveEventByIdAsync(
                    eventView.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(eventView);

            this.listenerEventsViewServiceMock.Setup(service =>
                service.RetrieveListenerEventsByEventIdAsync(
                    eventView.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(listenerEvents);
        }

        [Fact]
        public void ShouldRenderListenerNameAndStatusBadgesAndRowColors()
        {
            // given
            EventView eventView = CreateRandomEvent(content: "{}");

            var listenerEvents = new List<ListenerEventView>
            {
                CreateListenerEvent(eventView.Id, "Success"),
                CreateListenerEvent(eventView.Id, "Error")
            };

            SetupPage(eventView, listenerEvents);

            // when
            IRenderedComponent<EventDetailPage> renderedPage =
                Render<EventDetailPage>(parameters =>
                    parameters.Add(page => page.EventId, eventView.Id));

            // then
            renderedPage.Markup.Should().Contain(listenerEvents[0].ListenerName!);
            renderedPage.FindAll("span.badge.text-bg-success").Should().NotBeEmpty();
            renderedPage.FindAll("span.badge.text-bg-danger").Should().NotBeEmpty();
            renderedPage.FindAll("tr.table-success").Should().NotBeEmpty();
            renderedPage.FindAll("tr.table-danger").Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldShowFormattedJsonContentInModal()
        {
            // given
            EventView eventView = CreateRandomEvent(content: "{\"Title\":\"AddNewRelease\"}");
            SetupPage(eventView, new List<ListenerEventView>());

            IRenderedComponent<EventDetailPage> renderedPage =
                Render<EventDetailPage>(parameters =>
                    parameters.Add(page => page.EventId, eventView.Id));

            // when
            renderedPage.FindAll("button")
                .First(button => button.TextContent.Trim() == "View Content")
                .Click();

            // then
            string modalBody = renderedPage.Find("div.modal-body").TextContent;
            modalBody.Should().Contain("Title");
            modalBody.Should().Contain("AddNewRelease");
        }

        [Fact]
        public void ShouldFilterListenerEventsByStatus()
        {
            // given
            EventView eventView = CreateRandomEvent(content: "{}");

            ListenerEventView successEvent = CreateListenerEvent(eventView.Id, "Success");
            ListenerEventView errorEvent = CreateListenerEvent(eventView.Id, "Error");

            SetupPage(eventView, new List<ListenerEventView> { successEvent, errorEvent });

            IRenderedComponent<EventDetailPage> renderedPage =
                Render<EventDetailPage>(parameters =>
                    parameters.Add(page => page.EventId, eventView.Id));

            // when — filter to Error only
            renderedPage.Find("select.form-select").Change("Error");

            // then
            renderedPage.Markup.Should().Contain(errorEvent.ResponseCode);
            renderedPage.Markup.Should().NotContain(successEvent.ListenerName!);
        }
    }
}
