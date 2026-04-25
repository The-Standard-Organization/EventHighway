// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    internal partial class EventArchiveV1ProcessingService
    {
        private static void ValidateEventArchiveIsNotNull(EventArchiveV1 eventArchive)
        {
            if (eventArchive is null)
            {
                throw new NullEventArchiveV1ProcessingException(
                    message: "Event archive is null.");
            }
        }
    }
}
