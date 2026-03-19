// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    public partial interface IEventV1ArchiveService
    {
        ValueTask<EventV1Archive> AddEventV1ArchiveAsync(
            EventV1Archive eventV1Archive);

        ValueTask<IQueryable<EventV1Archive>> RetrieveAllEventV1ArchivesAsync();

        ValueTask<EventV1Archive> RemoveEventV1ArchiveByIdAsync(
            Guid eventArchiveV1Id);
    }
}
