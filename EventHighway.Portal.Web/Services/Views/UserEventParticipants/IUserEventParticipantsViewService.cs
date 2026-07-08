// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.UserEventParticipants;

namespace EventHighway.Portal.Web.Services.Views.UserEventParticipants
{
    public interface IUserEventParticipantsViewService
    {
        ValueTask<List<UserEventParticipantView>> RetrieveAssociationsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
