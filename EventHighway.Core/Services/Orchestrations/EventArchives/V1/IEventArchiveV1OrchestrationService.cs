// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    public interface IEventArchiveV1OrchestrationService
    {
        ValueTask AddEventArchiveV1WithListenerEventArchiveV1sAsync(EventArchiveV1 eventArchiveV1);
    }
}
