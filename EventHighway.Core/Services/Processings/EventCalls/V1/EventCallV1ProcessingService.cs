// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Services.Foundations.EventCalls.V1;

namespace EventHighway.Core.Services.Processings.EventCalls.V1
{
    internal partial class EventCallV1ProcessingService : IEventCallV1ProcessingService
    {
        private readonly IEventCallV1Service eventCallV1Service;
        private readonly ILoggingBroker loggingBroker;

        public EventCallV1ProcessingService(
            IEventCallV1Service eventCallV1Service,
            ILoggingBroker loggingBroker)
        {
            this.eventCallV1Service = eventCallV1Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventCallV1> RunEventCallAsync(EventCallV1 eventCallV1) =>
        TryCatch(async () =>
        {
            ValidateEventCallV1IsNotNull(eventCallV1);

            return await this.eventCallV1Service.RunEventCallAsync(eventCallV1);
        });

        public ValueTask<EventCallV1> RunEventCallAsyncV1(EventCallV1 eventCallV1) =>
        TryCatch(async () =>
        {
            ValidateEventCallV1IsNotNull(eventCallV1);

            return await this.eventCallV1Service.RunEventCallAsyncV1(eventCallV1);
        });
    }
}
