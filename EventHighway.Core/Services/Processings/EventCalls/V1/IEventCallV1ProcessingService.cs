// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;

namespace EventHighway.Core.Services.Processings.EventCalls.V1
{
    internal interface IEventCallV1ProcessingService
    {
        ValueTask<EventCallV1> RunEventCallAsync(EventCallV1 eventCall);
        ValueTask<EventCallV1> RunEventCallV1Async(EventCallV1 eventCall);
    }
}
