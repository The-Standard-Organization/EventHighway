// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventArchiveV1OrchestrationService
    {
        private static void ValidateEventV1Arhive(EventArchiveV1 eventArchiveV1)
        {
            ValidateEventV1ArhiveIsNotNull(eventArchiveV1);
            ValidateListenerEventV1ArhivesAreNotNull(eventArchiveV1);
        }

        private static void ValidateEventV1ArhiveIsNotNull(EventArchiveV1 eventArchiveV1)
        {
            if (eventArchiveV1 is null)
            {
                throw new NullEventArchiveV1OrchestrationException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateListenerEventV1ArhivesAreNotNull(EventArchiveV1 eventArchiveV1)
        {
            if (eventArchiveV1.ListenerEventArchiveV1s is null)
            {
                throw new NullListenerEventArchiveV1sOrchestrationException(
                    message: "Listener event archives are null.");
            }
        }
    }
}
