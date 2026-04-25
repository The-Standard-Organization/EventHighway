// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.EventListeners.V1
{
    internal partial class EventListenerV1OrchestrationService
    {
        private delegate ValueTask<EventListenerV1> ReturningEventListenerV1Function();
        private delegate ValueTask<IQueryable<EventListenerV1>> ReturningEventListenerV1sFunction();
        private delegate ValueTask<ListenerEventV1> ReturningListenerEventV1Function();
        private delegate ValueTask<IQueryable<ListenerEventV1>> ReturningListenerEventV1sFunction();

        private async ValueTask<EventListenerV1> TryCatch(
            ReturningEventListenerV1Function returningEventListenerV1Function)
        {
            try
            {
                return await returningEventListenerV1Function();
            }
            catch (InvalidEventListenerV1OrchestrationException
                invalidEventListenerV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV1OrchestrationException);
            }
            catch (NullEventListenerV1OrchestrationException
                nullEventListenerV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventListenerV1OrchestrationException);
            }
            catch (EventListenerV1ProcessingValidationException
                eventListenerV1ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1ProcessingValidationException);
            }
            catch (EventListenerV1ProcessingDependencyValidationException
                eventListenerV1ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1ProcessingDependencyValidationException);
            }
            catch (EventListenerV1ProcessingDependencyException
                eventListenerV1ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1ProcessingDependencyException);
            }
            catch (EventListenerV1ProcessingServiceException
                eventListenerV1ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV1OrchestrationServiceException =
                    new FailedEventListenerV1OrchestrationServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV1OrchestrationServiceException);
            }
        }

        private async ValueTask<IQueryable<EventListenerV1>> TryCatch(
            ReturningEventListenerV1sFunction returningEventListenerV1sFunction)
        {
            try
            {
                return await returningEventListenerV1sFunction();
            }
            catch (InvalidEventListenerV1OrchestrationException
                invalidEventListenerV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV1OrchestrationException);
            }
            catch (EventListenerV1ProcessingValidationException
                eventListenerV1ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1ProcessingValidationException);
            }
            catch (EventListenerV1ProcessingDependencyException
                eventListenerV1ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1ProcessingDependencyException);
            }
            catch (EventListenerV1ProcessingServiceException
                eventListenerV1ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV1OrchestrationServiceException =
                    new FailedEventListenerV1OrchestrationServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV1OrchestrationServiceException);
            }
        }

        private async ValueTask<ListenerEventV1> TryCatch(
            ReturningListenerEventV1Function returningListenerEventV1Function)
        {
            try
            {
                return await returningListenerEventV1Function();
            }
            catch (NullListenerEventV1OrchestrationException
                nullListenerEventV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV1OrchestrationException);
            }
            catch (InvalidEventListenerV1OrchestrationException
                invalidEventListenerV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventListenerV1OrchestrationException);
            }
            catch (ListenerEventV1ProcessingValidationException
                listenerEventV1ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV1ProcessingValidationException);
            }
            catch (ListenerEventV1ProcessingDependencyValidationException
                listenerEventV1ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV1ProcessingDependencyValidationException);
            }
            catch (ListenerEventV1ProcessingDependencyException
                listenerEventV1ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV1ProcessingDependencyException);
            }
            catch (ListenerEventV1ProcessingServiceException
                listenerEventV1ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV1ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV1OrchestrationServiceException =
                    new FailedEventListenerV1OrchestrationServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV1OrchestrationServiceException);
            }
        }

        private async ValueTask<IQueryable<ListenerEventV1>> TryCatch(
            ReturningListenerEventV1sFunction returningListenerEventV1sFunction)
        {
            try
            {
                return await returningListenerEventV1sFunction();
            }
            catch (ListenerEventV1ProcessingDependencyException
                listenerEventV1ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV1ProcessingDependencyException);
            }
            catch (ListenerEventV1ProcessingServiceException
                listenerEventV1ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV1ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventListenerV1OrchestrationServiceException =
                    new FailedEventListenerV1OrchestrationServiceException(
                        message: "Failed event listener service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventListenerV1OrchestrationServiceException);
            }
        }

        private async ValueTask<EventListenerV1OrchestrationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventListenerV1OrchestrationValidationException =
                new EventListenerV1OrchestrationValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV1OrchestrationValidationException);

            return eventListenerV1OrchestrationValidationException;
        }

        private async ValueTask<EventListenerV1OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventListenerV1OrchestrationDependencyValidationException =
                new EventListenerV1OrchestrationDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventListenerV1OrchestrationDependencyValidationException);

            return eventListenerV1OrchestrationDependencyValidationException;
        }

        private async ValueTask<EventListenerV1OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventListenerV1OrchestrationDependencyException =
                new EventListenerV1OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventListenerV1OrchestrationDependencyException);

            return eventListenerV1OrchestrationDependencyException;
        }

        private async ValueTask<EventListenerV1OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventListenerV1OrchestrationServiceException =
                new EventListenerV1OrchestrationServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventListenerV1OrchestrationServiceException);

            return eventListenerV1OrchestrationServiceException;
        }
    }
}
