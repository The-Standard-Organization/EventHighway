// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    internal partial class EventArchiveV1ProcessingService
    {
        private delegate ValueTask<EventArchiveV1> ReturningEventArchiveV1Function();

        private async ValueTask<EventArchiveV1> TryCatch(
            ReturningEventArchiveV1Function returningEventArchiveV1Function)
        {
            try
            {
                return await returningEventArchiveV1Function();
            }
            catch (NullEventArchiveV1ProcessingException
                nullEventArchiveV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV1ProcessingException);
            }
            catch (InvalidEventArchiveV1ProcessingException
                invalidEventArchiveV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventArchiveV1ProcessingException);
            }
            catch (EventArchiveV1ValidationException
                eventArchiveV1ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV1ValidationException);
            }
            catch (EventArchiveV1DependencyValidationException
                eventArchiveV1DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV1DependencyValidationException);
            }
            catch (EventArchiveV1DependencyException
                eventArchiveV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV1DependencyException);
            }
            catch (EventArchiveV1ServiceException
                eventArchiveV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventArchiveV1ProcessingServiceException =
                    new FailedEventArchiveV1ProcessingServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV1ProcessingServiceException);
            }
        }

        private async ValueTask<EventArchiveV1ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1ProcessingValidationException =
                new EventArchiveV1ProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1ProcessingValidationException);

            return eventArchiveV1ProcessingValidationException;
        }

        private async ValueTask<EventArchiveV1ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventArchiveV1ProcessingDependencyValidationException =
                new EventArchiveV1ProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1ProcessingDependencyValidationException);

            return eventArchiveV1ProcessingDependencyValidationException;
        }

        private async ValueTask<EventArchiveV1ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1ProcessingDependencyException =
                new EventArchiveV1ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1ProcessingDependencyException);

            return eventArchiveV1ProcessingDependencyException;
        }

        private async ValueTask<EventArchiveV1ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1ProcessingServiceException =
                new EventArchiveV1ProcessingServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1ProcessingServiceException);

            return eventArchiveV1ProcessingServiceException;
        }
    }
}
