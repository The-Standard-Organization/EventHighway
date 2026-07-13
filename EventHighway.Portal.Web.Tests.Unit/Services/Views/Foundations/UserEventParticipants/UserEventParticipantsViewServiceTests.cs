// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Identities;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Brokers.UserEventParticipants;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users;
using EventHighway.Portal.Web.Services.Views.Foundations.UserEventParticipants;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.UserEventParticipants
{
    public partial class UserEventParticipantsViewServiceTests
    {
        private readonly Mock<IUserEventParticipantBroker> userEventParticipantBrokerMock;
        private readonly Mock<IIdentityBroker> identityBrokerMock;
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IUserEventParticipantsViewService userEventParticipantsViewService;

        public UserEventParticipantsViewServiceTests()
        {
            this.userEventParticipantBrokerMock = new Mock<IUserEventParticipantBroker>();
            this.identityBrokerMock = new Mock<IIdentityBroker>();
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userEventParticipantsViewService = new UserEventParticipantsViewService(
                userEventParticipantBroker: this.userEventParticipantBrokerMock.Object,
                identityBroker: this.identityBrokerMock.Object,
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Guid GetRandomGuid() =>
            Guid.NewGuid();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static UserEventParticipant CreateRandomAssociation(
            Guid userId,
            Guid eventParticipantId) =>
            new UserEventParticipant
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventParticipantId = eventParticipantId,
                CreatedDate = GetRandomDateTimeOffset()
            };

        private static AppUser CreateRandomUser(Guid userId) =>
            new AppUser
            {
                Id = userId,
                UserName = GetRandomString(),
                Email = GetRandomString()
            };

        private static EventParticipantV2 CreateRandomParticipant(Guid participantId) =>
            new EventParticipantV2
            {
                Id = participantId,
                Name = GetRandomString()
            };

        private static System.Linq.Expressions.Expression<Func<Xeption, bool>>
            SameExceptionAs(Xeption expectedException) =>
            actualException =>
                actualException.SameExceptionAs(expectedException);
    }
}
