// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.EventParticipants.V2
{
    internal partial class EventParticipantV2OrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

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
                    new TimeoutException("The dependency operation timed out.");

                var timeoutEventParticipantV2OrchestrationException =
                    new TimeoutEventParticipantV2OrchestrationException(
                        message: "Failed event participant orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutEventParticipantV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidEventParticipantV2OrchestrationException
                invalidEventParticipantV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventParticipantV2OrchestrationException);
            }
            catch (EventParticipantV2ValidationException
                eventParticipantV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2ValidationException);
            }
            catch (EventParticipantV2DependencyValidationException
                eventParticipantV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantV2DependencyValidationException);
            }
            catch (EventParticipantSecretV2ValidationException
                eventParticipantSecretV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantSecretV2ValidationException);
            }
            catch (EventParticipantSecretV2DependencyValidationException
                eventParticipantSecretV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventParticipantSecretV2DependencyValidationException);
            }
            catch (EventParticipantV2DependencyException
                eventParticipantV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2DependencyException);
            }
            catch (EventParticipantV2ServiceException
                eventParticipantV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantV2ServiceException);
            }
            catch (EventParticipantSecretV2DependencyException
                eventParticipantSecretV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantSecretV2DependencyException);
            }
            catch (EventParticipantSecretV2ServiceException
                eventParticipantSecretV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventParticipantSecretV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventParticipantV2OrchestrationServiceException =
                    new FailedEventParticipantV2OrchestrationServiceException(
                        message: "Failed event participant service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventParticipantV2OrchestrationServiceException);
            }
        }

        private async ValueTask<EventParticipantV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantV2OrchestrationValidationException =
                new EventParticipantV2OrchestrationValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                eventParticipantV2OrchestrationValidationException);

            return eventParticipantV2OrchestrationValidationException;
        }

        private async ValueTask<EventParticipantV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantV2OrchestrationDependencyValidationException =
                new EventParticipantV2OrchestrationDependencyValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                eventParticipantV2OrchestrationDependencyValidationException);

            return eventParticipantV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<EventParticipantV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantV2OrchestrationDependencyException =
                new EventParticipantV2OrchestrationDependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                eventParticipantV2OrchestrationDependencyException);

            return eventParticipantV2OrchestrationDependencyException;
        }

        private async ValueTask<EventParticipantV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventParticipantV2OrchestrationServiceException =
                new EventParticipantV2OrchestrationServiceException(
                    message: "Event participant service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                eventParticipantV2OrchestrationServiceException);

            return eventParticipantV2OrchestrationServiceException;
        }

        private async ValueTask<EventParticipantV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var eventParticipantV2OrchestrationDependencyException =
                new EventParticipantV2OrchestrationDependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2OrchestrationDependencyException);

            return eventParticipantV2OrchestrationDependencyException;
        }

    }
}
