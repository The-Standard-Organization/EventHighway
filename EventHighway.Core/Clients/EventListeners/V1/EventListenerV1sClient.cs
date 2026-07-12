// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventListeners.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventListeners.V1;
using Xeptions;

namespace EventHighway.Core.Clients.EventListeners.V1
{
    internal class EventListenerV1sClient : IEventListenerV1sClient
    {
        private readonly IEventListenerV1OrchestrationService eventListenerV1OrchestrationService;

        public EventListenerV1sClient(IEventListenerV1OrchestrationService eventListenerV1OrchestrationService) =>
            this.eventListenerV1OrchestrationService = eventListenerV1OrchestrationService;

        [Obsolete("This function is deprecated use the latest version instead.}")]
        public async ValueTask<EventListenerV1> RegisterEventListenerV1Async(
            EventListenerV1 eventListenerV1)
        {
            try
            {
                return await this.eventListenerV1OrchestrationService
                    .AddEventListenerV1Async(eventListenerV1);
            }
            catch (EventListenerV1OrchestrationValidationException
                eventListenerV1OrchestrationValidationException)
            {
                throw CreateEventListenerV1ClientDependencyValidationException(
                    eventListenerV1OrchestrationValidationException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationDependencyValidationException
                eventListenerV1OrchestrationDependencyValidationException)
            {
                throw CreateEventListenerV1ClientDependencyValidationException(
                    eventListenerV1OrchestrationDependencyValidationException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationDependencyException
                eventListenerV1OrchestrationDependencyException)
            {
                throw CreateEventListenerV1ClientDependencyException(
                    eventListenerV1OrchestrationDependencyException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationServiceException
                eventListenerV1OrchestrationServiceException)
            {
                throw CreateEventListenerV1ClientServiceException(
                    eventListenerV1OrchestrationServiceException.InnerException
                        as Xeption);
            }
        }

        [Obsolete("This function is deprecated use the latest version instead.}")]
        public async ValueTask<EventListenerV1> RemoveEventListenerV1ByIdAsync(
            Guid eventListenerV1Id)
        {
            try
            {
                return await this.eventListenerV1OrchestrationService
                    .RemoveEventListenerV1ByIdAsync(eventListenerV1Id);
            }
            catch (EventListenerV1OrchestrationValidationException
                eventListenerV1OrchestrationValidationException)
            {
                throw CreateEventListenerV1ClientDependencyValidationException(
                    eventListenerV1OrchestrationValidationException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationDependencyValidationException
                eventListenerV1OrchestrationDependencyValidationException)
            {
                throw CreateEventListenerV1ClientDependencyValidationException(
                    eventListenerV1OrchestrationDependencyValidationException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationDependencyException
                eventListenerV1OrchestrationDependencyException)
            {
                throw CreateEventListenerV1ClientDependencyException(
                    eventListenerV1OrchestrationDependencyException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationServiceException
                eventListenerV1OrchestrationServiceException)
            {
                throw CreateEventListenerV1ClientServiceException(
                    eventListenerV1OrchestrationServiceException.InnerException
                        as Xeption);
            }
        }

        private static EventListenerV1ClientDependencyValidationException
            CreateEventListenerV1ClientDependencyValidationException(
                Xeption innerException)
        {
            return new EventListenerV1ClientDependencyValidationException(
                message: "Event listener client validation error occurred, fix the errors and try again.",
                innerException: innerException);
        }

        private static EventListenerV1ClientDependencyException CreateEventListenerV1ClientDependencyException(
            Xeption innerException)
        {
            return new EventListenerV1ClientDependencyException(
                message: "Event listener client dependency error occurred, contact support.",
                innerException: innerException);
        }

        private static EventListenerV1ClientServiceException CreateEventListenerV1ClientServiceException(
            Xeption innerException)
        {
            return new EventListenerV1ClientServiceException(
                message: "Event listener client service error occurred, contact support.",
                innerException: innerException);
        }
    }
}
