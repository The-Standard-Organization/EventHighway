// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    internal partial class EventV1ArchiveProcessingService
    {
        private static void ValidateEventV1ArchiveIsNotNull(EventV1Archive eventV1Archive)
        {
            if (eventV1Archive is null)
            {
                throw new NullEventV1ArchiveProcessingException(
                    message: "Event archive is null.");
            }
        }
    }
}
