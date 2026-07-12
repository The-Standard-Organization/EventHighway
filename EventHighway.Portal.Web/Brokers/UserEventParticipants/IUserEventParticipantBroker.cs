// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Services.Domains.Foundations.UserEventParticipants;

namespace EventHighway.Portal.Web.Brokers.UserEventParticipants
{
    // The user-to-participant association is plain EF on the security database (not an Identity
    // concept), so it is wrapped by its own storage broker. Services depend on the broker, never
    // on the DbContext directly.
    public interface IUserEventParticipantBroker
    {
        ValueTask<UserEventParticipant> InsertUserEventParticipantAsync(
            UserEventParticipant userEventParticipant);

        IQueryable<UserEventParticipant> SelectAllUserEventParticipants();

        ValueTask<UserEventParticipant> SelectUserEventParticipantByIdAsync(
            Guid userEventParticipantId);

        ValueTask<UserEventParticipant> DeleteUserEventParticipantAsync(
            UserEventParticipant userEventParticipant);
    }
}
