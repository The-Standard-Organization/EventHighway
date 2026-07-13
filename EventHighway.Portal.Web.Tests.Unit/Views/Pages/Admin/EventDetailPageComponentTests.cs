// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Bunit;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Events;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents;
using EventHighway.Portal.Web.Services.Views.Foundations.Events;
using EventHighway.Portal.Web.Services.Views.Foundations.ListenerEvents;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Pages.Admin
{
    public partial class EventDetailPageComponentTests : BunitContext
    {
        private readonly Mock<IEventsViewService> eventsViewServiceMock;
        private readonly Mock<IListenerEventsViewService> listenerEventsViewServiceMock;

        public EventDetailPageComponentTests()
        {
            this.eventsViewServiceMock = new Mock<IEventsViewService>();
            this.listenerEventsViewServiceMock = new Mock<IListenerEventsViewService>();

            Services.AddSingleton(this.eventsViewServiceMock.Object);
            Services.AddSingleton(this.listenerEventsViewServiceMock.Object);

            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventView CreateRandomEvent(string content) =>
            new EventView
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Content = content,
                Type = "Immediate",
                Status = "Active",
                EventAddressV2Id = Guid.NewGuid(),
                EventAddressName = GetRandomString(),
                CreatedDate = DateTimeOffset.UtcNow
            };

        private static ListenerEventView CreateListenerEvent(Guid eventId, string status) =>
            new ListenerEventView
            {
                Id = Guid.NewGuid(),
                Status = status,
                ResponseCode = status == "Success" ? "200" : "503",
                ResponseMessage = status,
                EventV2Id = eventId,
                EventAddressV2Id = Guid.NewGuid(),
                EventListenerV2Id = Guid.NewGuid(),
                ListenerName = GetRandomString(),
                CreatedDate = DateTimeOffset.UtcNow
            };
    }
}
