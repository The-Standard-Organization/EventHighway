// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Events;
using EventHighway.Portal.Web.Services.Views.Foundations.Events;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.Events
{
    public partial class EventsViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventsViewService eventsViewService;

        public EventsViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventsViewService = new EventsViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static EventV2 CreateRandomEvent(EventStatusV2 status) =>
            new EventV2
            {
                Id = Guid.NewGuid(),
                Status = status
            };

        private static EventV2 CreateRandomEvent(DateTimeOffset createdDate)
        {
            Guid eventAddressId = Guid.NewGuid();

            return new EventV2
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Content = GetRandomString(),
                Type = EventTypeV2.Scheduled,
                Status = EventStatusV2.Active,
                EventAddressV2Id = eventAddressId,

                EventAddressV2 = new EventAddressV2
                {
                    Id = eventAddressId,
                    Name = GetRandomString()
                },

                EventParticipantV2Id = Guid.NewGuid(),
                ScheduledDate = createdDate,
                CreatedDate = createdDate
            };
        }

        private static ListenerEventV2 CreateListenerEvent(
            Guid eventId,
            ListenerEventStatusV2 status) =>
            new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                EventV2Id = eventId,
                Status = status
            };

        private static EventView MapToView(
            EventV2 @event,
            IEnumerable<ListenerEventV2> listenerEvents)
        {
            List<ListenerEventV2> eventListenerEvents = listenerEvents
                .Where(listenerEvent => listenerEvent.EventV2Id == @event.Id)
                .ToList();

            return new EventView
            {
                Id = @event.Id,
                EventName = @event.EventName ?? string.Empty,
                Content = @event.Content ?? string.Empty,
                Type = @event.Type.ToString(),
                Status = @event.Status.ToString(),
                EventAddressV2Id = @event.EventAddressV2Id,
                EventAddressName = @event.EventAddressV2?.Name ?? string.Empty,
                EventParticipantV2Id = @event.EventParticipantV2Id,
                ScheduledDate = @event.ScheduledDate,
                CreatedDate = @event.CreatedDate,
                ListenerEventCount = eventListenerEvents.Count,

                SucceededListenerEventCount = eventListenerEvents.Count(
                    listenerEvent => listenerEvent.Status == ListenerEventStatusV2.Success)
            };
        }
    }
}
