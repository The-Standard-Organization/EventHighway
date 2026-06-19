// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2OrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<IEnumerable<EventV2>> ReturningEventV2EnumerableFunction();
        private delegate ValueTask<IEnumerable<ListenerEventV2>> ReturningListenerEventV2EnumerableFunction();

        private async ValueTask<IEnumerable<EventV2>> TryCatch(
            ReturningEventV2EnumerableFunction returningEventV2EnumerableFunction)
        {
            try
            {
                return await returningEventV2EnumerableFunction();
            }
            catch (InvalidArchivingEventV2OrchestrationException invalidArchivingEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArchivingEventV2OrchestrationException);
            }
            catch (EventV2ProcessingValidationException eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingServiceException);
            }
            catch (ListenerEventV2ProcessingValidationException listenerEventV2ProcessingValidationException)
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
            catch (ListenerEventV2ProcessingDependencyException listenerEventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2ProcessingDependencyException);
            }
            catch (ListenerEventV2ProcessingServiceException listenerEventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedArchivingEventV2OrchestrationServiceException =
                    new FailedArchivingEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedArchivingEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<IEnumerable<ListenerEventV2>> TryCatch(
            ReturningListenerEventV2EnumerableFunction returningListenerEventV2EnumerableFunction)
        {
            try
            {
                return await returningListenerEventV2EnumerableFunction();
            }
            catch (InvalidArchivingEventV2OrchestrationException invalidArchivingEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArchivingEventV2OrchestrationException);
            }
            catch (EventV2ProcessingValidationException eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingServiceException);
            }
            catch (ListenerEventV2ProcessingValidationException listenerEventV2ProcessingValidationException)
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
            catch (ListenerEventV2ProcessingDependencyException listenerEventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2ProcessingDependencyException);
            }
            catch (ListenerEventV2ProcessingServiceException listenerEventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedArchivingEventV2OrchestrationServiceException =
                    new FailedArchivingEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedArchivingEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullArchivingEventV2sOrchestrationException nullArchivingEventV2sOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullArchivingEventV2sOrchestrationException);
            }
            catch (NullArchivingEventV2OrchestrationException nullArchivingEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullArchivingEventV2OrchestrationException);
            }
            catch (EventV2ProcessingValidationException eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingServiceException);
            }
            catch (ListenerEventV2ProcessingValidationException listenerEventV2ProcessingValidationException)
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
            catch (ListenerEventV2ProcessingDependencyException listenerEventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2ProcessingDependencyException);
            }
            catch (ListenerEventV2ProcessingServiceException listenerEventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedArchivingEventV2OrchestrationServiceException =
                    new FailedArchivingEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedArchivingEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<ArchivingEventV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var archivingEventV2OrchestrationValidationException =
                new ArchivingEventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationValidationException);

            return archivingEventV2OrchestrationValidationException;
        }

        private async ValueTask<ArchivingEventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var archivingEventV2OrchestrationDependencyValidationException =
                new ArchivingEventV2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationDependencyValidationException);

            return archivingEventV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<ArchivingEventV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var archivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationDependencyException);

            return archivingEventV2OrchestrationDependencyException;
        }

        private async ValueTask<ArchivingEventV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var archivingEventV2OrchestrationServiceException =
                new ArchivingEventV2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationServiceException);

            return archivingEventV2OrchestrationServiceException;
        }
    }
}
