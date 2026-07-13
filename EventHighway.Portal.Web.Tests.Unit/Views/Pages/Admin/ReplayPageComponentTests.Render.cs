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
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Replays;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class ReplayPageComponentTests
    {
        [Fact]
        public void ShouldRenderAddressDropdownWithAllOptionAndAddresses()
        {
            // given
            List<EventAddressView> addresses = CreateRandomAddresses(count: 2);

            this.addressesViewServiceMock.Setup(service =>
                service.RetrieveAllAddressesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(addresses);

            // when
            IRenderedComponent<ReplayPage> renderedPage = Render<ReplayPage>();

            // then
            string markup = renderedPage.Markup;
            markup.Should().Contain("All");
            addresses.ForEach(address => markup.Should().Contain(address.Name));
        }

        [Fact]
        public void ShouldLoadListenersWhenAddressSelected()
        {
            // given
            List<EventAddressView> addresses = CreateRandomAddresses(count: 2);
            EventAddressView chosen = addresses[0];
            List<EventListenerView> listeners = CreateRandomListeners(chosen.Id, count: 2);

            this.addressesViewServiceMock.Setup(service =>
                service.RetrieveAllAddressesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(addresses);

            this.listenersViewServiceMock.Setup(service =>
                service.RetrieveListenersByAddressAsync(
                    chosen.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(listeners);

            IRenderedComponent<ReplayPage> renderedPage = Render<ReplayPage>();

            // when
            renderedPage.FindAll("select")[0].Change(chosen.Id.ToString());

            // then
            this.listenersViewServiceMock.Verify(service =>
                service.RetrieveListenersByAddressAsync(
                    chosen.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            renderedPage.Markup.Should().Contain("Add Listener");
            renderedPage.Markup.Should().Contain(listeners[0].Name);
        }

        [Fact]
        public void ShouldReplayWithSelectedAddressAndListener()
        {
            // given
            List<EventAddressView> addresses = CreateRandomAddresses(count: 1);
            EventAddressView chosen = addresses[0];
            List<EventListenerView> listeners = CreateRandomListeners(chosen.Id, count: 1);

            this.addressesViewServiceMock.Setup(service =>
                service.RetrieveAllAddressesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(addresses);

            this.listenersViewServiceMock.Setup(service =>
                service.RetrieveListenersByAddressAsync(
                    chosen.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(listeners);

            ReplayRequestView? captured = null;

            this.replayViewServiceMock.Setup(service =>
                service.ReplayAsync(
                    It.IsAny<ReplayRequestView>(), It.IsAny<CancellationToken>()))
                        .Callback<ReplayRequestView, CancellationToken>(
                            (request, _) => captured = request)
                        .Returns(ValueTask.CompletedTask);

            IRenderedComponent<ReplayPage> renderedPage = Render<ReplayPage>();

            renderedPage.FindAll("select")[0].Change(chosen.Id.ToString());

            // add the (pre-selected) listener into the selection
            renderedPage.FindAll("button")
                .First(button => button.TextContent.Trim() == "Add")
                .Click();

            // when
            renderedPage.FindAll("button")
                .First(button => button.TextContent.Trim() == "Replay")
                .Click();

            // then
            this.replayViewServiceMock.Verify(service =>
                service.ReplayAsync(
                    It.IsAny<ReplayRequestView>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            captured.Should().NotBeNull();
            captured!.EventAddressV2Id.Should().Be(chosen.Id);
            captured.EventListenerV2Ids.Should().Contain(listeners[0].Id);
        }
    }
}
