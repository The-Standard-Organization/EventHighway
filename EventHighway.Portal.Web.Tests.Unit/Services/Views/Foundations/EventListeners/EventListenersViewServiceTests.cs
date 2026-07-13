// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners;
using EventHighway.Portal.Web.Services.Views.Foundations.EventListeners;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventListeners
{
    public partial class EventListenersViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventListenersViewService eventListenersViewService;

        public EventListenersViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventListenersViewService = new EventListenersViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static List<EventListenerV2> CreateRandomListeners(Guid eventAddressId, int count) =>
            Enumerable.Range(0, count).Select(_ => new EventListenerV2
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                HandlerName = GetRandomString(),
                HandlerId = Guid.NewGuid(),
                EventAddressV2Id = eventAddressId,
                EventParticipantV2Id = Guid.NewGuid(),
                PromotedProperties = GetRandomString(),
                FilterCriteria = GetRandomString()
            }).ToList();

        private static List<EventListenerView> MapToViews(IEnumerable<EventListenerV2> listeners) =>
            listeners.Select(listener => new EventListenerView
            {
                Id = listener.Id,
                Name = listener.Name,
                Description = listener.Description,
                HandlerName = listener.HandlerName,
                HandlerId = listener.HandlerId,
                EventAddressV2Id = listener.EventAddressV2Id,
                EventParticipantV2Id = listener.EventParticipantV2Id,
                PromotedProperties = listener.PromotedProperties ?? string.Empty,
                FilterCriteria = listener.FilterCriteria ?? string.Empty
            }).ToList();
    }
}
