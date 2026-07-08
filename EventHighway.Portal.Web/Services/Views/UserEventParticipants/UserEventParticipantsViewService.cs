// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Identities;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Brokers.UserEventParticipants;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants;

namespace EventHighway.Portal.Web.Services.Views.UserEventParticipants
{
    public partial class UserEventParticipantsViewService : IUserEventParticipantsViewService
    {
        private readonly IUserEventParticipantBroker userEventParticipantBroker;
        private readonly IIdentityBroker identityBroker;
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserEventParticipantsViewService(
            IUserEventParticipantBroker userEventParticipantBroker,
            IIdentityBroker identityBroker,
            IEventHighwayBroker eventHighwayBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.userEventParticipantBroker = userEventParticipantBroker;
            this.identityBroker = identityBroker;
            this.eventHighwayBroker = eventHighwayBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        private const string ParticipantNotFoundName = "(participant not found)";

        public ValueTask<List<UserEventParticipantView>> RetrieveAssociationsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
