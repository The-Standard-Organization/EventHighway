// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V1
{
    internal partial class EventArchiveV1OrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(
            ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullEventArchiveV1OrchestrationException
                nullEventV1ArchiveOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventV1ArchiveOrchestrationException);
            }
            catch (NullListenerEventArchiveV1sOrchestrationException
                nullListenerEventV1ArchivesOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV1ArchivesOrchestrationException);
            }
            catch (EventArchiveV1ValidationException
                eventV1ArchiveValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveValidationException);
            }
            catch (EventArchiveV1DependencyValidationException
                eventV1ArchiveDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV1ArchiveDependencyValidationException);
            }
            catch (ListenerEventArchiveV1ValidationException
                listenerEventV1ArchiveValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV1ArchiveValidationException);
            }
            catch (ListenerEventArchiveV1DependencyValidationException
                listenerEventV1ArchiveDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV1ArchiveDependencyValidationException);
            }
            catch (EventArchiveV1DependencyException
                eventV1ArchiveDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ArchiveDependencyException);
            }
            catch (EventArchiveV1ServiceException
                eventV1ArchiveServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV1ArchiveServiceException);
            }
            catch (ListenerEventArchiveV1DependencyException
                listenerListenerEventV1ArchiveDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerListenerEventV1ArchiveDependencyException);
            }
            catch (ListenerEventArchiveV1ServiceException
                listenerListenerEventV1ArchiveServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerListenerEventV1ArchiveServiceException);
            }
            catch (Exception exception)
            {
                var failedEventV1ArchiveOrchestrationServiceException =
                    new FailedEventArchiveV1OrchestrationServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventV1ArchiveOrchestrationServiceException);
            }
        }

        private async ValueTask<EventArchiveV1OrchestrationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveOrchestrationValidationException =
                new EventArchiveV1OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveOrchestrationValidationException);

            return eventV1ArchiveOrchestrationValidationException;
        }

        private async ValueTask<EventArchiveV1OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventV1ArchiveOrchestrationDependencyValidationException =
                new EventArchiveV1OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveOrchestrationDependencyValidationException);

            return eventV1ArchiveOrchestrationDependencyValidationException;
        }

        private async ValueTask<EventArchiveV1OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveOrchestrationDependencyException =
                new EventArchiveV1OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveOrchestrationDependencyException);

            return eventV1ArchiveOrchestrationDependencyException;
        }

        private async ValueTask<EventArchiveV1OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventV1ArchiveOrchestrationServiceException =
                new EventArchiveV1OrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventV1ArchiveOrchestrationServiceException);

            return eventV1ArchiveOrchestrationServiceException;
        }
    }
}
