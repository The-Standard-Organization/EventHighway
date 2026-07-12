// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Models.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventArchives;
using EventHighway.Portal.Web.Services.Views.Foundations.EventArchives;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventArchives
{
    public partial class EventArchivesViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventArchivesViewService eventArchivesViewService;

        public EventArchivesViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventArchivesViewService = new EventArchivesViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventArchiveV2Summary CreateRandomEventArchiveSummary(
            DateTimeOffset archivedDate) =>
            new EventArchiveV2Summary
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Content = GetRandomString(),
                Type = EventArchiveTypeV2.Scheduled,
                Status = EventArchiveStatusV2.Active,
                EventAddressV2Id = Guid.NewGuid(),
                EventAddressName = GetRandomString(),
                EventParticipantV2Id = Guid.NewGuid(),
                ScheduledDate = archivedDate,
                CreatedDate = archivedDate,
                ArchivedDate = archivedDate,
                ListenerEventCount = 3,
                SucceededListenerEventCount = 2
            };

        private static EventArchiveView MapToView(EventArchiveV2Summary eventArchiveSummary) =>
            new EventArchiveView
            {
                Id = eventArchiveSummary.Id,
                EventName = eventArchiveSummary.EventName ?? string.Empty,
                Content = eventArchiveSummary.Content ?? string.Empty,
                Type = eventArchiveSummary.Type.ToString(),
                Status = eventArchiveSummary.Status.ToString(),
                EventAddressV2Id = eventArchiveSummary.EventAddressV2Id,
                EventAddressName = eventArchiveSummary.EventAddressName ?? string.Empty,
                EventParticipantV2Id = eventArchiveSummary.EventParticipantV2Id,
                ScheduledDate = eventArchiveSummary.ScheduledDate,
                CreatedDate = eventArchiveSummary.CreatedDate,
                ArchivedDate = eventArchiveSummary.ArchivedDate,
                ListenerEventCount = eventArchiveSummary.ListenerEventCount,
                SucceededListenerEventCount = eventArchiveSummary.SucceededListenerEventCount
            };
    }
}
