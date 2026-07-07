// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2
{
    internal partial class ReplayingListenerEventV2OrchestrationService
    {
        private delegate ValueTask<IEnumerable<ListenerEventV2>> ReturningListenerEventV2sFunction();
        private delegate ValueTask<ListenerEventV2> ReturningListenerEventV2Function();

        private async ValueTask<IEnumerable<ListenerEventV2>> TryCatch(
            ReturningListenerEventV2sFunction returningListenerEventV2sFunction)
        {
            try
            {
                return await returningListenerEventV2sFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutReplayingListenerEventV2OrchestrationException =
                    new TimeoutReplayingListenerEventV2OrchestrationException(
                        message: "Failed replaying listener event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutReplayingListenerEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ListenerEventV2ProcessingValidationException
                listenerEventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingValidationException);
            }
            catch (ListenerEventV2ProcessingDependencyValidationException
                listenerEventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingDependencyValidationException);
            }
            catch (ListenerEventV2ProcessingDependencyException
                listenerEventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingDependencyException);
            }
            catch (ListenerEventV2ProcessingServiceException
                listenerEventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedReplayingListenerEventV2OrchestrationServiceException =
                    new FailedReplayingListenerEventV2OrchestrationServiceException(
                        message: "Failed replaying listener event orchestration service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedReplayingListenerEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<ListenerEventV2> TryCatch(
            ReturningListenerEventV2Function returningListenerEventV2Function)
        {
            try
            {
                return await returningListenerEventV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutReplayingListenerEventV2OrchestrationException =
                    new TimeoutReplayingListenerEventV2OrchestrationException(
                        message: "Failed replaying listener event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutReplayingListenerEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullReplayingListenerEventV2OrchestrationException
                nullReplayingListenerEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullReplayingListenerEventV2OrchestrationException);
            }
            catch (EventCallV2ProcessingValidationException
                eventCallV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingValidationException);
            }
            catch (EventCallV2ProcessingDependencyValidationException
                eventCallV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingDependencyValidationException);
            }
            catch (ListenerEventV2ProcessingValidationException
                listenerEventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingValidationException);
            }
            catch (ListenerEventV2ProcessingDependencyValidationException
                listenerEventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingDependencyValidationException);
            }
            catch (EventCallV2ProcessingDependencyException
                eventCallV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventCallV2ProcessingDependencyException);
            }
            catch (EventCallV2ProcessingServiceException
                eventCallV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventCallV2ProcessingServiceException);
            }
            catch (ListenerEventV2ProcessingDependencyException
                listenerEventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingDependencyException);
            }
            catch (ListenerEventV2ProcessingServiceException
                listenerEventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedReplayingListenerEventV2OrchestrationServiceException =
                    new FailedReplayingListenerEventV2OrchestrationServiceException(
                        message: "Failed replaying listener event orchestration service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedReplayingListenerEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<ReplayingListenerEventV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var replayingListenerEventV2OrchestrationValidationException =
                new ReplayingListenerEventV2OrchestrationValidationException(
                    message: "Replaying listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                replayingListenerEventV2OrchestrationValidationException);

            return replayingListenerEventV2OrchestrationValidationException;
        }

        private async ValueTask<ReplayingListenerEventV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var replayingListenerEventV2OrchestrationServiceException =
                new ReplayingListenerEventV2OrchestrationServiceException(
                    message: "Replaying listener event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                replayingListenerEventV2OrchestrationServiceException);

            return replayingListenerEventV2OrchestrationServiceException;
        }

        private async ValueTask<ReplayingListenerEventV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var replayingListenerEventV2OrchestrationDependencyException =
                new ReplayingListenerEventV2OrchestrationDependencyException(
                    message: "Replaying listener event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                replayingListenerEventV2OrchestrationDependencyException);

            return replayingListenerEventV2OrchestrationDependencyException;
        }

        private async ValueTask<ReplayingListenerEventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var replayingListenerEventV2OrchestrationDependencyValidationException =
                new ReplayingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Replaying listener event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                replayingListenerEventV2OrchestrationDependencyValidationException);

            return replayingListenerEventV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<ReplayingListenerEventV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var replayingListenerEventV2OrchestrationDependencyException =
                new ReplayingListenerEventV2OrchestrationDependencyException(
                    message: "Replaying listener event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(replayingListenerEventV2OrchestrationDependencyException);

            return replayingListenerEventV2OrchestrationDependencyException;
        }

    }
}
