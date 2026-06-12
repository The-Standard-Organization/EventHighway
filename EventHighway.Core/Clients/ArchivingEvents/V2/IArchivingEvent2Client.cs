// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Clients.ArchivingEvents.V2
{
    public interface IArchivingEvent2Client
    {
        ValueTask<IQueryable<EventV2>> RetrieveAllDeadEventV2sWithListenersAsync(
            CancellationToken cancellationToken = default);

        ValueTask RemoveEventV2AndListenerEventV2sAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);
    }
}
