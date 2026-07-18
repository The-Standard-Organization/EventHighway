// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Brokers.EventHandlers
{
    public partial class EventHandlerBrokerTests
    {
        [Fact]
        public void ShouldNotDuplicateHandlerWhenRegisteringSameIdTwice()
        {
            // given
            Guid sharedId = GetRandomId();
            IEventHandler firstEventHandler = CreateEventHandler(sharedId);
            IEventHandler secondEventHandler = CreateEventHandler(sharedId);

            // when
            this.eventHandlerBroker.Register(firstEventHandler);
            this.eventHandlerBroker.Register(secondEventHandler);

            // then
            IEnumerable<IEventHandler> registeredEventHandlers =
                this.eventHandlerBroker.GetAll();

            registeredEventHandlers.Count(eventHandler => eventHandler.Id == sharedId)
                .Should().Be(1);
        }
    }
}
