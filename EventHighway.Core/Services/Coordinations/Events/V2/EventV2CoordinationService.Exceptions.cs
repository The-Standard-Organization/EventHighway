// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventFirings.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Coordinations.Events.V2
{
    internal partial class EventV2CoordinationService
    {
        private delegate ValueTask<EventV2> ReturningEventV2Function();
        private delegate ValueTask<IQueryable<EventV2>> ReturningEventV2sFunction();
        private delegate ValueTask ReturningNothingFunction();

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

                var timeoutEventV2CoordinationException =
                    new TimeoutEventV2CoordinationException(
                        message: "Failed event coordination timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventV2CoordinationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullEventV2QueryCoordinationException nullEventV2QueryCoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV2QueryCoordinationException);
            }
            catch (InvalidEventV2QueryCoordinationException invalidEventV2QueryCoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV2QueryCoordinationException);
            }
            catch (EventV2OrchestrationValidationException eventV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2OrchestrationValidationException);
            }
            catch (EventV2OrchestrationDependencyValidationException
                eventV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2OrchestrationDependencyValidationException);
            }
            catch (EventFiringV2OrchestrationValidationException
                eventFiringV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventFiringV2OrchestrationValidationException);
            }
            catch (EventFiringV2OrchestrationDependencyValidationException
                eventFiringV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventFiringV2OrchestrationDependencyValidationException);
            }
            catch (EventParticipantV2OrchestrationValidationException
                eventParticipantV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2OrchestrationValidationException);
            }
            catch (EventParticipantV2OrchestrationDependencyValidationException
                eventParticipantV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2OrchestrationDependencyValidationException);
            }
            catch (EventV2OrchestrationDependencyException eventV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2OrchestrationDependencyException);
            }
            catch (EventV2OrchestrationServiceException eventV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2OrchestrationServiceException);
            }
            catch (EventFiringV2OrchestrationDependencyException
                eventFiringV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventFiringV2OrchestrationDependencyException);
            }
            catch (EventFiringV2OrchestrationServiceException
                eventFiringV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventFiringV2OrchestrationServiceException);
            }
            catch (EventParticipantV2OrchestrationDependencyException
                eventParticipantV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2OrchestrationDependencyException);
            }
            catch (EventParticipantV2OrchestrationServiceException
                eventParticipantV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2OrchestrationServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2CoordinationServiceException =
                    new FailedEventV2CoordinationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV2CoordinationServiceException);
            }
        }

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

                var timeoutEventV2CoordinationException =
                    new TimeoutEventV2CoordinationException(
                        message: "Failed event coordination timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventV2CoordinationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (LoopDetectedEventV2CoordinationException loopDetectedEventV2CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(loopDetectedEventV2CoordinationException);
            }
            catch (NullEventV2CoordinationException nullEventV2CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV2CoordinationException);
            }
            catch (InvalidEventV2CoordinationException invalidEventV2CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV2CoordinationException);
            }
            catch (EventV2OrchestrationValidationException eventV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2OrchestrationValidationException);
            }
            catch (EventV2OrchestrationDependencyValidationException
                eventV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2OrchestrationDependencyValidationException);
            }
            catch (EventFiringV2OrchestrationValidationException
                eventFiringV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventFiringV2OrchestrationValidationException);
            }
            catch (EventFiringV2OrchestrationDependencyValidationException
                eventFiringV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventFiringV2OrchestrationDependencyValidationException);
            }
            catch (EventParticipantV2OrchestrationValidationException
                eventParticipantV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2OrchestrationValidationException);
            }
            catch (EventParticipantV2OrchestrationDependencyValidationException
                eventParticipantV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2OrchestrationDependencyValidationException);
            }
            catch (EventV2OrchestrationDependencyException eventV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2OrchestrationDependencyException);
            }
            catch (EventV2OrchestrationServiceException eventV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2OrchestrationServiceException);
            }
            catch (EventFiringV2OrchestrationDependencyException
                eventFiringV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventFiringV2OrchestrationDependencyException);
            }
            catch (EventFiringV2OrchestrationServiceException
                eventFiringV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventFiringV2OrchestrationServiceException);
            }
            catch (EventParticipantV2OrchestrationDependencyException
                eventParticipantV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2OrchestrationDependencyException);
            }
            catch (EventParticipantV2OrchestrationServiceException
                eventParticipantV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2OrchestrationServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2CoordinationServiceException =
                    new FailedEventV2CoordinationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV2CoordinationServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutEventV2CoordinationException =
                    new TimeoutEventV2CoordinationException(
                        message: "Failed event coordination timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventV2CoordinationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventV2OrchestrationValidationException eventV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2OrchestrationValidationException);
            }
            catch (EventV2OrchestrationDependencyValidationException
                eventV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2OrchestrationDependencyValidationException);
            }
            catch (EventFiringV2OrchestrationValidationException
                eventFiringV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventFiringV2OrchestrationValidationException);
            }
            catch (EventFiringV2OrchestrationDependencyValidationException
                eventFiringV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventFiringV2OrchestrationDependencyValidationException);
            }
            catch (EventParticipantV2OrchestrationValidationException
                eventParticipantV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2OrchestrationValidationException);
            }
            catch (EventParticipantV2OrchestrationDependencyValidationException
                eventParticipantV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2OrchestrationDependencyValidationException);
            }
            catch (EventV2OrchestrationDependencyException eventV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2OrchestrationDependencyException);
            }
            catch (EventV2OrchestrationServiceException eventV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2OrchestrationServiceException);
            }
            catch (EventFiringV2OrchestrationDependencyException
                eventFiringV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventFiringV2OrchestrationDependencyException);
            }
            catch (EventFiringV2OrchestrationServiceException
                eventFiringV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventFiringV2OrchestrationServiceException);
            }
            catch (EventParticipantV2OrchestrationDependencyException
                eventParticipantV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2OrchestrationDependencyException);
            }
            catch (EventParticipantV2OrchestrationServiceException
                eventParticipantV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2OrchestrationServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV2CoordinationServiceException =
                    new FailedEventV2CoordinationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV2CoordinationServiceException);
            }
        }

        private async ValueTask<EventV2CoordinationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventV2CoordinationValidationException =
                new EventV2CoordinationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2CoordinationValidationException);

            return eventV2CoordinationValidationException;
        }

        private async ValueTask<EventV2CoordinationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventV2CoordinationDependencyValidationException =
                new EventV2CoordinationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV2CoordinationDependencyValidationException);

            return eventV2CoordinationDependencyValidationException;
        }

        private async ValueTask<EventV2CoordinationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventV2CoordinationDependencyException =
                new EventV2CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                eventV2CoordinationDependencyException);

            return eventV2CoordinationDependencyException;
        }

        private async ValueTask<EventV2CoordinationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventV2CoordinationDependencyException =
                new EventV2CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV2CoordinationDependencyException);

            return eventV2CoordinationDependencyException;
        }

        private async ValueTask<EventV2CoordinationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventV2CoordinationServiceException =
                new EventV2CoordinationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV2CoordinationServiceException);

            return eventV2CoordinationServiceException;
        }
    }
}
