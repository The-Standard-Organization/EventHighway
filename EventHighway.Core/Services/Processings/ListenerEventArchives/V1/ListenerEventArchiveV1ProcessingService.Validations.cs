// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V1
{
    internal partial class ListenerEventArchiveV1ProcessingService
    {
        private void ValidateListenerEventArchive(ListenerEventArchiveV1 listenerEventArchive)
        {
            if (listenerEventArchive is null)
            {
                throw new NullListenerEventArchiveV1ProcessingException(
                    message: "Listener event archive is null.");
            }
        }
    }
}