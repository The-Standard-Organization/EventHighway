// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventArchiveV1OrchestrationService
    {
        private static void ValidateEventArchiveV1(EventArchiveV1 eventV1Archive)
        {
            ValidateEventV1ArhiveIsNotNull(eventV1Archive);
            ValidateListenerEventV1ArhivesAreNotNull(eventV1Archive);
        }

        private static void ValidateEventV1ArhiveIsNotNull(EventArchiveV1 eventV1Archive)
        {
            if (eventV1Archive is null)
            {
                throw new NullEventArchiveV1OrchestrationException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateListenerEventV1ArhivesAreNotNull(EventArchiveV1 eventV1Archive)
        {
            if (eventV1Archive.ListenerEventArchives is null)
            {
                throw new NullListenerEventArchiveV1sOrchestrationException(
                    message: "Listener event archives are null.");
            }
        }
    }
}
