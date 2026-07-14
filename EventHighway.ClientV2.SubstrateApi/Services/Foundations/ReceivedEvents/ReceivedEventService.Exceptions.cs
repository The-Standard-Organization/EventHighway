// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents;
using EventHighway.ClientV2.SubstrateApi.Models.ReceivedEvents.Exceptions;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.ReceivedEvents
{
    public partial class ReceivedEventService
    {
        private delegate ValueTask<ReceivedEvent> ReturningReceivedEventFunction();
        private delegate ValueTask<IQueryable<ReceivedEvent>> ReturningReceivedEventsFunction();

        private async ValueTask<ReceivedEvent> TryCatch(
            ReturningReceivedEventFunction returningReceivedEventFunction)
        {
            try
            {
                return await returningReceivedEventFunction();
            }
            catch (NullReceivedEventException nullReceivedEventException)
            {
                throw CreateAndLogValidationException(nullReceivedEventException);
            }
            catch (InvalidReceivedEventException invalidReceivedEventException)
            {
                throw CreateAndLogValidationException(invalidReceivedEventException);
            }
            catch (Exception exception)
            {
                var failedReceivedEventServiceException =
                    new FailedReceivedEventServiceException(
                        message: "Failed received event service error occurred, contact support.",
                        innerException: exception);

                throw CreateAndLogServiceException(failedReceivedEventServiceException);
            }
        }

        private async ValueTask<IQueryable<ReceivedEvent>> TryCatch(
            ReturningReceivedEventsFunction returningReceivedEventsFunction)
        {
            try
            {
                return await returningReceivedEventsFunction();
            }
            catch (Exception exception)
            {
                var failedReceivedEventServiceException =
                    new FailedReceivedEventServiceException(
                        message: "Failed received event service error occurred, contact support.",
                        innerException: exception);

                throw CreateAndLogServiceException(failedReceivedEventServiceException);
            }
        }

        private ReceivedEventValidationException CreateAndLogValidationException(Xeption exception)
        {
            var receivedEventValidationException =
                new ReceivedEventValidationException(
                    message: "Received event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogError(receivedEventValidationException);

            return receivedEventValidationException;
        }

        private ReceivedEventServiceException CreateAndLogServiceException(Xeption exception)
        {
            var receivedEventServiceException =
                new ReceivedEventServiceException(
                    message: "Received event service error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError(receivedEventServiceException);

            return receivedEventServiceException;
        }
    }
}
