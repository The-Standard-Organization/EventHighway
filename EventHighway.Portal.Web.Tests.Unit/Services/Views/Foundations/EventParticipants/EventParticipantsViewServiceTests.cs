// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipants;
using EventHighway.Portal.Web.Services.Views.Foundations.EventParticipants;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventParticipants
{
    public partial class EventParticipantsViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventParticipantsViewService eventParticipantsViewService;

        public EventParticipantsViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventParticipantsViewService = new EventParticipantsViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static EventParticipantView CreateRandomParticipantView() =>
            new EventParticipantView
            {
                Name = GetRandomString(),
                Description = GetRandomString(),
                ContactEmail = GetRandomString(),
                ContactPhone = GetRandomString(),
                IsActive = true,
                ActiveFrom = null,
                ActiveTo = null
            };

        private static List<EventParticipantV2> CreateRandomParticipants() =>
            Enumerable.Range(0, 3).Select(_ => new EventParticipantV2
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                ContactEmail = GetRandomString(),
                ContactPhone = GetRandomString(),
                IsActive = true,
                ActiveFrom = null,
                ActiveTo = null,
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            }).ToList();

        private static List<EventParticipantView> MapToViews(
            IEnumerable<EventParticipantV2> participants) =>
            participants.Select(participant => new EventParticipantView
            {
                Id = participant.Id,
                Name = participant.Name,
                Description = participant.Description,
                ContactEmail = participant.ContactEmail,
                ContactPhone = participant.ContactPhone,
                IsActive = participant.IsActive,
                ActiveFrom = participant.ActiveFrom,
                ActiveTo = participant.ActiveTo
            }).ToList();
    }
}
