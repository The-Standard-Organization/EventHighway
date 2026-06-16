// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using Xeptions;

namespace EventHighway.Core.Clients.EventListeners.V2
{
    /// <summary>
    /// Represents the V2 event listener client implementation, handling event listener
    /// registration, retrieval, and removal operations while managing orchestration service
    /// exceptions.
    /// </summary>
    internal class EventListenerV2Client : IEventListenerV2Client
    {
        private readonly IEventListenerV2OrchestrationService eventListenerV2OrchestrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListenerV2Client"/> class with
        /// the specified orchestration service.
        /// </summary>
        /// <param name="eventListenerV2OrchestrationService">The orchestration service for
        /// managing event listeners.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// eventListenerV2OrchestrationService is null.</exception>
        public EventListenerV2Client(IEventListenerV2OrchestrationService eventListenerV2OrchestrationService) =>
            this.eventListenerV2OrchestrationService = eventListenerV2OrchestrationService;

        /// <summary>
        /// Registers a new event listener asynchronously by delegating to the orchestration
        /// service and handling any exceptions that occur.
        /// </summary>
        /// <param name="eventListenerV2">The event listener to register.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventListenerV2}"/> representing the asynchronous
        /// operation that returns the registered event listener.</returns>
        /// <exception cref="EventListenerV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="EventListenerV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventListenerV2ClientServiceException">Thrown when an unexpected
        /// error occurs during registration.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<EventListenerV2> RegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventListenerV2OrchestrationService
                    .AddEventListenerV2Async(eventListenerV2, cancellationToken);
            }
            catch (EventListenerV2OrchestrationValidationException
                eventListenerV2OrchestrationValidationException)
            {
                throw CreateEventListenerV2ClientValidationException(
                    eventListenerV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyValidationException
                eventListenerV2OrchestrationDependencyValidationException)
            {
                throw CreateEventListenerV2ClientValidationException(
                    eventListenerV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyException
                eventListenerV2OrchestrationDependencyException)
            {
                throw CreateEventListenerV2ClientDependencyException(
                    eventListenerV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationServiceException
                eventListenerV2OrchestrationServiceException)
            {
                throw CreateEventListenerV2ClientDependencyException(
                    eventListenerV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventListenerV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Retrieves event listeners by event address identifier asynchronously by delegating
        /// to the orchestration service and handling any exceptions that occur.
        /// </summary>
        /// <param name="eventAddressId">The identifier of the event address to filter
        /// listeners by.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of event listeners for the specified
        /// event address.</returns>
        /// <exception cref="EventListenerV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="EventListenerV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventListenerV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<IQueryable<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventListenerV2OrchestrationService
                    .RetrieveEventListenerV2sByEventAddressIdAsync(eventAddressId, cancellationToken);
            }
            catch (EventListenerV2OrchestrationValidationException
                eventListenerV2OrchestrationValidationException)
            {
                throw CreateEventListenerV2ClientValidationException(
                    eventListenerV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyValidationException
                eventListenerV2OrchestrationDependencyValidationException)
            {
                throw CreateEventListenerV2ClientValidationException(
                    eventListenerV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyException
                eventListenerV2OrchestrationDependencyException)
            {
                throw CreateEventListenerV2ClientDependencyException(
                    eventListenerV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationServiceException
                eventListenerV2OrchestrationServiceException)
            {
                throw CreateEventListenerV2ClientDependencyException(
                    eventListenerV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventListenerV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Removes an event listener by its identifier asynchronously by delegating to the
        /// orchestration service and handling any exceptions that occur.
        /// </summary>
        /// <param name="eventListenerV2Id">The identifier of the event listener to remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventListenerV2}"/> representing the asynchronous
        /// operation that returns the removed event listener.</returns>
        /// <exception cref="EventListenerV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="EventListenerV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventListenerV2ClientServiceException">Thrown when an unexpected
        /// error occurs during removal.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventListenerV2OrchestrationService
                    .RemoveEventListenerV2ByIdAsync(eventListenerV2Id, cancellationToken);
            }
            catch (EventListenerV2OrchestrationValidationException
                eventListenerV2OrchestrationValidationException)
            {
                throw CreateEventListenerV2ClientValidationException(
                    eventListenerV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyValidationException
                eventListenerV2OrchestrationDependencyValidationException)
            {
                throw CreateEventListenerV2ClientValidationException(
                    eventListenerV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyException
                eventListenerV2OrchestrationDependencyException)
            {
                throw CreateEventListenerV2ClientDependencyException(
                    eventListenerV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationServiceException
                eventListenerV2OrchestrationServiceException)
            {
                throw CreateEventListenerV2ClientDependencyException(
                    eventListenerV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventListenerV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventListenerV2> RetrieveOrRegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventListenerV2OrchestrationService
                    .RetrieveOrRegisterEventListenerV2Async(eventListenerV2, cancellationToken);
            }
            catch (EventListenerV2OrchestrationValidationException
                eventListenerV2OrchestrationValidationException)
            {
                throw CreateEventListenerV2ClientValidationException(
                    eventListenerV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyValidationException
                eventListenerV2OrchestrationDependencyValidationException)
            {
                throw CreateEventListenerV2ClientValidationException(
                    eventListenerV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyException
                eventListenerV2OrchestrationDependencyException)
            {
                throw CreateEventListenerV2ClientDependencyException(
                    eventListenerV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationServiceException
                eventListenerV2OrchestrationServiceException)
            {
                throw CreateEventListenerV2ClientDependencyException(
                    eventListenerV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventListenerV2ClientServiceException(exception as Xeption);
            }
        }

        private static EventListenerV2ClientValidationException
            CreateEventListenerV2ClientValidationException(Xeption innerException)
        {
            return new EventListenerV2ClientValidationException(
                message: "Event listener client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static EventListenerV2ClientDependencyException
            CreateEventListenerV2ClientDependencyException(Xeption innerException)
        {
            return new EventListenerV2ClientDependencyException(
                message: "Event listener client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static EventListenerV2ClientServiceException
            CreateEventListenerV2ClientServiceException(Xeption innerException)
        {
            return new EventListenerV2ClientServiceException(
                message: "Event listener client service error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }
    }
}
