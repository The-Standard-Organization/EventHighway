// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventArchiveV1> InsertEventArchiveV1Async(EventArchiveV1 eventArchive);
        ValueTask<IQueryable<EventArchiveV1>> SelectAllEventArchivesV1Async();
        ValueTask<EventArchiveV1> SelectEventArchiveByIdV1Async(Guid eventArchiveId);
        ValueTask<EventArchiveV1> DeleteEventArchiveV1Async(EventArchiveV1 eventArchive);
    }
}
