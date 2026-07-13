// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace EventHighway.Core.Clients.Events.V1
{
    public interface IEventV1sClientV1
    {
        [Obsolete("This function is deprecated use the latest version instead.")]
        ValueTask ArchiveDeadEventV1sAsync();
    }
}
