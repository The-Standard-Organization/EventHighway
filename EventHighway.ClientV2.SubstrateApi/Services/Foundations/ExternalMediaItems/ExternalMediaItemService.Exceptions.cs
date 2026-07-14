// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApi.Models.ExternalMediaItems.Exceptions;
using EventHighway.ClientV2.SubstrateApi.Models.MediaItems;
using EventHighway.Core.Models.Clients.Events.V2.Exceptions;
using Xeptions;

namespace EventHighway.ClientV2.SubstrateApi.Services.Foundations.ExternalMediaItems
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

            // The substrate's own refusals — an unknown participant, a secret that does not match,
            // an item it quarantined as a loop — are the contributor's to fix, not ours. The
            // console sample only ever printed them; behind an HTTP intake they have to keep their
            // shape all the way out to the caller, so they are mapped rather than flattened into a
            // service failure.
            catch (EventV2ClientValidationException eventV2ClientValidationException)
            {
                throw CreateAndLogDependencyValidationException(
                    eventV2ClientValidationException);
            }
            catch (EventV2ClientDependencyException eventV2ClientDependencyException)
            {
                throw CreateAndLogDependencyException(
                    eventV2ClientDependencyException);
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

        private ExternalMediaItemDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var externalMediaItemDependencyValidationException =
                new ExternalMediaItemDependencyValidationException(
                    message: "External media item dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogError(externalMediaItemDependencyValidationException);

            return externalMediaItemDependencyValidationException;
        }

        private ExternalMediaItemDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var externalMediaItemDependencyException =
                new ExternalMediaItemDependencyException(
                    message: "External media item dependency error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError(externalMediaItemDependencyException);

            return externalMediaItemDependencyException;
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
