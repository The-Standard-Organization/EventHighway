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
        private static void ValidateEventV1Arhive(EventV1Archive eventV1Archive)
        {
            ValidateEventV1ArhiveIsNotNull(eventV1Archive);
            ValidateListenerEventV1ArhivesAreNotNull(eventV1Archive);
        }

        private static void ValidateEventV1ArhiveIsNotNull(EventV1Archive eventV1Archive)
        {
            if(eventV1Archive is null)
            {
                throw new NullEventV1ArchiveOrchestrationException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateListenerEventV1ArhivesAreNotNull(EventV1Archive eventV1Archive)
        {
            if (eventV1Archive.ListenerEventV1Archives is null)
            {
                throw new NullListenerEventV1ArchivesOrchestrationException(
                    message: "Listener event archives are null.");
            }
        }
    }
}
