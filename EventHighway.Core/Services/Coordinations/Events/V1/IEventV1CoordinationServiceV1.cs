// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace EventHighway.Core.Services.Coordinations.Events.V1
{
    public partial interface IEventV1CoordinationServiceV1
    {
        ValueTask ArchiveDeadEventV1sAsync();
    }
}
