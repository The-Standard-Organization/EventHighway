// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V2
{
    internal partial class EventArchiveV2OrchestrationService
    {
        private delegate ValueTask<IQueryable<EventArchiveV2>> ReturningEventArchiveV2sFunction();
        private delegate ValueTask<IQueryable<ListenerEventArchiveV2>> ReturningListenerEventArchiveV2sFunction();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<IQueryable<EventArchiveV2>> TryCatch(
            ReturningEventArchiveV2sFunction returningEventArchiveV2sFunction)
        {
            try
            {
                return await returningEventArchiveV2sFunction();
            }
            catch (EventArchiveV2DependencyException eventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventArchiveV2DependencyException);
            }
            catch (EventArchiveV2ServiceException eventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventArchiveV2OrchestrationServiceException =
                    new FailedEventArchiveV2OrchestrationServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventArchiveV2OrchestrationServiceException);
            }
        }

        private async ValueTask<IQueryable<ListenerEventArchiveV2>> TryCatch(
            ReturningListenerEventArchiveV2sFunction returningListenerEventArchiveV2sFunction)
        {
            try
            {
                return await returningListenerEventArchiveV2sFunction();
            }
            catch (ListenerEventArchiveV2DependencyException listenerEventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventArchiveV2DependencyException);
            }
            catch (ListenerEventArchiveV2ServiceException listenerEventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventArchiveV2OrchestrationServiceException =
                    new FailedEventArchiveV2OrchestrationServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventArchiveV2OrchestrationServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullEventArchiveV2OrchestrationException
                nullEventArchiveV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventArchiveV2OrchestrationException);
            }
            catch (NullListenerEventArchiveV2sOrchestrationException
                nullListenerEventArchiveV2sOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventArchiveV2sOrchestrationException);
            }
            catch (InvalidEventArchiveV2OrchestrationException
                invalidEventArchiveV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidEventArchiveV2OrchestrationException);
            }
            catch (EventArchiveV2ValidationException
                eventArchiveV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2ValidationException);
            }
            catch (EventArchiveV2DependencyValidationException
                eventArchiveV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2DependencyValidationException);
            }
            catch (ListenerEventArchiveV2ValidationException
                listenerEventArchiveV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventArchiveV2ValidationException);
            }
            catch (ListenerEventArchiveV2DependencyValidationException
                listenerEventArchiveV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventArchiveV2DependencyValidationException);
            }
            catch (EventArchiveV2DependencyException
                eventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2DependencyException);
            }
            catch (EventArchiveV2ServiceException
                eventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2ServiceException);
            }
            catch (ListenerEventArchiveV2DependencyException
                listenerEventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventArchiveV2DependencyException);
            }
            catch (ListenerEventArchiveV2ServiceException
                listenerEventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedEventArchiveV2OrchestrationServiceException =
                    new FailedEventArchiveV2OrchestrationServiceException(
                        message: "Failed event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventArchiveV2OrchestrationServiceException);
            }
        }

        private async ValueTask<EventArchiveV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventArchiveV2OrchestrationValidationException =
                new EventArchiveV2OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2OrchestrationValidationException);

            return eventArchiveV2OrchestrationValidationException;
        }

        private async ValueTask<EventArchiveV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventArchiveV2OrchestrationDependencyValidationException =
                new EventArchiveV2OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                eventArchiveV2OrchestrationDependencyValidationException);

            return eventArchiveV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<EventArchiveV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2OrchestrationDependencyException);

            return eventArchiveV2OrchestrationDependencyException;
        }

        private async ValueTask<EventArchiveV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventArchiveV2OrchestrationServiceException =
                new EventArchiveV2OrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventArchiveV2OrchestrationServiceException);

            return eventArchiveV2OrchestrationServiceException;
        }
    }
}
