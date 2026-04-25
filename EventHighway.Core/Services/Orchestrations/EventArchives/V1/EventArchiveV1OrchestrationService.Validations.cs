// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventArchiveV1OrchestrationService
    {
        private static void ValidateEventArchive(EventArchiveV1 eventArchive)
        {
            ValidateEventArhiveIsNotNull(eventArchive);
            ValidateListenerEventArhivesAreNotNull(eventArchive);
        }

        private static void ValidateEventArhiveIsNotNull(EventArchiveV1 eventArchive)
        {
            if (eventArchive is null)
            {
                throw new NullEventArchiveV1OrchestrationException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateListenerEventArhivesAreNotNull(EventArchiveV1 eventArchive)
        {
            if (eventArchive.ListenerEventArchives is null)
            {
                throw new NullListenerEventArchiveV1sOrchestrationException(
                    message: "Listener event archives are null.");
            }
        }
    }
}
