// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Services.Foundations.EventHandlers.V2;

namespace EventHighway.Core.Services.Processings.EventHandlers.V2
{
    internal partial class EventHandlerV2ProcessingService : IEventHandlerV2ProcessingService
    {
        private readonly IEventHandlerV2Service eventHandlerV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventHandlerV2ProcessingService(
            IEventHandlerV2Service eventHandlerV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventHandlerV2Service = eventHandlerV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEventHandler> RegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
