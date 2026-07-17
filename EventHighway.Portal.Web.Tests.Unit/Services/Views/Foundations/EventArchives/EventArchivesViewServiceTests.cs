// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
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

        private static EventArchiveV2 CreateRandomEventArchive(DateTimeOffset archivedDate)
        {
            Guid eventAddressId = Guid.NewGuid();

            return new EventArchiveV2
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Content = GetRandomString(),
                Type = EventArchiveTypeV2.Scheduled,
                Status = EventArchiveStatusV2.Active,
                EventAddressV2Id = eventAddressId,

                EventAddressV2 = new EventAddressV2
                {
                    Id = eventAddressId,
                    Name = GetRandomString()
                },

                EventParticipantV2Id = Guid.NewGuid(),
                ScheduledDate = archivedDate,
                CreatedDate = archivedDate,
                ArchivedDate = archivedDate
            };
        }

        private static ListenerEventArchiveV2 CreateListenerEventArchive(
            Guid eventArchiveId,
            ListenerEventArchiveStatusV2 status) =>
            new ListenerEventArchiveV2
            {
                Id = Guid.NewGuid(),
                EventArchiveV2Id = eventArchiveId,
                Status = status
            };

        private static EventArchiveView MapToView(
            EventArchiveV2 eventArchive,
            IEnumerable<ListenerEventArchiveV2> listenerEventArchives)
        {
            List<ListenerEventArchiveV2> archiveListenerEventArchives = listenerEventArchives
                .Where(listenerEventArchive =>
                    listenerEventArchive.EventArchiveV2Id == eventArchive.Id)
                .ToList();

            return new EventArchiveView
            {
                Id = eventArchive.Id,
                EventName = eventArchive.EventName ?? string.Empty,
                Content = eventArchive.Content ?? string.Empty,
                Type = eventArchive.Type.ToString(),
                Status = eventArchive.Status.ToString(),
                EventAddressV2Id = eventArchive.EventAddressV2Id,
                EventAddressName = eventArchive.EventAddressV2?.Name ?? string.Empty,
                EventParticipantV2Id = eventArchive.EventParticipantV2Id,
                ScheduledDate = eventArchive.ScheduledDate,
                CreatedDate = eventArchive.CreatedDate,
                ArchivedDate = eventArchive.ArchivedDate,
                ListenerEventCount = archiveListenerEventArchives.Count,

                SucceededListenerEventCount = archiveListenerEventArchives.Count(
                    listenerEventArchive =>
                        listenerEventArchive.Status == ListenerEventArchiveStatusV2.Success)
            };
        }
    }
}
