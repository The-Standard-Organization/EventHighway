// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Services.Foundations.EventArchives.V1
{
    public partial interface IEventV1ArchiveService
    {
        ValueTask<EventV1Archive> AddEventV1ArchiveAsync(EventV1Archive eventV1Archive);
    }
}
