// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V1
{
    internal partial class ListenerEventV1ArchiveProcessingService
    {
        private void ValidateListenerEventV1Archive(ListenerEventV1Archive listenerEventV1Archive)
        {
            if (listenerEventV1Archive is null)
            {
                throw new NullListenerEventV1ArchiveProcessingException(
                    message: "Listener event archive is null.");
            }
        }
    }
}