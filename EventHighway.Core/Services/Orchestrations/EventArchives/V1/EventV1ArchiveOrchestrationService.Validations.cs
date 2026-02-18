// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.Events;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventV1ArchiveOrchestrationService
    {
        private static void ValidateEventV1ArhiveIsNotNull(EventV1Archive eventV1Archive)
        {
            if(eventV1Archive is null)
            {
                throw new NullEventV1ArchiveOrchestrationException(
                    message: "Event archive is null.");
            }
        }
    }
}
