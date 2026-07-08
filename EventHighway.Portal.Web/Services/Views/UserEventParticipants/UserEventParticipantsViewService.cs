// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Identities;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Brokers.UserEventParticipants;
using EventHighway.Portal.Web.Models.Foundations.UserEventParticipants;
using EventHighway.Portal.Web.Models.Foundations.Users;
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

        public async ValueTask<List<UserEventParticipantView>> RetrieveAssociationsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            List<UserEventParticipant> associations =
                this.userEventParticipantBroker.SelectAllUserEventParticipants()
                    .Where(association => association.UserId == userId)
                    .ToList();

            var views = new List<UserEventParticipantView>();

            foreach (UserEventParticipant association in associations)
            {
                EventParticipantV2 participant =
                    await this.eventHighwayBroker.RetrieveEventParticipantV2ByIdAsync(
                        association.EventParticipantId, cancellationToken);

                views.Add(new UserEventParticipantView
                {
                    Id = association.Id,
                    UserId = association.UserId,
                    EventParticipantId = association.EventParticipantId,
                    EventParticipantName = participant?.Name ?? ParticipantNotFoundName
                });
            }

            return views;
        }

        public async ValueTask<List<UserEventParticipantView>>
            RetrieveAssociationsByParticipantIdAsync(
                Guid eventParticipantId,
                CancellationToken cancellationToken = default)
        {
            List<UserEventParticipant> associations =
                this.userEventParticipantBroker.SelectAllUserEventParticipants()
                    .Where(association => association.EventParticipantId == eventParticipantId)
                    .ToList();

            var views = new List<UserEventParticipantView>();

            foreach (UserEventParticipant association in associations)
            {
                AppUser user =
                    await this.identityBroker.SelectUserByIdAsync(association.UserId);

                if (user is null)
                {
                    continue;
                }

                views.Add(new UserEventParticipantView
                {
                    Id = association.Id,
                    UserId = association.UserId,
                    UserName = user.UserName ?? string.Empty,
                    UserEmail = user.Email ?? string.Empty,
                    EventParticipantId = association.EventParticipantId
                });
            }

            return views;
        }

        public async ValueTask<UserEventParticipantView> AddAssociationAsync(
            Guid userId,
            Guid eventParticipantId,
            CancellationToken cancellationToken = default)
        {
            AppUser user = await this.identityBroker.SelectUserByIdAsync(userId);

            EventParticipantV2 participant =
                await this.eventHighwayBroker.RetrieveEventParticipantV2ByIdAsync(
                    eventParticipantId, cancellationToken);

            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            var associationToAdd = new UserEventParticipant
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventParticipantId = eventParticipantId,
                CreatedDate = now
            };

            UserEventParticipant addedAssociation =
                await this.userEventParticipantBroker.InsertUserEventParticipantAsync(
                    associationToAdd);

            return new UserEventParticipantView
            {
                Id = addedAssociation.Id,
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                UserEmail = user.Email ?? string.Empty,
                EventParticipantId = eventParticipantId,
                EventParticipantName = participant.Name
            };
        }

        public async ValueTask RemoveAssociationByIdAsync(
            Guid associationId,
            CancellationToken cancellationToken = default)
        {
            UserEventParticipant association =
                await this.userEventParticipantBroker.SelectUserEventParticipantByIdAsync(
                    associationId);

            await this.userEventParticipantBroker.DeleteUserEventParticipantAsync(association);
        }
    }
}
