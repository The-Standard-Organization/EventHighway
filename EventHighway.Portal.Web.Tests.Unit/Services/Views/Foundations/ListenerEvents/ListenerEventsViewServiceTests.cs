// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.ListenerEvents;
using EventHighway.Portal.Web.Services.Views.Foundations.ListenerEvents;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.ListenerEvents
{
    public partial class ListenerEventsViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventsViewService listenerEventsViewService;

        public ListenerEventsViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.listenerEventsViewService = new ListenerEventsViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static ListenerEventV2 CreateRandomListenerEvent(DateTimeOffset createdDate) =>
            new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                Status = ListenerEventStatusV2.Success,
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                RemainingRetryAttempts = 2,
                RetryAttemptsAllowed = 5,
                NextRetryAttemptNotBefore = createdDate.AddMinutes(30),
                DispatchedDate = createdDate,
                EventV2Id = Guid.NewGuid(),
                EventAddressV2Id = Guid.NewGuid(),
                EventListenerV2Id = Guid.NewGuid(),
                EventParticipantV2Id = Guid.NewGuid(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate
            };

        private static ListenerEventView MapToView(ListenerEventV2 listenerEvent) =>
            new ListenerEventView
            {
                Id = listenerEvent.Id,
                Status = listenerEvent.Status.ToString(),
                ResponseCode = listenerEvent.ResponseCode,
                ResponseMessage = listenerEvent.ResponseMessage,
                RemainingRetryAttempts = listenerEvent.RemainingRetryAttempts,
                RetryAttemptsAllowed = listenerEvent.RetryAttemptsAllowed,
                NextRetryAttemptNotBefore = listenerEvent.NextRetryAttemptNotBefore,
                DispatchedDate = listenerEvent.DispatchedDate,
                EventV2Id = listenerEvent.EventV2Id,
                EventAddressV2Id = listenerEvent.EventAddressV2Id,
                EventListenerV2Id = listenerEvent.EventListenerV2Id,
                EventParticipantV2Id = listenerEvent.EventParticipantV2Id,
                CreatedDate = listenerEvent.CreatedDate
            };
    }
}
