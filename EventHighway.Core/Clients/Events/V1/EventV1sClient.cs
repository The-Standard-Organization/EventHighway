// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using EventHighway.Core.Services.Coordinations.Events.V1;
using Xeptions;

namespace EventHighway.Core.Clients.Events.V1
{
    internal class EventV1sClient : IEventV1sClient
    {
        private readonly IEventV1CoordinationService eventV1CoordinationService;

        public EventV1sClient(IEventV1CoordinationService eventV1CoordinationService) =>
            this.eventV1CoordinationService = eventV1CoordinationService;

        public async ValueTask<EventV1> SubmitEventV1Async(EventV1 eventV1)
        {
            try
            {
                return await this.eventV1CoordinationService
                    .SubmitEventV1Async(eventV1);
            }
            catch (EventV1CoordinationValidationException
                eventV1CoordinationValidationException)
            {
                throw CreateEventV1ClientDependencyValidationException(
                    eventV1CoordinationValidationException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationDependencyValidationException
                eventV1CoordinationDependencyValidationException)
            {
                throw CreateEventV1ClientDependencyValidationException(
                    eventV1CoordinationDependencyValidationException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationDependencyException
                eventV1CoordinationDependencyException)
            {
                throw CreateEventV1ClientDependencyException(
                    eventV1CoordinationDependencyException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationServiceException
                eventV1CoordinationServiceException)
            {
                throw CreateEventV1ClientServiceException(
                    eventV1CoordinationServiceException.InnerException
                        as Xeption);
            }
        }

        public async ValueTask<EventV1> SubmitEventV1AsyncV1(EventV1 eventV1)
        {
            try
            {
                return await this.eventV1CoordinationService
                    .SubmitEventV1AsyncV1(eventV1);
            }
            catch (EventV1CoordinationValidationException
                eventV1CoordinationValidationException)
            {
                throw CreateEventV1ClientDependencyValidationException(
                    eventV1CoordinationValidationException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationDependencyValidationException
                eventV1CoordinationDependencyValidationException)
            {
                throw CreateEventV1ClientDependencyValidationException(
                    eventV1CoordinationDependencyValidationException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationDependencyException
                eventV1CoordinationDependencyException)
            {
                throw CreateEventV1ClientDependencyException(
                    eventV1CoordinationDependencyException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationServiceException
                eventV1CoordinationServiceException)
            {
                throw CreateEventV1ClientServiceException(
                    eventV1CoordinationServiceException.InnerException
                        as Xeption);
            }
        }

        public async ValueTask FireScheduledPendingEventV1sAsync()
        {
            try
            {
                await this.eventV1CoordinationService
                    .FireScheduledPendingEventV1sAsync();
            }
            catch (EventV1CoordinationDependencyValidationException
                eventV1CoordinationDependencyValidationException)
            {
                throw CreateEventV1ClientDependencyValidationException(
                    eventV1CoordinationDependencyValidationException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationDependencyException
                eventV1CoordinationDependencyException)
            {
                throw CreateEventV1ClientDependencyException(
                    eventV1CoordinationDependencyException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationServiceException
                eventV1CoordinationServiceException)
            {
                throw CreateEventV1ClientServiceException(
                    eventV1CoordinationServiceException.InnerException
                        as Xeption);
            }
        }

        public async ValueTask<EventV1> RemoveEventV1ByIdAsync(Guid eventV1Id)
        {
            try
            {
                return await this.eventV1CoordinationService
                    .RemoveEventV1ByIdAsync(eventV1Id);
            }
            catch (EventV1CoordinationValidationException
                eventV1CoordinationValidationException)
            {
                throw CreateEventV1ClientDependencyValidationException(
                    eventV1CoordinationValidationException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationDependencyValidationException
                eventV1CoordinationDependencyValidationException)
            {
                throw CreateEventV1ClientDependencyValidationException(
                    eventV1CoordinationDependencyValidationException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationDependencyException
                eventV1CoordinationDependencyException)
            {
                throw CreateEventV1ClientDependencyException(
                    eventV1CoordinationDependencyException.InnerException
                        as Xeption);
            }
            catch (EventV1CoordinationServiceException
                eventV1CoordinationServiceException)
            {
                throw CreateEventV1ClientServiceException(
                    eventV1CoordinationServiceException.InnerException
                        as Xeption);
            }
        }

        private static EventV1ClientDependencyValidationException CreateEventV1ClientDependencyValidationException(
            Xeption innerException)
        {
            return new EventV1ClientDependencyValidationException(
                message: "Event client validation error occurred, fix the errors and try again.",
                innerException: innerException);
        }

        private static EventV1ClientDependencyException CreateEventV1ClientDependencyException(
            Xeption innerException)
        {
            return new EventV1ClientDependencyException(
                message: "Event client dependency error occurred, contact support.",
                innerException: innerException);
        }

        private static EventV1ClientServiceException CreateEventV1ClientServiceException(
            Xeption innerException)
        {
            return new EventV1ClientServiceException(
                message: "Event client service error occurred, contact support.",
                innerException: innerException);
        }
    }
}
