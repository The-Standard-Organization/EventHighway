// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.Events.V1
{
    internal partial class EventV1OrchestrationServiceV1
    {
        private delegate ValueTask<IQueryable<EventV1>> ReturningEventV1sFunction();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<IQueryable<EventV1>> TryCatch(ReturningEventV1sFunction returningEventV1sFunction)
        {
            try
            {
                return await returningEventV1sFunction();
            }
            catch (EventV1ProcessingDependencyException eventV1ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV1ProcessingDependencyException);
            }
            catch (EventV1ProcessingServiceException eventV1ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV1ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV1OrchestrationServiceException =
                    new FailedEventV1OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedEventV1OrchestrationServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullEventV1OrchestrationException nullEventV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventV1OrchestrationException);
            }
        }

        private async ValueTask<EventV1OrchestrationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV1OrchestrationValidationException =
                new EventV1OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1OrchestrationValidationException);

            return eventV1OrchestrationValidationException;
        }

        private async ValueTask<EventV1OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1OrchestrationDependencyException =
                new EventV1OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1OrchestrationDependencyException);

            return eventV1OrchestrationDependencyException;
        }

        private async ValueTask<EventV1OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV1OrchestrationServiceException =
                new EventV1OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1OrchestrationServiceException);

            return eventV1OrchestrationServiceException;
        }
    }
}
