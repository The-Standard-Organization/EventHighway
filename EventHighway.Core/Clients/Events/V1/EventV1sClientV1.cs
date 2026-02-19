// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.Events.V1.Exceptions;
using EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions;
using EventHighway.Core.Services.Coordinations.Events.V1;
using Xeptions;

namespace EventHighway.Core.Clients.Events.V1
{
    internal class EventV1sClientV1 : IEventV1sClientV1
    {
        private readonly IEventV1CoordinationServiceV1 eventV1CoordinationServiceV1;

        public EventV1sClientV1(IEventV1CoordinationServiceV1 eventV1CoordinationServiceV1) =>
            this.eventV1CoordinationServiceV1 = eventV1CoordinationServiceV1;

        public async ValueTask ArchiveDeadEventV1sAsync()
        {
            try
            {
                await this.eventV1CoordinationServiceV1.ArchiveDeadEventV1sAsync();
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
