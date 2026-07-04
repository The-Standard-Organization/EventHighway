// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.RetryingListenerEvents.V2
{
    internal partial class RetryingListenerEventV2OrchestrationService
    {
        private delegate ValueTask<ListenerEventV2> ReturningListenerEventV2Function();

        private async ValueTask<ListenerEventV2> TryCatch(
            ReturningListenerEventV2Function returningListenerEventV2Function)
        {
            try
            {
                return await returningListenerEventV2Function();
            }
            catch (NullRetryingListenerEventV2OrchestrationException
                nullRetryingListenerEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullRetryingListenerEventV2OrchestrationException);
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
        }

        private async ValueTask<RetryingListenerEventV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var retryingListenerEventV2OrchestrationValidationException =
                new RetryingListenerEventV2OrchestrationValidationException(
                    message: "Retrying listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                retryingListenerEventV2OrchestrationValidationException);

            return retryingListenerEventV2OrchestrationValidationException;
        }

        private async ValueTask<RetryingListenerEventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var retryingListenerEventV2OrchestrationDependencyValidationException =
                new RetryingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Retrying listener event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                retryingListenerEventV2OrchestrationDependencyValidationException);

            return retryingListenerEventV2OrchestrationDependencyValidationException;
        }
    }
}
