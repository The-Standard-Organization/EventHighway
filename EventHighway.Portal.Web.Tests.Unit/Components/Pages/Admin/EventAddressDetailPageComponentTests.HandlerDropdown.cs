// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using Bunit;
using EventHighway.Portal.Web.Components.Pages.Admin;
using EventHighway.Portal.Web.Models.Views.EventAddresses;
using EventHighway.Portal.Web.Models.Views.EventHandlers;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.Admin
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
    }
}
