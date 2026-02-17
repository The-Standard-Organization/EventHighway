// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V1
{
    internal partial class ListenerEventV1ArchiveProcessingService
    {
        private delegate ValueTask<ListenerEventV1Archive> ReturningListenerEventV1ArchiveFunction();

        private async ValueTask<ListenerEventV1Archive> TryCatch(
            ReturningListenerEventV1ArchiveFunction returningListenerEventV1ArchiveFunction)
        {
            try
            {
                return await returningListenerEventV1ArchiveFunction();
            }
            catch (NullListenerEventV1ArchiveProcessingException nullListenerEventV1ArchiveProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullListenerEventV1ArchiveProcessingException);
            }
        }

        private async ValueTask<ListenerEventV1ArchiveProcessingValidationException>
            CreateAndLogValidationExceptionAsync(
                Xeption exception)
        {
            var ListenerEventV1ArchiveProcessingValidationException =
                new ListenerEventV1ArchiveProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(ListenerEventV1ArchiveProcessingValidationException);

            return ListenerEventV1ArchiveProcessingValidationException;
        }

        private async ValueTask<ListenerEventV1ArchiveProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var ListenerEventV1ArchiveProcessingDependencyValidationException =
                new ListenerEventV1ArchiveProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(ListenerEventV1ArchiveProcessingDependencyValidationException);

            return ListenerEventV1ArchiveProcessingDependencyValidationException;
        }

        private async ValueTask<ListenerEventV1ArchiveProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var ListenerEventV1ArchiveProcessingDependencyException =
                new ListenerEventV1ArchiveProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(ListenerEventV1ArchiveProcessingDependencyException);

            return ListenerEventV1ArchiveProcessingDependencyException;
        }

        private async ValueTask<ListenerEventV1ArchiveProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var ListenerEventV1ArchiveProcessingServiceException =
                new ListenerEventV1ArchiveProcessingServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(ListenerEventV1ArchiveProcessingServiceException);

            return ListenerEventV1ArchiveProcessingServiceException;
        }
    }
}
