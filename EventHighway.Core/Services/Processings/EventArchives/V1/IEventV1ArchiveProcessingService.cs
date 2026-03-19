// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    public interface IEventV1ArchiveProcessingService
    {
        ValueTask<EventV1Archive> AddEventV1ArchiveAsync(EventV1Archive eventV1Archive);
        ValueTask<IQueryable<EventV1Archive>> RetrieveAllEventV1ArchivesAsync();
        ValueTask<EventV1Archive> RetrieveEventV1ArchiveByIdAsync(Guid eventV1ArchiveId);
    }
}
