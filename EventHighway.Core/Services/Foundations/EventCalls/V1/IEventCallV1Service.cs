// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;

namespace EventHighway.Core.Services.Foundations.EventCalls.V1
{
    internal interface IEventCallV1Service
    {
        ValueTask<EventCallV1> RunEventCallV1Async(EventCallV1 eventCallV1);
        ValueTask<EventCallV1> RunEventCallV1AsyncV1(EventCallV1 eventCallV1);
    }
}
