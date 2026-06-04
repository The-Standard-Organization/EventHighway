// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
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
                nullEventArchiveV1OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV1OrchestrationException);
            }
            catch (NullListenerEventArchiveV1sOrchestrationException
                nullListenerEventArchiveV1sOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventArchiveV1sOrchestrationException);
            }
            catch (EventArchiveV1ValidationException
                eventArchiveV1ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV1ValidationException);
            }
            catch (EventArchiveV1DependencyValidationException
                eventArchiveV1DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV1DependencyValidationException);
            }
            catch (ListenerEventArchiveV1ValidationException
                listenerEventArchiveV1ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventArchiveV1ValidationException);
            }
            catch (ListenerEventArchiveV1DependencyValidationException
                listenerEventArchiveV1DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventArchiveV1DependencyValidationException);
            }
            catch (EventArchiveV1DependencyException
                eventArchiveV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV1DependencyException);
            }
            catch (EventArchiveV1ServiceException
                eventArchiveV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV1ServiceException);
            }
            catch (ListenerEventArchiveV1DependencyException
                listenerListenerEventArchiveV1DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerListenerEventArchiveV1DependencyException);
            }
            catch (ListenerEventArchiveV1ServiceException
                listenerListenerEventArchiveV1ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerListenerEventArchiveV1ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventArchiveV1OrchestrationServiceException =
                    new FailedEventArchiveV1OrchestrationServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV1OrchestrationServiceException);
            }
        }

        private async ValueTask<EventArchiveV1OrchestrationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1OrchestrationValidationException =
                new EventArchiveV1OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1OrchestrationValidationException);

            return eventArchiveV1OrchestrationValidationException;
        }

        private async ValueTask<EventArchiveV1OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var eventArchiveV1OrchestrationDependencyValidationException =
                new EventArchiveV1OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1OrchestrationDependencyValidationException);

            return eventArchiveV1OrchestrationDependencyValidationException;
        }

        private async ValueTask<EventArchiveV1OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1OrchestrationDependencyException =
                new EventArchiveV1OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1OrchestrationDependencyException);

            return eventArchiveV1OrchestrationDependencyException;
        }

        private async ValueTask<EventArchiveV1OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventArchiveV1OrchestrationServiceException =
                new EventArchiveV1OrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV1OrchestrationServiceException);

            return eventArchiveV1OrchestrationServiceException;
        }
    }
}
