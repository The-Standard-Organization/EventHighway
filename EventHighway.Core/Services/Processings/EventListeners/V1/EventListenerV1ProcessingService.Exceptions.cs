// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventListeners.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.EventListeners.V1
{
    internal partial class EventListenerV1ProcessingService
    {
        private delegate ValueTask<EventListenerV1> ReturningEventListenerV1Function();
        private delegate ValueTask<IQueryable<EventListenerV1>> ReturningEventListenerV1sFunction();

        private async ValueTask<EventListenerV1> TryCatch(
            ReturningEventListenerV1Function returningEventListenerV1Function)
        {
            try
            {
                return await returningEventListenerV1Function();
            }
            catch (NullEventListenerV1ProcessingException
                nullEventListenerV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventListenerV1ProcessingException);
            }
            catch (InvalidEventListenerV1ProcessingException
                invalidEventListenerV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV1ProcessingException);
            }
            catch (EventListenerV1ValidationException
                eventListenerV1ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1ValidationException);
            }
            catch (EventListenerV1DependencyValidationException
                eventListenerV1DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1DependencyValidationException);
            }
            catch (EventListenerV1DependencyException
                eventListenerV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1DependencyException);
            }
            catch (EventListenerV1ServiceException
                eventListenerV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV1ProcessingServiceException =
                    new FailedEventListenerV1ProcessingServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV1ProcessingServiceException);
            }
        }

        private async ValueTask<IQueryable<EventListenerV1>> TryCatch(
            ReturningEventListenerV1sFunction returningEventListenerV1sFunction)
        {
            try
            {
                return await returningEventListenerV1sFunction();
            }
            catch (InvalidEventListenerV1ProcessingException
                invalidEventListenerV1ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV1ProcessingException);
            }
            catch (EventListenerV1DependencyException
                eventListenerV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1DependencyException);
            }
            catch (EventListenerV1ServiceException
                eventListenerV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV1ProcessingServiceException =
                    new FailedEventListenerV1ProcessingServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV1ProcessingServiceException);
            }
        }

        private async ValueTask<EventListenerV1ProcessingValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventListenerV1ProcessingValidationException =
                new EventListenerV1ProcessingValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV1ProcessingValidationException);

            return eventListenerV1ProcessingValidationException;
        }

        private async ValueTask<EventListenerV1ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventListenerV1ProcessingDependencyValidationException =
                new EventListenerV1ProcessingDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventListenerV1ProcessingDependencyValidationException);

            return eventListenerV1ProcessingDependencyValidationException;
        }

        private async ValueTask<EventListenerV1ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerV1ProcessingDependencyException =
                new EventListenerV1ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventListenerV1ProcessingDependencyException);

            return eventListenerV1ProcessingDependencyException;
        }

        private async ValueTask<EventListenerV1ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventListenerV1ProcessingServiceException =
                new EventListenerV1ProcessingServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV1ProcessingServiceException);

            return eventListenerV1ProcessingServiceException;
        }
    }
}
