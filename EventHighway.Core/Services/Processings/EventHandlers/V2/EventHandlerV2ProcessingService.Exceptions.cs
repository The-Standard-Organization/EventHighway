// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventHandlers.V2
{
    internal partial class EventHandlerV2ProcessingService
    {
        private delegate ValueTask<IEventHandler> ReturningEventHandlerFunction();

        private async ValueTask<IEventHandler> TryCatch(
            ReturningEventHandlerFunction returningEventHandlerFunction)
        {
            try
            {
                return await returningEventHandlerFunction();
            }
            catch (NullEventHandlerV2ProcessingException nullEventHandlerV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventHandlerV2ProcessingException);
            }
        }

        private async ValueTask<EventHandlerV2ProcessingValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventHandlerV2ProcessingValidationException =
                new EventHandlerV2ProcessingValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2ProcessingValidationException);

            return eventHandlerV2ProcessingValidationException;
        }
    }
}
