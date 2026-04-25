// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V2;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventArchiveV2> InsertEventArchiveV2Async(EventArchiveV2 eventArchive);
        ValueTask<IQueryable<EventArchiveV2>> SelectAllEventArchivesV2Async();
        ValueTask<EventArchiveV2> SelectEventArchiveByIdV2Async(Guid eventArchiveId);
        ValueTask<EventArchiveV2> DeleteEventArchiveV2Async(EventArchiveV2 eventArchive);
    }
}
