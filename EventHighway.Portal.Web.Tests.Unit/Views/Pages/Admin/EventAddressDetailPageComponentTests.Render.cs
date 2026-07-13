// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Views.Pages.Admin;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventAddresses;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class EventAddressDetailPageComponentTests
    {
        private void SetupPage(
            EventAddressView address,
            List<EventListenerView> listeners,
            List<EventParticipantView> participants,
            List<EventHandlerView>? eventHandlers = null)
        {
            this.addressesViewServiceMock.Setup(service =>
                service.RetrieveAllAddressesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<EventAddressView> { address });

            this.participantsViewServiceMock.Setup(service =>
                service.RetrieveAllParticipantsAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(participants);

            this.eventHandlersViewServiceMock.Setup(service =>
                service.RetrieveAllEventHandlersAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventHandlers ?? new List<EventHandlerView>());

            this.listenersViewServiceMock.Setup(service =>
                service.RetrieveListenersByAddressAsync(
                    address.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(listeners);
        }

        [Fact]
        public void ShouldRenderListenersWithIdAsFirstColumn()
        {
            // given
            EventAddressView address = CreateRandomAddress();
            List<EventListenerView> listeners = CreateRandomListeners(address.Id, count: 1);
            SetupPage(address, listeners, CreateRandomParticipants(count: 2));

            // when
            IRenderedComponent<EventAddressDetailPage> renderedPage =
                Render<EventAddressDetailPage>(parameters =>
                    parameters.Add(page => page.AddressId, address.Id));

            // then
            renderedPage.FindAll("thead th")[0].TextContent.Trim().Should().Be("Id");
            renderedPage.Markup.Should().Contain(listeners[0].Id.ToString());
            renderedPage.Markup.Should().Contain(listeners[0].Name);
        }

        [Fact]
        public void ShouldShowRegisterListenerModalWithHandlerAndNewFields()
        {
            // given
            EventAddressView address = CreateRandomAddress();
            List<EventParticipantView> participants = CreateRandomParticipants(count: 2);
            SetupPage(address, CreateRandomListeners(address.Id, count: 0), participants);

            IRenderedComponent<EventAddressDetailPage> renderedPage =
                Render<EventAddressDetailPage>(parameters =>
                    parameters.Add(page => page.AddressId, address.Id));

            // when
            renderedPage.Find("button.btn-primary.btn-sm").Click();

            // then
            string markup = renderedPage.Markup;

            markup.Should().Contain("Handler");
            markup.Should().Contain("Participant");
            markup.Should().Contain("Promoted Properties");
            markup.Should().Contain("Filter Criteria");

            foreach (EventParticipantView participant in participants)
            {
                markup.Should().Contain(participant.Name);
            }
        }

        [Fact]
        public void ShouldRegisterListenerWithSelectedHandlerAndParticipantAndNewFields()
        {
            // given
            EventAddressView address = CreateRandomAddress();
            List<EventParticipantView> participants = CreateRandomParticipants(count: 2);
            List<EventHandlerView> eventHandlers = CreateRandomEventHandlers(count: 3);
            EventParticipantView chosenParticipant = participants[1];
            EventHandlerView chosenEventHandler = eventHandlers[1];
            string promotedProperties = GetRandomString();
            string filterCriteria = GetRandomString();

            SetupPage(
                address,
                CreateRandomListeners(address.Id, count: 0),
                participants,
                eventHandlers);

            EventListenerView? captured = null;

            this.listenersViewServiceMock.Setup(service =>
                service.RegisterListenerAsync(
                    It.IsAny<EventListenerView>(), It.IsAny<CancellationToken>()))
                        .Callback<EventListenerView, CancellationToken>(
                            (listener, _) => captured = listener)
                        .ReturnsAsync(new EventListenerView());

            IRenderedComponent<EventAddressDetailPage> renderedPage =
                Render<EventAddressDetailPage>(parameters =>
                    parameters.Add(page => page.AddressId, address.Id));

            renderedPage.Find("button.btn-primary.btn-sm").Click();

            // Modal FormText inputs, in render order:
            // 0 Name, 1 Description, 2 Promoted Properties, 3 Filter Criteria
            var inputs = renderedPage.FindAll("input.form-control");
            inputs[0].Input(GetRandomString());
            inputs[2].Input(promotedProperties);
            inputs[3].Input(filterCriteria);

            // Modal selects, in render order: 0 Handler, 1 Participant
            var selects = renderedPage.FindAll("select.form-select");
            selects[0].Change(chosenEventHandler.Id.ToString());
            selects[1].Change(chosenParticipant.Id.ToString());

            // when
            renderedPage.FindAll("button")
                .First(button => button.TextContent.Trim() == "Save")
                .Click();

            // then
            this.listenersViewServiceMock.Verify(service =>
                service.RegisterListenerAsync(
                    It.IsAny<EventListenerView>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            captured.Should().NotBeNull();
            captured!.HandlerId.Should().Be(chosenEventHandler.Id);
            captured.HandlerName.Should().Be(chosenEventHandler.Name);
            captured.EventParticipantV2Id.Should().Be(chosenParticipant.Id);
            captured.PromotedProperties.Should().Be(promotedProperties);
            captured.FilterCriteria.Should().Be(filterCriteria);
            captured.EventAddressV2Id.Should().Be(address.Id);
        }
    }
}
