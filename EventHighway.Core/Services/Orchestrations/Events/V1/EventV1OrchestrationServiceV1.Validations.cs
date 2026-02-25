// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.Events.V1
{
    internal partial class EventV1OrchestrationServiceV1
    {
        private static void ValidateEventV1IsNotNull(EventV1 eventV1)
        {
            if (eventV1 is null)
            {
                throw new NullEventV1OrchestrationException(
                    message: "Event is null.");
            }
        }
    }
}
