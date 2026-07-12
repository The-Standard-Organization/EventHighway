// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AngleSharp.Dom;
using Bunit;
using EventHighway.Portal.Web.Views.Pages.Admin;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class EventAddressDetailPageComponentTests
    {
        [Fact]
        public void ShouldShowRegisterListenerModalWithHandlerDropdown()
        {
            // given
            EventAddressView address = CreateRandomAddress();
            List<EventHandlerView> eventHandlers = CreateRandomEventHandlers(count: 3);

            SetupPage(
                address,
                CreateRandomListeners(address.Id, count: 0),
                CreateRandomParticipants(count: 2),
                eventHandlers);

            IRenderedComponent<EventAddressDetailPage> renderedPage =
                Render<EventAddressDetailPage>(parameters =>
                    parameters.Add(page => page.AddressId, address.Id));

            // when
            renderedPage.Find("button.btn-primary.btn-sm").Click();

            // then
            IElement handlerSelect = renderedPage.FindAll("select.form-select")[0];

            foreach (EventHandlerView eventHandler in eventHandlers)
            {
                handlerSelect.QuerySelectorAll("option")
                    .Should().Contain(option =>
                        option.GetAttribute("value") == eventHandler.Id.ToString()
                            && option.TextContent == eventHandler.Name);
            }

            renderedPage.Markup.Should().NotContain("Handler Id");
            renderedPage.Markup.Should().NotContain("Handler Name");
        }

        [Fact]
        public void ShouldShowErrorWhenNoHandlerIsSelectedOnRegisterListener()
        {
            // given
            EventAddressView address = CreateRandomAddress();
            List<EventHandlerView> eventHandlers = CreateRandomEventHandlers(count: 3);

            SetupPage(
                address,
                CreateRandomListeners(address.Id, count: 0),
                CreateRandomParticipants(count: 2),
                eventHandlers);

            IRenderedComponent<EventAddressDetailPage> renderedPage =
                Render<EventAddressDetailPage>(parameters =>
                    parameters.Add(page => page.AddressId, address.Id));

            renderedPage.Find("button.btn-primary.btn-sm").Click();

            // when
            renderedPage.FindAll("button")
                .First(button => button.TextContent.Trim() == "Save")
                .Click();

            // then
            renderedPage.Markup.Should().Contain("Select a handler.");

            this.listenersViewServiceMock.Verify(service =>
                service.RegisterListenerAsync(
                    It.IsAny<EventListenerView>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }
    }
}
