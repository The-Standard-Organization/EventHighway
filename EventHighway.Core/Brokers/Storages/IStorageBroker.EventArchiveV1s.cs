// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventArchiveV1> InsertEventArchiveV1Async(EventArchiveV1 eventArchiveV1);
        ValueTask<IQueryable<EventArchiveV1>> SelectAllEventArchiveV1sAsync();
        ValueTask<EventArchiveV1> SelectEventArchiveV1ByIdAsync(Guid eventArchiveV1Id);
        ValueTask<EventArchiveV1> DeleteEventArchiveV1Async(EventArchiveV1 eventArchiveV1V1);
    }
}
