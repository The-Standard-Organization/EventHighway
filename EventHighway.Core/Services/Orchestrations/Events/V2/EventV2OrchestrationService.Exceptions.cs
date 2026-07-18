// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.Events.V2
{
    internal partial class EventV2OrchestrationService
    {
        private delegate ValueTask<EventV2> ReturningEventV2Function();
        private delegate ValueTask<IQueryable<EventV2>> ReturningEventV2sFunction();
        private delegate ValueTask<bool> ReturningBoolFunction();
        private delegate ValueTask<int> ReturningIntFunction();

        private async ValueTask<EventV2> TryCatch(ReturningEventV2Function returningEventV2Function)
        {
            try
            {
                return await returningEventV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutEventV2OrchestrationException =
                    new TimeoutEventV2OrchestrationException(
                        message: "Failed event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidEventV2OrchestrationException
                invalidEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV2OrchestrationException);
            }
            catch (NullEventV2OrchestrationException
                nullEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV2OrchestrationException);
            }
            catch (NotFoundEventAddressV2OrchestrationException
                notFoundEventAddressV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundEventAddressV2OrchestrationException);
            }
            catch (EventV2ProcessingValidationException
                eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException
                eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (EventAddressV2ProcessingValidationException
                eventAddressV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV2ProcessingValidationException);
            }
            catch (EventAddressV2ProcessingDependencyValidationException
                eventAddressV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException
                eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException
                eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2ProcessingServiceException);
            }
            catch (EventAddressV2ProcessingDependencyException
                eventAddressV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV2ProcessingDependencyException);
            }
            catch (EventAddressV2ProcessingServiceException
                eventAddressV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2OrchestrationServiceException =
                    new FailedEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<IQueryable<EventV2>> TryCatch(
            ReturningEventV2sFunction returningEventV2sFunction)
        {
            try
            {
                return await returningEventV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutEventV2OrchestrationException =
                    new TimeoutEventV2OrchestrationException(
                        message: "Failed event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventV2ProcessingValidationException
                eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException
                eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (EventAddressV2ProcessingValidationException
                eventAddressV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV2ProcessingValidationException);
            }
            catch (EventAddressV2ProcessingDependencyValidationException
                eventAddressV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingServiceException);
            }
            catch (EventAddressV2ProcessingDependencyException
                eventAddressV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventAddressV2ProcessingDependencyException);
            }
            catch (EventAddressV2ProcessingServiceException
                eventAddressV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventAddressV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2OrchestrationServiceException =
                    new FailedEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<int> TryCatch(ReturningIntFunction returningIntFunction)
        {
            try
            {
                return await returningIntFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutEventV2OrchestrationException =
                    new TimeoutEventV2OrchestrationException(
                        message: "Failed event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventV2OrchestrationException
                nullEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV2OrchestrationException);
            }
            catch (EventV2ProcessingValidationException
                eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException
                eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException
                eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException
                eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2OrchestrationServiceException =
                    new FailedEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<bool> TryCatch(ReturningBoolFunction returningBoolFunction)
        {
            try
            {
                return await returningBoolFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutEventV2OrchestrationException =
                    new TimeoutEventV2OrchestrationException(
                        message: "Failed event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventV2OrchestrationException
                nullEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV2OrchestrationException);
            }
            catch (EventV2ProcessingValidationException
                eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException
                eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (EventAddressV2ProcessingValidationException
                eventAddressV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV2ProcessingValidationException);
            }
            catch (EventAddressV2ProcessingDependencyValidationException
                eventAddressV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException
                eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException
                eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2ProcessingServiceException);
            }
            catch (EventAddressV2ProcessingDependencyException
                eventAddressV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV2ProcessingDependencyException);
            }
            catch (EventAddressV2ProcessingServiceException
                eventAddressV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2OrchestrationServiceException =
                    new FailedEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<EventV2OrchestrationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV2OrchestrationValidationException =
                new EventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2OrchestrationValidationException);

            return eventV2OrchestrationValidationException;
        }

        private async ValueTask<EventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV2OrchestrationDependencyValidationException =
                new EventV2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV2OrchestrationDependencyValidationException);

            return eventV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<EventV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventV2OrchestrationDependencyException =
                new EventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2OrchestrationDependencyException);

            return eventV2OrchestrationDependencyException;
        }

        private async ValueTask<EventV2OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV2OrchestrationDependencyException =
                new EventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV2OrchestrationDependencyException);

            return eventV2OrchestrationDependencyException;
        }

        private async ValueTask<EventV2OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV2OrchestrationServiceException =
                new EventV2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2OrchestrationServiceException);

            return eventV2OrchestrationServiceException;
        }
    }
}
