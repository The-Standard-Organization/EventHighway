// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Clients.EventHandlers.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using EventHighway.Core.Services.Processings.EventHandlers.V2;
using Xeptions;

namespace EventHighway.Core.Clients.EventHandlers.V2
{
    /// <summary>
    /// Represents the V2 event handler client implementation, handling event handler
    /// registration, retrieval-or-registration, and removal operations while managing
    /// processing service exceptions.
    /// </summary>
    internal class EventHandlerV2Client : IEventHandlerV2Client
    {
        private readonly IEventHandlerV2ProcessingService eventHandlerV2ProcessingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerV2Client"/> class with
        /// the specified event handler processing service.
        /// </summary>
        /// <param name="eventHandlerV2ProcessingService">The processing service for managing
        /// event handlers.</param>
        public EventHandlerV2Client(IEventHandlerV2ProcessingService eventHandlerV2ProcessingService) =>
            this.eventHandlerV2ProcessingService = eventHandlerV2ProcessingService;

        public async ValueTask<IEventHandler> RegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventHandlerV2ProcessingService
                    .RegisterEventHandlerV2Async(eventHandler, cancellationToken);
            }
            catch (EventHandlerV2ProcessingValidationException
                eventHandlerV2ProcessingValidationException)
            {
                throw CreateEventHandlerV2ClientValidationException(
                    eventHandlerV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventHandlerV2ProcessingDependencyValidationException
                eventHandlerV2ProcessingDependencyValidationException)
            {
                throw CreateEventHandlerV2ClientValidationException(
                    eventHandlerV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventHandlerV2ProcessingDependencyException
                eventHandlerV2ProcessingDependencyException)
            {
                throw CreateEventHandlerV2ClientDependencyException(
                    eventHandlerV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventHandlerV2ProcessingServiceException
                eventHandlerV2ProcessingServiceException)
            {
                throw CreateEventHandlerV2ClientDependencyException(
                    eventHandlerV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventHandlerV2ClientServiceException(exception as Xeption);
            }
        }

        private static EventHandlerV2ClientValidationException
            CreateEventHandlerV2ClientValidationException(Xeption innerException)
        {
            return new EventHandlerV2ClientValidationException(
                message: "Event handler client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventHandlerV2ClientDependencyException
            CreateEventHandlerV2ClientDependencyException(Xeption innerException)
        {
            return new EventHandlerV2ClientDependencyException(
                message: "Event handler client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventHandlerV2ClientServiceException
            CreateEventHandlerV2ClientServiceException(Xeption innerException)
        {
            return new EventHandlerV2ClientServiceException(
                message: "Event handler client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        public async ValueTask<IEventHandler> RetrieveOrRegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventHandlerV2ProcessingService
                    .RetrieveOrRegisterEventHandlerV2Async(eventHandler, cancellationToken);
            }
            catch (EventHandlerV2ProcessingValidationException
                eventHandlerV2ProcessingValidationException)
            {
                throw CreateEventHandlerV2ClientValidationException(
                    eventHandlerV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventHandlerV2ProcessingDependencyValidationException
                eventHandlerV2ProcessingDependencyValidationException)
            {
                throw CreateEventHandlerV2ClientValidationException(
                    eventHandlerV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventHandlerV2ProcessingDependencyException
                eventHandlerV2ProcessingDependencyException)
            {
                throw CreateEventHandlerV2ClientDependencyException(
                    eventHandlerV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventHandlerV2ProcessingServiceException
                eventHandlerV2ProcessingServiceException)
            {
                throw CreateEventHandlerV2ClientDependencyException(
                    eventHandlerV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventHandlerV2ClientServiceException(exception as Xeption);
            }
        }

        public ValueTask<EventHandlerV2> RemoveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
