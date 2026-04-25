// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Processings.EventCalls.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.EventCalls.V1
{
    internal partial class EventCallV1ProcessingService
    {
        private static void ValidateEventCallIsNotNull(EventCallV1 eventCall)
        {
            if (eventCall is null)
            {
                throw new NullEventCallV1ProcessingException(
                    message: "Event call is null.");
            }
        }
    }
}
