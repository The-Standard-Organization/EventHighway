// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Brokers.Storages;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.UserEventParticipants;

namespace EventHighway.Portal.Web.Brokers.UserEventParticipants
{
    public sealed class UserEventParticipantBroker : IUserEventParticipantBroker
    {
        private readonly SecurityDbContext securityDbContext;

        public UserEventParticipantBroker(SecurityDbContext securityDbContext) =>
            this.securityDbContext = securityDbContext;

        public async ValueTask<UserEventParticipant> InsertUserEventParticipantAsync(
            UserEventParticipant userEventParticipant)
        {
            await this.securityDbContext.UserEventParticipants.AddAsync(userEventParticipant);
            await this.securityDbContext.SaveChangesAsync();

            return userEventParticipant;
        }

        public IQueryable<UserEventParticipant> SelectAllUserEventParticipants() =>
            this.securityDbContext.UserEventParticipants;

        public async ValueTask<UserEventParticipant?> SelectUserEventParticipantByIdAsync(
            Guid userEventParticipantId) =>
            await this.securityDbContext.UserEventParticipants.FindAsync(userEventParticipantId);

        public async ValueTask<UserEventParticipant> DeleteUserEventParticipantAsync(
            UserEventParticipant userEventParticipant)
        {
            this.securityDbContext.UserEventParticipants.Remove(userEventParticipant);
            await this.securityDbContext.SaveChangesAsync();

            return userEventParticipant;
        }
    }
}
