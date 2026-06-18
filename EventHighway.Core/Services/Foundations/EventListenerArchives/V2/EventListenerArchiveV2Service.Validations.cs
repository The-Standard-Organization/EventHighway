// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventListenerArchives.V2
{
    internal partial class EventListenerArchiveV2Service
    {
        private static void ValidateEventListenerArchiveV2IsNotNull(
            EventListenerArchiveV2 eventListenerArchiveV2)
        {
            if (eventListenerArchiveV2 is null)
            {
                throw new NullEventListenerArchiveV2Exception(
                    message: "Event listener archive is null.");
            }
        }
    }
}
