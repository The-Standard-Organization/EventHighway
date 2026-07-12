// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Models.Views.EventAddresses;
using EventHighway.Portal.Web.Models.Views.EventHandlers;
using EventHighway.Portal.Web.Models.Views.EventListeners;
using EventHighway.Portal.Web.Models.Views.EventParticipants;
using EventHighway.Portal.Web.Services.Views.EventAddresses;
using EventHighway.Portal.Web.Services.Views.EventHandlers;
using EventHighway.Portal.Web.Services.Views.EventListeners;
using EventHighway.Portal.Web.Services.Views.EventParticipants;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.Admin
{
    public partial class EventAddressDetailPageComponentTests : BunitContext
    {
        private readonly Mock<IEventAddressesViewService> addressesViewServiceMock;
        private readonly Mock<IEventListenersViewService> listenersViewServiceMock;
        private readonly Mock<IEventParticipantsViewService> participantsViewServiceMock;
        private readonly Mock<IEventHandlersViewService> eventHandlersViewServiceMock;

        public EventAddressDetailPageComponentTests()
        {
            this.addressesViewServiceMock = new Mock<IEventAddressesViewService>();
            this.listenersViewServiceMock = new Mock<IEventListenersViewService>();
            this.participantsViewServiceMock = new Mock<IEventParticipantsViewService>();
            this.eventHandlersViewServiceMock = new Mock<IEventHandlersViewService>();

            Services.AddSingleton(this.addressesViewServiceMock.Object);
            Services.AddSingleton(this.listenersViewServiceMock.Object);
            Services.AddSingleton(this.participantsViewServiceMock.Object);
            Services.AddSingleton(this.eventHandlersViewServiceMock.Object);

            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static List<EventHandlerView> CreateRandomEventHandlers(int count) =>
            Enumerable.Range(0, count).Select(_ => new EventHandlerView
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString()
            }).ToList();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventAddressView CreateRandomAddress() =>
            new EventAddressView
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString()
            };

        private static List<EventParticipantView> CreateRandomParticipants(int count) =>
            Enumerable.Range(0, count).Select(_ => new EventParticipantView
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString()
            }).ToList();

        private static List<EventListenerView> CreateRandomListeners(Guid addressId, int count) =>
            Enumerable.Range(0, count).Select(_ => new EventListenerView
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                HandlerName = GetRandomString(),
                HandlerId = Guid.NewGuid(),
                EventAddressV2Id = addressId
            }).ToList();
    }
}
