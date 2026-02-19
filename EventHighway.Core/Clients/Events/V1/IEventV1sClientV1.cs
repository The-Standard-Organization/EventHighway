// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace EventHighway.Core.Clients.Events.V1
{
    public interface IEventV1sClientV1
    {
        ValueTask ArchiveDeadEventV1sAsync();
    }
}
