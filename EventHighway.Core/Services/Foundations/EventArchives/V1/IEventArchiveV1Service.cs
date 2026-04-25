// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    public partial interface IEventArchiveV1Service
    {
        ValueTask<EventArchiveV1> AddEventArchiveAsync(EventArchiveV1 eventArchive);
        ValueTask<IQueryable<EventArchiveV1>> RetrieveAllEventArchivesAsync();
        ValueTask<EventArchiveV1> RetrieveEventArchiveByIdAsync(Guid eventArchiveId);
    }
}
