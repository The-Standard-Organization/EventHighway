// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Coordinations.Events.V1
{
    internal partial class EventV1CoordinationServiceV1
    {
        private delegate ValueTask ReturningNothingFunction();

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
            catch (EventV1ArchiveOrchestrationDependencyException
                eventV1ArchiveOrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ArchiveOrchestrationDependencyException);
            }
            catch (EventV1ArchiveOrchestrationServiceException
                eventV1ArchiveOrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ArchiveOrchestrationServiceException);
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
            catch (EventV1ArchiveOrchestrationValidationException
                eventV1ArchiveOrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveOrchestrationValidationException);
            }
            catch (EventV1ArchiveOrchestrationDependencyValidationException
                eventV1ArchiveOrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveOrchestrationDependencyValidationException);
            }
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
