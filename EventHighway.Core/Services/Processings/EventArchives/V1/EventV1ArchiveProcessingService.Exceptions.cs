// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    internal partial class EventV1ArchiveProcessingService
    {
        private delegate ValueTask<EventV1Archive> ReturningEventV1ArchiveFunction();

        private async ValueTask<EventV1Archive> TryCatch(
            ReturningEventV1ArchiveFunction returningEventV1ArchiveFunction)
        {
            try
            {
                return await returningEventV1ArchiveFunction();
            }
            catch (NullEventV1ArchiveProcessingException
                nullEventV1ArchiveProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1ArchiveProcessingException);
            }
            catch (InvalidEventV1ArchiveProcessingException
                invalidEventV1ArchiveProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV1ArchiveProcessingException);
            }
            catch (EventV1ArchiveValidationException
                eventV1ArchiveValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveValidationException);
            }
            catch (EventV1ArchiveDependencyValidationException
                eventV1ArchiveDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveDependencyValidationException);
            }
        }

        private async ValueTask<EventV1ArchiveProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveProcessingValidationException =
                new EventV1ArchiveProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveProcessingValidationException);

            return eventV1ArchiveProcessingValidationException;
        }

        private async ValueTask<EventV1ArchiveProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV1ArchiveProcessingDependencyValidationException =
                new EventV1ArchiveProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveProcessingDependencyValidationException);

            return eventV1ArchiveProcessingDependencyValidationException;
        }

        private async ValueTask<EventV1ArchiveProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveProcessingDependencyException =
                new EventV1ArchiveProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveProcessingDependencyException);

            return eventV1ArchiveProcessingDependencyException;
        }

        private async ValueTask<EventV1ArchiveProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveProcessingServiceException =
                new EventV1ArchiveProcessingServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveProcessingServiceException);

            return eventV1ArchiveProcessingServiceException;
        }
    }
}
