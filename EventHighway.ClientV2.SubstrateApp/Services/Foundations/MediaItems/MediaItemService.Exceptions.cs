// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems.Exceptions;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems
{
    internal partial class MediaItemService
    {
        private delegate ValueTask<MediaItem> ReturningMediaItemFunction();
        private delegate ValueTask<IQueryable<MediaItem>> ReturningMediaItemsFunction();

        private async ValueTask<MediaItem> TryCatch(
            ReturningMediaItemFunction returningMediaItemFunction)
        {
            try
            {
                return await returningMediaItemFunction();
            }
            catch (NullMediaItemException nullMediaItemException)
            {
                throw CreateAndLogValidationException(nullMediaItemException);
            }
            catch (InvalidMediaItemException invalidMediaItemException)
            {
                throw CreateAndLogValidationException(invalidMediaItemException);
            }
            catch (NotFoundMediaItemException notFoundMediaItemException)
            {
                throw CreateAndLogValidationException(notFoundMediaItemException);
            }
            catch (Exception exception)
            {
                var failedMediaItemServiceException =
                    new FailedMediaItemServiceException(
                        message: "Failed media item service error occurred, contact support.",
                        innerException: exception);

                throw CreateAndLogServiceException(failedMediaItemServiceException);
            }
        }

        private async ValueTask<IQueryable<MediaItem>> TryCatch(
            ReturningMediaItemsFunction returningMediaItemsFunction)
        {
            try
            {
                return await returningMediaItemsFunction();
            }
            catch (Exception exception)
            {
                var failedMediaItemServiceException =
                    new FailedMediaItemServiceException(
                        message: "Failed media item service error occurred, contact support.",
                        innerException: exception);

                throw CreateAndLogServiceException(failedMediaItemServiceException);
            }
        }

        private MediaItemValidationException CreateAndLogValidationException(Xeption exception)
        {
            var mediaItemValidationException =
                new MediaItemValidationException(
                    message: "Media item validation error occurred, fix the errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogError(mediaItemValidationException);

            return mediaItemValidationException;
        }

        private MediaItemServiceException CreateAndLogServiceException(Xeption exception)
        {
            var mediaItemServiceException =
                new MediaItemServiceException(
                    message: "Media item service error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError(mediaItemServiceException);

            return mediaItemServiceException;
        }
    }
}
