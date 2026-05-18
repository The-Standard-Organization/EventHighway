// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    public interface IEventArchiveV1ProcessingService
    {
        ValueTask<EventArchiveV1> AddEventArchiveV1Async(EventArchiveV1 eventArchiveV1);
    }
}
