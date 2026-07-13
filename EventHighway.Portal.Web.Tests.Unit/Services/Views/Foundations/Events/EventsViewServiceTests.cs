// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
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

        private static EventV2Summary CreateRandomEventSummary(DateTimeOffset createdDate) =>
            new EventV2Summary
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Content = GetRandomString(),
                Type = EventTypeV2.Scheduled,
                Status = EventStatusV2.Active,
                EventAddressV2Id = Guid.NewGuid(),
                EventAddressName = GetRandomString(),
                EventParticipantV2Id = Guid.NewGuid(),
                ScheduledDate = createdDate,
                CreatedDate = createdDate,
                ListenerEventCount = 3,
                SucceededListenerEventCount = 2
            };

        private static EventView MapToView(EventV2Summary eventSummary) =>
            new EventView
            {
                Id = eventSummary.Id,
                EventName = eventSummary.EventName ?? string.Empty,
                Content = eventSummary.Content ?? string.Empty,
                Type = eventSummary.Type.ToString(),
                Status = eventSummary.Status.ToString(),
                EventAddressV2Id = eventSummary.EventAddressV2Id,
                EventAddressName = eventSummary.EventAddressName ?? string.Empty,
                EventParticipantV2Id = eventSummary.EventParticipantV2Id,
                ScheduledDate = eventSummary.ScheduledDate,
                CreatedDate = eventSummary.CreatedDate,
                ListenerEventCount = eventSummary.ListenerEventCount,
                SucceededListenerEventCount = eventSummary.SucceededListenerEventCount
            };
    }
}
