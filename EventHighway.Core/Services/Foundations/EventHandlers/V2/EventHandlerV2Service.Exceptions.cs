// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal partial class EventHandlerV2Service
    {
        private delegate void ReturningVoidFunction();
        private delegate IEnumerable<IEventHandler> ReturningEventHandlersFunction();
        private delegate ValueTask<IEventHandler> ReturningEventHandlerFunction();

        private async ValueTask<IEventHandler> TryCatch(
            ReturningEventHandlerFunction returningEventHandlerFunction)
        {
            try
            {
                return await returningEventHandlerFunction();
            }
            catch (NullEventHandlerV2Exception nullEventHandlerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventHandlerV2Exception);
            }
            catch (InvalidEventHandlerV2Exception invalidEventHandlerV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventHandlerV2Exception);
            }
        }

        private async ValueTask<EventHandlerV2ValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventHandlerV2ValidationException =
                new EventHandlerV2ValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventHandlerV2ValidationException);

            return eventHandlerV2ValidationException;
        }

        private IEnumerable<IEventHandler> TryCatch(ReturningEventHandlersFunction returningEventHandlersFunction)
        {
            try
            {
                return returningEventHandlersFunction();
            }
            catch (Exception serviceException)
            {
                var failedEventHandlerV2ServiceException =
                    new FailedEventHandlerV2ServiceException(
                        message: "Failed event handler service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw CreateServiceException(failedEventHandlerV2ServiceException);
            }
        }

        private void TryCatch(ReturningVoidFunction returningVoidFunction)
        {
            try
            {
                returningVoidFunction();
            }
            catch (NullEventHandlerV2Exception nullEventHandlerV2Exception)
            {
                throw CreateValidationException(nullEventHandlerV2Exception);
            }
            catch (InvalidEventHandlerV2Exception invalidEventHandlerV2Exception)
            {
                throw CreateValidationException(invalidEventHandlerV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventHandlerV2ServiceException =
                    new FailedEventHandlerV2ServiceException(
                        message: "Failed event handler service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw CreateServiceException(failedEventHandlerV2ServiceException);
            }
        }

        private static EventHandlerV2ValidationException CreateValidationException(Xeption exception)
        {
            return new EventHandlerV2ValidationException(
                message: "Event handler validation error occurred, fix the errors and try again.",
                innerException: exception);
        }

        private static EventHandlerV2ServiceException CreateServiceException(Xeption exception)
        {
            return new EventHandlerV2ServiceException(
                message: "Event handler service error occurred, contact support.",
                innerException: exception);
        }
    }
}
