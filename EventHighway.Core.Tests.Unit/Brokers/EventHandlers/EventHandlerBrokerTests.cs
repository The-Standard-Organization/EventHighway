// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using Moq;

namespace EventHighway.Core.Tests.Unit.Brokers.EventHandlers
{
    public partial class EventHandlerBrokerTests
    {
        private readonly IEventHandlerBroker eventHandlerBroker;

        public EventHandlerBrokerTests() =>
            this.eventHandlerBroker = new EventHandlerBroker();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            Guid.NewGuid().ToString();

        private static IEventHandler CreateEventHandler(Guid id)
        {
            var eventHandlerMock = new Mock<IEventHandler>();

            eventHandlerMock.SetupGet(eventHandler => eventHandler.Id)
                .Returns(id);

            eventHandlerMock.SetupGet(eventHandler => eventHandler.Name)
                .Returns(GetRandomString());

            return eventHandlerMock.Object;
        }
    }
}
