// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using EventHighway.Core.Services.Processings.EventAddresses.V2;
using Xeptions;

namespace EventHighway.Core.Clients.EventAddresses.V2
{
    internal class EventAddressV2Client : IEventAddressV2Client
    {
        private readonly IEventAddressV2ProcessingService eventAddressV2ProcessingService;

        public EventAddressV2Client(IEventAddressV2ProcessingService eventAddressV2ProcessingService) =>
            this.eventAddressV2ProcessingService = eventAddressV2ProcessingService;

        public async ValueTask<EventAddressV2> RegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventAddressV2ProcessingService
                    .RegisterEventAddressV2Async(eventAddressV2, cancellationToken);
            }
            catch (EventAddressV2ProcessingValidationException
                eventAddressV2ProcessingValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingDependencyValidationException
                eventAddressV2ProcessingDependencyValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingDependencyException
                eventAddressV2ProcessingDependencyException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingServiceException
                eventAddressV2ProcessingServiceException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventAddressV2> RetrieveOrRegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventAddressV2ProcessingService
                    .RetrieveOrRegisterEventAddressV2Async(eventAddressV2, cancellationToken);
            }
            catch (EventAddressV2ProcessingValidationException
                eventAddressV2ProcessingValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingDependencyValidationException
                eventAddressV2ProcessingDependencyValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingDependencyException
                eventAddressV2ProcessingDependencyException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingServiceException
                eventAddressV2ProcessingServiceException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventAddressV2ProcessingService.RetrieveAllEventAddressV2sAsync();
            }
            catch (EventAddressV2ProcessingDependencyException
                eventAddressV2ProcessingDependencyException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingServiceException
                eventAddressV2ProcessingServiceException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventAddressV2ProcessingService
                    .RemoveEventAddressV2ByIdAsync(eventAddressV2Id, cancellationToken);
            }
            catch (EventAddressV2ProcessingValidationException
                eventAddressV2ProcessingValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingDependencyValidationException
                eventAddressV2ProcessingDependencyValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingDependencyException
                eventAddressV2ProcessingDependencyException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventAddressV2ProcessingServiceException
                eventAddressV2ProcessingServiceException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        private static EventAddressV2ClientValidationException
            CreateEventAddressV2ClientValidationException(Xeption innerException)
        {
            return new EventAddressV2ClientValidationException(
                message: "Event address client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static EventAddressV2ClientDependencyException
            CreateEventAddressV2ClientDependencyException(Xeption innerException)
        {
            return new EventAddressV2ClientDependencyException(
                message: "Event address client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static EventAddressV2ClientServiceException
            CreateEventAddressV2ClientServiceException(Xeption innerException)
        {
            return new EventAddressV2ClientServiceException(
                message: "Event address client service error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }
    }
}
