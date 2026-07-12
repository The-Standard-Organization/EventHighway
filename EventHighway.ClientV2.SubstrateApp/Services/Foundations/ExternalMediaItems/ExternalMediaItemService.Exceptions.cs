// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems.Exceptions;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems
{
    public partial class ExternalMediaItemService
    {
        private delegate ValueTask<MediaItem> ReturningMediaItemFunction();

        private async ValueTask<MediaItem> TryCatch(
            ReturningMediaItemFunction returningMediaItemFunction)
        {
            try
            {
                return await returningMediaItemFunction();
            }
            catch (NullExternalMediaItemException nullExternalMediaItemException)
            {
                throw CreateAndLogValidationException(nullExternalMediaItemException);
            }
            catch (InvalidExternalMediaItemException invalidExternalMediaItemException)
            {
                throw CreateAndLogValidationException(invalidExternalMediaItemException);
            }
            catch (Exception exception)
            {
                var failedExternalMediaItemServiceException =
                    new FailedExternalMediaItemServiceException(
                        message: "Failed external media item service error occurred, contact support.",
                        innerException: exception);

                throw CreateAndLogServiceException(failedExternalMediaItemServiceException);
            }
        }

        private ExternalMediaItemValidationException CreateAndLogValidationException(Xeption exception)
        {
            var externalMediaItemValidationException =
                new ExternalMediaItemValidationException(
                    message: "External media item validation error occurred, fix the errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogError(externalMediaItemValidationException);

            return externalMediaItemValidationException;
        }

        private ExternalMediaItemServiceException CreateAndLogServiceException(Xeption exception)
        {
            var externalMediaItemServiceException =
                new ExternalMediaItemServiceException(
                    message: "External media item service error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError(externalMediaItemServiceException);

            return externalMediaItemServiceException;
        }
    }
}
