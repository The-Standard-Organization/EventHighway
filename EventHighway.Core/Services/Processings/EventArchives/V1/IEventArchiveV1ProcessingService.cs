// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    public interface IEventArchiveV1ProcessingService
    {
        ValueTask<EventArchiveV1> AddEventArchiveAsync(EventArchiveV1 eventArchive);
    }
}
