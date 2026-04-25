// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    internal partial class EventArchiveV1ProcessingService
    {
        private delegate ValueTask<EventArchiveV1> ReturningEventV1ArchiveFunction();

        private async ValueTask<EventArchiveV1> TryCatch(
            ReturningEventV1ArchiveFunction returningEventV1ArchiveFunction)
        {
            try
            {
                return await returningEventV1ArchiveFunction();
            }
            catch (NullEventArchiveV1ProcessingException
                nullEventV1ArchiveProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1ArchiveProcessingException);
            }
            catch (InvalidEventArchiveV1ProcessingException
                invalidEventV1ArchiveProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV1ArchiveProcessingException);
            }
            catch (EventArchiveV1ValidationException
                eventV1ArchiveValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveValidationException);
            }
            catch (EventArchiveV1DependencyValidationException
                eventV1ArchiveDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveDependencyValidationException);
            }
            catch (EventArchiveV1DependencyException
                eventV1ArchiveDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ArchiveDependencyException);
            }
            catch (EventArchiveV1ServiceException
                eventV1ArchiveServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ArchiveServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV1ArchiveProcessingServiceException =
                    new FailedEventArchiveV1ProcessingServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1ArchiveProcessingServiceException);
            }
        }

        private async ValueTask<EventArchiveV1ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveProcessingValidationException =
                new EventArchiveV1ProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveProcessingValidationException);

            return eventV1ArchiveProcessingValidationException;
        }

        private async ValueTask<EventArchiveV1ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV1ArchiveProcessingDependencyValidationException =
                new EventArchiveV1ProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveProcessingDependencyValidationException);

            return eventV1ArchiveProcessingDependencyValidationException;
        }

        private async ValueTask<EventArchiveV1ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveProcessingDependencyException =
                new EventArchiveV1ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveProcessingDependencyException);

            return eventV1ArchiveProcessingDependencyException;
        }

        private async ValueTask<EventArchiveV1ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveProcessingServiceException =
                new EventArchiveV1ProcessingServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveProcessingServiceException);

            return eventV1ArchiveProcessingServiceException;
        }
    }
}
