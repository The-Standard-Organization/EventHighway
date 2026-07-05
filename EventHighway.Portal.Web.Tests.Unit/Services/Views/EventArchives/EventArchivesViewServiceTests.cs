// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.EventArchives;
using EventHighway.Portal.Web.Services.Views.EventArchives;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.EventArchives
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

        private static EventArchiveV2 CreateRandomEventArchive(DateTimeOffset archivedDate) =>
            new EventArchiveV2
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Content = GetRandomString(),
                Type = EventArchiveTypeV2.Scheduled,
                Status = EventArchiveStatusV2.Active,
                EventAddressV2Id = Guid.NewGuid(),
                EventAddressV2 = new EventAddressV2 { Name = GetRandomString() },
                EventParticipantV2Id = Guid.NewGuid(),
                ScheduledDate = archivedDate,
                CreatedDate = archivedDate,
                UpdatedDate = archivedDate,
                ArchivedDate = archivedDate
            };

        private static EventArchiveView MapToView(EventArchiveV2 eventArchive) =>
            new EventArchiveView
            {
                Id = eventArchive.Id,
                EventName = eventArchive.EventName,
                Content = eventArchive.Content,
                Type = eventArchive.Type.ToString(),
                Status = eventArchive.Status.ToString(),
                RemainingRetryAttempts = 0,
                EventAddressV2Id = eventArchive.EventAddressV2Id,
                EventAddressName = eventArchive.EventAddressV2?.Name ?? string.Empty,
                EventParticipantV2Id = eventArchive.EventParticipantV2Id,
                ScheduledDate = eventArchive.ScheduledDate,
                CreatedDate = eventArchive.CreatedDate,
                ArchivedDate = eventArchive.ArchivedDate
            };
    }
}
