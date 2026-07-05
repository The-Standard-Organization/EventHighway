// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.Events;
using EventHighway.Portal.Web.Services.Views.Events;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Events
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

        private static EventV2 CreateRandomEvent(
            EventStatusV2 status,
            int remainingRetryAttempts) =>
            new EventV2
            {
                Id = Guid.NewGuid(),
                Status = status
            };

        private static EventV2 CreateRandomEvent(DateTimeOffset createdDate) =>
            new EventV2
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Content = GetRandomString(),
                Type = EventTypeV2.Scheduled,
                Status = EventStatusV2.Active,
                EventAddressV2Id = Guid.NewGuid(),
                EventParticipantV2Id = Guid.NewGuid(),
                ScheduledDate = createdDate,
                CreatedDate = createdDate,
                UpdatedDate = createdDate
            };

        private static EventView MapToView(EventV2 @event) =>
            new EventView
            {
                Id = @event.Id,
                EventName = @event.EventName,
                Content = @event.Content,
                Type = @event.Type.ToString(),
                Status = @event.Status.ToString(),
                RemainingRetryAttempts = 0,
                EventAddressV2Id = @event.EventAddressV2Id,
                EventParticipantV2Id = @event.EventParticipantV2Id,
                ScheduledDate = @event.ScheduledDate,
                CreatedDate = @event.CreatedDate
            };
    }
}
