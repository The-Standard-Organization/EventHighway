// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Coordinations.Events.V1
{
    internal partial class EventV1CoordinationService
    {
        private delegate ValueTask<EventV1> ReturningEventV1Function();
        private delegate ValueTask<EventV1> RetryingEventV1Function();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<EventV1> TryCatchWithRetryAsync(
            ReturningEventV1Function returningEventV1Function,
            RetryingEventV1Function retryEventV1Function)
        {
            try
            {
                return await returningEventV1Function();
            }
            catch (NullEventV1CoordinationException
                nullEventV1CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1CoordinationException);
            }
            catch (InvalidEventV1CoordinationException
                invalidEventV1CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV1CoordinationException);
            }
            catch (EventV1OrchestrationValidationException
                eventV1OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1OrchestrationValidationException);
            }
            catch (EventV1OrchestrationDependencyValidationException
                eventV1OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1OrchestrationDependencyValidationException);
            }
            catch (EventListenerV1OrchestrationValidationException
                eventListenerV1OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1OrchestrationValidationException);
            }
            catch (EventListenerV1OrchestrationDependencyValidationException
                eventListenerV1OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1OrchestrationDependencyValidationException);
            }
            catch (EventV1OrchestrationDependencyException
                eventV1OrchestrationDependencyException)
            {
                return await TryRetryOrThrowAsync(
                    retryEventV1Function,
                    async () => await CreateAndLogDependencyExceptionAsync(
                        eventV1OrchestrationDependencyException));
            }
            catch (EventV1OrchestrationServiceException
                eventV1OrchestrationServiceException)
            {
                return await TryRetryOrThrowAsync(
                    retryEventV1Function,
                    async () => await CreateAndLogDependencyExceptionAsync(
                        eventV1OrchestrationServiceException));
            }
            catch (EventListenerV1OrchestrationDependencyException
                eventListenerV1OrchestrationDependencyException)
            {
                return await TryRetryOrThrowAsync(
                    retryEventV1Function,
                    async () => await CreateAndLogDependencyExceptionAsync(
                        eventListenerV1OrchestrationDependencyException));
            }
            catch (EventListenerV1OrchestrationServiceException
                eventListenerV1OrchestrationServiceException)
            {
                return await TryRetryOrThrowAsync(
                    retryEventV1Function,
                    async () => await CreateAndLogDependencyExceptionAsync(
                        eventListenerV1OrchestrationServiceException));
            }
            catch (Exception exception)
            {
                var failedEventV1CoordinationServiceException =
                    new FailedEventV1CoordinationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception);

                return await TryRetryOrThrowAsync(
                    retryEventV1Function,
                    async () => await CreateAndLogServiceExceptionAsync(
                        failedEventV1CoordinationServiceException));
            }
        }

        private async ValueTask<EventV1> TryCatch(ReturningEventV1Function returningEventV1Function)
        {
            try
            {
                return await returningEventV1Function();
            }
            catch (NullEventV1CoordinationException
                nullEventV1CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1CoordinationException);
            }
            catch (InvalidEventV1CoordinationException
                invalidEventV1CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV1CoordinationException);
            }
            catch (EventV1OrchestrationValidationException
                eventV1OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1OrchestrationValidationException);
            }
            catch (EventV1OrchestrationDependencyValidationException
                eventV1OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1OrchestrationDependencyValidationException);
            }
            catch (EventListenerV1OrchestrationValidationException
                eventListenerV1OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1OrchestrationValidationException);
            }
            catch (EventListenerV1OrchestrationDependencyValidationException
                eventListenerV1OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1OrchestrationDependencyValidationException);
            }
            catch (EventV1OrchestrationDependencyException
                eventV1OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1OrchestrationDependencyException);
            }
            catch (EventV1OrchestrationServiceException
                eventV1OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1OrchestrationServiceException);
            }
            catch (EventListenerV1OrchestrationDependencyException
                eventListenerV1OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1OrchestrationDependencyException);
            }
            catch (EventListenerV1OrchestrationServiceException
                eventListenerV1OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1OrchestrationServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV1CoordinationServiceException =
                    new FailedEventV1CoordinationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1CoordinationServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (EventV1OrchestrationDependencyException
                eventV1OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1OrchestrationDependencyException);
            }
            catch (EventV1OrchestrationServiceException
                eventV1OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1OrchestrationServiceException);
            }
            catch (EventListenerV1OrchestrationDependencyException
                eventListenerV1OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1OrchestrationDependencyException);
            }
            catch (EventListenerV1OrchestrationServiceException
                eventListenerV1OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV1OrchestrationServiceException);
            }
            catch (EventV1OrchestrationValidationException
                eventV1OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1OrchestrationValidationException);
            }
            catch (EventV1OrchestrationDependencyValidationException
                eventV1OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1OrchestrationDependencyValidationException);
            }
            catch (EventListenerV1OrchestrationValidationException
                eventListenerV1OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1OrchestrationValidationException);
            }
            catch (EventListenerV1OrchestrationDependencyValidationException
                eventListenerV1OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV1OrchestrationDependencyValidationException);
            }
            catch (Exception exception)
            {
                var failedEventV1CoordinationServiceException =
                    new FailedEventV1CoordinationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1CoordinationServiceException);
            }
        }

        private async ValueTask<EventV1> TryRetryOrThrowAsync(
            RetryingEventV1Function retryEventV1Function,
            Func<ValueTask<Xeption>> createAndLogExceptionAsync)
        {
            EventV1 retriedEventV1 = await retryEventV1Function();

            if (retriedEventV1 is not null)
                return retriedEventV1;

            throw await createAndLogExceptionAsync();
        }

        private async ValueTask<EventV1CoordinationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV1CoordinationValidationException =
                new EventV1CoordinationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1CoordinationValidationException);

            return eventV1CoordinationValidationException;
        }

        private async ValueTask<EventV1CoordinationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV1CoordinationDependencyValidationException =
                new EventV1CoordinationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1CoordinationDependencyValidationException);

            return eventV1CoordinationDependencyValidationException;
        }

        private async ValueTask<EventV1CoordinationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1CoordinationDependencyException =
                new EventV1CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1CoordinationDependencyException);

            return eventV1CoordinationDependencyException;
        }

        private async ValueTask<EventV1CoordinationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV1CoordinationServiceException =
                new EventV1CoordinationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1CoordinationServiceException);

            return eventV1CoordinationServiceException;
        }
    }
}
