// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventV1ArchiveOrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(
            ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullEventV1ArchiveOrchestrationException
                nullEventV1ArchiveOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1ArchiveOrchestrationException);
            }
            catch (NullListenerEventV1ArchivesOrchestrationException
                nullListenerEventV1ArchivesOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV1ArchivesOrchestrationException);
            }
            catch (EventV1ArchiveValidationException
                eventV1ArchiveValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveValidationException);
            }
            catch (EventV1ArchiveDependencyValidationException
                eventV1ArchiveDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveDependencyValidationException);
            }
            catch (ListenerEventV1ArchiveValidationException
                listenerEventV1ArchiveValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV1ArchiveValidationException);
            }
            catch (ListenerEventV1ArchiveDependencyValidationException
                listenerEventV1ArchiveDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV1ArchiveDependencyValidationException);
            }
            catch (EventV1ArchiveDependencyException
                eventV1ArchiveDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ArchiveDependencyException);
            }
            catch (EventV1ArchiveServiceException
                eventV1ArchiveServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ArchiveServiceException);
            }
            catch (ListenerEventV1ArchiveDependencyException
                listenerListenerEventV1ArchiveDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerListenerEventV1ArchiveDependencyException);
            }
            catch (ListenerEventV1ArchiveServiceException
                listenerListenerEventV1ArchiveServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerListenerEventV1ArchiveServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV1ArchiveOrchestrationServiceException =
                    new FailedEventV1ArchiveOrchestrationServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1ArchiveOrchestrationServiceException);
            }
        }

        private async ValueTask<EventV1ArchiveOrchestrationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveOrchestrationValidationException =
                new EventV1ArchiveOrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveOrchestrationValidationException);

            return eventV1ArchiveOrchestrationValidationException;
        }

        private async ValueTask<EventV1ArchiveOrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV1ArchiveOrchestrationDependencyValidationException =
                new EventV1ArchiveOrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveOrchestrationDependencyValidationException);

            return eventV1ArchiveOrchestrationDependencyValidationException;
        }

        private async ValueTask<EventV1ArchiveOrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveOrchestrationDependencyException =
                new EventV1ArchiveOrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveOrchestrationDependencyException);

            return eventV1ArchiveOrchestrationDependencyException;
        }

        private async ValueTask<EventV1ArchiveOrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveOrchestrationServiceException =
                new EventV1ArchiveOrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveOrchestrationServiceException);

            return eventV1ArchiveOrchestrationServiceException;
        }
    }
}
