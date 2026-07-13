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
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventParticipantSecrets;
using EventHighway.Portal.Web.Services.Views.Foundations.EventParticipantSecrets;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventParticipantSecrets
{
    public partial class EventParticipantSecretsViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventParticipantSecretsViewService eventParticipantSecretsViewService;

        public EventParticipantSecretsViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventParticipantSecretsViewService = new EventParticipantSecretsViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static List<EventParticipantSecretV2> CreateRandomSecrets(
            Guid participantId, int count)
        {
            DateTimeOffset now = GetRandomDateTimeOffset();

            return Enumerable.Range(0, count).Select(_ => new EventParticipantSecretV2
            {
                Id = Guid.NewGuid(),
                Secret = GetRandomString(),
                IsActive = true,
                ActiveFrom = null,
                ActiveTo = null,
                CreatedDate = now,
                UpdatedDate = now,
                EventParticipantV2Id = participantId
            }).ToList();
        }

        private static List<EventParticipantSecretView> MapToViews(
            IEnumerable<EventParticipantSecretV2> secrets) =>
            secrets.Select(secret => new EventParticipantSecretView
            {
                Id = secret.Id,
                Secret = secret.Secret,
                IsActive = secret.IsActive,
                ActiveFrom = secret.ActiveFrom,
                ActiveTo = secret.ActiveTo,
                EventParticipantV2Id = secret.EventParticipantV2Id
            }).ToList();
    }
}
