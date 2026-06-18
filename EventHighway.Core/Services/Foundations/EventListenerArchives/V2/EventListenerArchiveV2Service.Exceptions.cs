// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventListenerArchives.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventListenerArchives.V2
{
    internal partial class EventListenerArchiveV2Service
    {
        private delegate ValueTask<EventListenerArchiveV2> ReturningEventListenerArchiveV2Function();

        private async ValueTask<EventListenerArchiveV2> TryCatch(
            ReturningEventListenerArchiveV2Function returningEventListenerArchiveV2Function)
        {
            try
            {
                return await returningEventListenerArchiveV2Function();
            }
            catch (NullEventListenerArchiveV2Exception nullEventListenerArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventListenerArchiveV2Exception);
            }
            catch (InvalidEventListenerArchiveV2Exception invalidEventListenerArchiveV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerArchiveV2Exception);
            }
        }

        private async ValueTask<EventListenerArchiveV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventListenerArchiveV2ValidationException =
                new EventListenerArchiveV2ValidationException(
                    message: "Event listener archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerArchiveV2ValidationException);

            return eventListenerArchiveV2ValidationException;
        }
    }
}
