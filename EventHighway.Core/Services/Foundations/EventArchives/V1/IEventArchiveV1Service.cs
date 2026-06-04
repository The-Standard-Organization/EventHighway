// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    public partial interface IEventArchiveV1Service
    {
        ValueTask<EventArchiveV1> AddEventArchiveV1Async(EventArchiveV1 eventArchiveV1);
        ValueTask<IQueryable<EventArchiveV1>> RetrieveAllEventArchiveV1sAsync();
        ValueTask<EventArchiveV1> RetrieveEventArchiveV1ByIdAsync(Guid eventArchiveV1Id);
    }
}
