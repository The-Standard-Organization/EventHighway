// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions;
using EventHighway.Core.Services.Foundations.EventAddresses.V1;
using Xeptions;

namespace EventHighway.Core.Clients.EventAddresses.V1
{
    internal class EventAddressesV1Client : IEventAddressesV1Client
    {
        private readonly IEventAddressV1Service eventAddressV1Service;

        public EventAddressesV1Client(IEventAddressV1Service eventAddressV1Service) =>
            this.eventAddressV1Service = eventAddressV1Service;

        [Obsolete("This function is deprecated use the latest version instead.}")]
        public async ValueTask<EventAddressV1> RegisterEventAddressV1Async(EventAddressV1 eventAddressV1)
        {
            try
            {
                return await this.eventAddressV1Service.AddEventAddressV1Async(eventAddressV1);
            }
            catch (EventAddressV1ValidationException eventAddressV1ValidationException)
            {
                throw CreateEventAddressV1ClientDependencyValidationException(
                    eventAddressV1ValidationException.InnerException
                        as Xeption);
            }
            catch (EventAddressV1DependencyValidationException eventAddressV1DependencyValidationException)
            {
                throw CreateEventAddressV1ClientDependencyValidationException(
                    eventAddressV1DependencyValidationException.InnerException
                        as Xeption);
            }
            catch (EventAddressV1DependencyException eventV1DependencyException)
            {
                throw CreateEventAddressV1ClientDependencyException(
                    eventV1DependencyException.InnerException
                        as Xeption);
            }
            catch (EventAddressV1ServiceException eventV1ServiceException)
            {
                throw CreateEventAddressV1ClientServiceException(
                    eventV1ServiceException.InnerException
                        as Xeption);
            }
        }

        [Obsolete("This function is deprecated use the latest version instead.}")]
        public async ValueTask<IQueryable<EventAddressV1>> RetrieveAllEventAddressV1sAsync()
        {
            try
            {
                return await this.eventAddressV1Service
                    .RetrieveAllEventAddressV1sAsync();
            }
            catch (EventListenerV1OrchestrationDependencyException
                eventListenerV1OrchestrationDependencyException)
            {
                throw CreateEventAddressV1ClientDependencyException(
                    eventListenerV1OrchestrationDependencyException.InnerException
                        as Xeption);
            }
            catch (EventListenerV1OrchestrationServiceException
                eventListenerV1OrchestrationServiceException)
            {
                throw CreateEventAddressV1ClientServiceException(
                    eventListenerV1OrchestrationServiceException.InnerException
                        as Xeption);
            }
        }

        [Obsolete("This function is deprecated use the latest version instead.}")]
        public async ValueTask<EventAddressV1> RemoveEventAddressV1ByIdAsync(Guid eventAddressV1Id)
        {
            try
            {
                return await this.eventAddressV1Service.RemoveEventAddressV1ByIdAsync(
                    eventAddressV1Id);
            }
            catch (EventAddressV1ValidationException eventAddressV1ValidationException)
            {
                throw CreateEventAddressV1ClientDependencyValidationException(
                    eventAddressV1ValidationException.InnerException
                        as Xeption);
            }
            catch (EventAddressV1DependencyValidationException eventAddressV1DependencyValidationException)
            {
                throw CreateEventAddressV1ClientDependencyValidationException(
                    eventAddressV1DependencyValidationException.InnerException
                        as Xeption);
            }
            catch (EventAddressV1DependencyException eventV1DependencyException)
            {
                throw CreateEventAddressV1ClientDependencyException(
                    eventV1DependencyException.InnerException
                        as Xeption);
            }
            catch (EventAddressV1ServiceException eventV1ServiceException)
            {
                throw CreateEventAddressV1ClientServiceException(
                    eventV1ServiceException.InnerException
                        as Xeption);
            }
        }

        private static EventAddressV1ClientDependencyValidationException
            CreateEventAddressV1ClientDependencyValidationException(
                Xeption innerException)
        {
            return new EventAddressV1ClientDependencyValidationException(
                message: "Event address client validation error occurred, fix the errors and try again.",
                innerException: innerException);
        }

        private static EventAddressV1ClientDependencyException CreateEventAddressV1ClientDependencyException(
            Xeption innerException)
        {
            return new EventAddressV1ClientDependencyException(
                message: "Event address client dependency error occurred, contact support.",
                innerException: innerException);
        }

        private static EventAddressV1ClientServiceException CreateEventAddressV1ClientServiceException(
            Xeption innerException)
        {
            return new EventAddressV1ClientServiceException(
                message: "Event address client service error occurred, contact support.",
                innerException: innerException);
        }
    }
}
