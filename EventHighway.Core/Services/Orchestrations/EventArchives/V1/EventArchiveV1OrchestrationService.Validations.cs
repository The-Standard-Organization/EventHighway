// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventArchiveV1OrchestrationService
    {
        private static void ValidateEventArchiveV1(EventArchiveV1 eventArchiveV1)
        {
            ValidateEventArchiveV1IsNotNull(eventArchiveV1);
            ValidateListenerEventArchiveV1sAreNotNull(eventArchiveV1);
        }

        private static void ValidateEventArchiveV1IsNotNull(EventArchiveV1 eventArchiveV1)
        {
            if (eventArchiveV1 is null)
            {
                throw new NullEventArchiveV1OrchestrationException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateListenerEventArchiveV1sAreNotNull(EventArchiveV1 eventArchiveV1)
        {
            if (eventArchiveV1.ListenerEventArchiveV1s is null)
            {
                throw new NullListenerEventArchiveV1sOrchestrationException(
                    message: "Listener event archives are null.");
            }
        }
    }
}
