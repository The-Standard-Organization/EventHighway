// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Services.Coordinations.Events.V1
{
    public partial interface IEventV1CoordinationServiceV1
    {
        ValueTask ArchiveDeadEventV1sAsync();
        ValueTask CleanUpArchiveDeadEventV1sAsync(
            ArchiveDeletionPolicy policy, int duration = 0);
    }
}
