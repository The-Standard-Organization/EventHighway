// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V1;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventCalls.V1.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.Events.V1
{
    internal partial class EventV1OrchestrationService
    {
        private delegate ValueTask<EventV1> ReturningEventV1Function();
        private delegate ValueTask<IQueryable<EventV1>> ReturningEventV1sFunction();
        private delegate ValueTask<EventCallV1> ReturningEventCallV1Function();

        private async ValueTask<EventV1> TryCatch(ReturningEventV1Function returningEventV1Function)
        {
            try
            {
                return await returningEventV1Function();
            }
            catch (InvalidEventV1OrchestrationException
                invalidEventV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventV1OrchestrationException);
            }
            catch (NullEventV1OrchestrationException
                nullEventV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1OrchestrationException);
            }
            catch (NotFoundEventAddressV1OrchestrationException
                notFoundEventAddressV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    notFoundEventAddressV1OrchestrationException);
            }
            catch (EventV1ProcessingValidationException
                eventV1ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ProcessingValidationException);
            }
            catch (EventV1ProcessingDependencyValidationException
                eventV1ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ProcessingDependencyValidationException);
            }
            catch (EventAddressV1ProcessingValidationException
                eventAddressV1ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV1ProcessingValidationException);
            }
            catch (EventAddressV1ProcessingDependencyValidationException
                eventAddressV1ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventAddressV1ProcessingDependencyValidationException);
            }
            catch (EventV1ProcessingDependencyException
                eventV1ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ProcessingDependencyException);
            }
            catch (EventV1ProcessingServiceException
                eventV1ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ProcessingServiceException);
            }
            catch (EventAddressV1ProcessingDependencyException
                eventAddressV1ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV1ProcessingDependencyException);
            }
            catch (EventAddressV1ProcessingServiceException
                eventAddressV1ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventAddressV1ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV1OrchestrationServiceException =
                    new FailedEventV1OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1OrchestrationServiceException);
            }
        }

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
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventV1OrchestrationServiceException);
            }
        }

        private async ValueTask<EventCallV1> TryCatch(
            ReturningEventCallV1Function returningEventCallV1Function)
        {
            try
            {
                return await returningEventCallV1Function();
            }
            catch (NullEventCallV1OrchestrationException
                nullEventCallV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventCallV1OrchestrationException);
            }
            catch (EventCallV1ProcessingValidationException
                eventCallV1ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV1ProcessingValidationException);
            }
            catch (EventCallV1ProcessingDependencyValidationException
                eventCallV1ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV1ProcessingDependencyValidationException);
            }
            catch (EventCallV1ProcessingDependencyException
                eventCallV1ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventCallV1ProcessingDependencyException);
            }
            catch (EventCallV1ProcessingServiceException
                eventCallV1ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventCallV1ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV1OrchestrationServiceException =
                    new FailedEventV1OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1OrchestrationServiceException);
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

        private async ValueTask<EventV1OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV1OrchestrationDependencyValidationException =
                new EventV1OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1OrchestrationDependencyValidationException);

            return eventV1OrchestrationDependencyValidationException;
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
