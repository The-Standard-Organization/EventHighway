// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEvents.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventListeners.V1;
using Xeptions;

namespace EventHighway.Core.Clients.ListenerEvents.V1
{
    internal class ListenerEventV1sClient : IListenerEventV1sClient
    {
        private readonly IEventListenerV1OrchestrationService eventListenerV1OrchestrationService;

        public ListenerEventV1sClient(IEventListenerV1OrchestrationService eventListenerV1OrchestrationService) =>
            this.eventListenerV1OrchestrationService = eventListenerV1OrchestrationService;

        [Obsolete("This function is deprecated use the latest version instead.}")]
        public async ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventV1sAsync()
        {
            try
            {
                return await this.eventListenerV1OrchestrationService
                    .RetrieveAllListenerEventV1sAsync();
            }
            catch (EventListenerV1OrchestrationDependencyException
                eventListenerV1OrchestrationDependencyException)
            {
                throw CreateListenerEventV1ClientDependencyException(
                    eventListenerV1OrchestrationDependencyException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationServiceException
                eventListenerV1OrchestrationServiceException)
            {
                throw CreateListenerEventV1ClientServiceException(
                    eventListenerV1OrchestrationServiceException.InnerException
                        as Xeption);
            }
        }

        [Obsolete("This function is deprecated use the latest version instead.}")]
        public async ValueTask<ListenerEventV1> RemoveListenerEventV1ByIdAsync(Guid listenerEventV1Id)
        {
            try
            {
                return await this.eventListenerV1OrchestrationService.RemoveListenerEventV1ByIdAsync(
                    listenerEventV1Id);
            }
            catch (EventListenerV1OrchestrationValidationException
                eventListenerV1OrchestrationValidationException)
            {
                throw CreateListenerEventV1ClientDependencyValidationException(
                    eventListenerV1OrchestrationValidationException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationDependencyValidationException
                eventListenerV1OrchestrationDependencyValidationException)
            {
                throw CreateListenerEventV1ClientDependencyValidationException(
                    eventListenerV1OrchestrationDependencyValidationException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationDependencyException
                eventListenerV1OrchestrationDependencyException)
            {
                throw CreateListenerEventV1ClientDependencyException(
                    eventListenerV1OrchestrationDependencyException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationServiceException
                eventListenerV1OrchestrationServiceException)
            {
                throw CreateListenerEventV1ClientServiceException(
                    eventListenerV1OrchestrationServiceException.InnerException
                        as Xeption);
            }
        }

        private static ListenerEventV1ClientDependencyValidationException
            CreateListenerEventV1ClientDependencyValidationException(
                Xeption innerException)
        {
            return new ListenerEventV1ClientDependencyValidationException(
                message: "Listener event client validation error occurred, fix the errors and try again.",
                innerException: innerException);
        }

        private static ListenerEventV1ClientDependencyException CreateListenerEventV1ClientDependencyException(
            Xeption innerException)
        {
            return new ListenerEventV1ClientDependencyException(
                message: "Listener event client dependency error occurred, contact support.",
                innerException: innerException);
        }

        private static ListenerEventV1ClientServiceException CreateListenerEventV1ClientServiceException(
            Xeption innerException)
        {
            return new ListenerEventV1ClientServiceException(
                message: "Listener event client service error occurred, contact support.",
                innerException: innerException);
        }
    }
}
