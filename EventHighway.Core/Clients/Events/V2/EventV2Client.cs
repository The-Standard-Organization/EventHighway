// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Coordinations.Events.V2;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Coordinations.Events.V2;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.Events.V2
{
    /// <summary>
    /// Represents the V2 event client implementation, handling event submission, scheduled event
    /// firing, and event removal operations while managing coordination service exceptions.
    /// </summary>
    internal class EventV2Client : IEventV2Client
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventV2Client"/> class with the
        /// specified service provider used to resolve the event coordination service.
        /// Every operation resolves its dependencies within a new service scope so that
        /// concurrent operations never share a storage context.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to resolve
        /// dependencies.</param>
        /// <exception cref="ArgumentNullException">Thrown when serviceProvider is
        /// null.</exception>
        public EventV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        /// <summary>
        /// Submits an event asynchronously by delegating to the coordination service and
        /// handling any exceptions that occur.
        /// </summary>
        /// <param name="eventV2">The event to submit.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventV2}"/> representing the asynchronous operation
        /// that returns the submitted event.</returns>
        /// <exception cref="EventV2ClientValidationException">Thrown when validation errors
        /// occur in the coordination service.</exception>
        /// <exception cref="EventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventV2ClientServiceException">Thrown when an unexpected error
        /// occurs during submission.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<EventV2> SubmitEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventV2CoordinationService eventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventV2CoordinationService>();

            try
            {
                return await eventV2CoordinationService
                    .SubmitEventV2Async(eventV2, cancellationToken);
            }
            catch (EventV2CoordinationValidationException
                eventV2CoordinationValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyValidationException
                eventV2CoordinationDependencyValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyException
                eventV2CoordinationDependencyException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (EventV2CoordinationServiceException
                eventV2CoordinationServiceException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Fires scheduled pending events asynchronously by delegating to the coordination
        /// service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="EventV2ClientValidationException">Thrown when validation errors
        /// occur in the coordination service.</exception>
        /// <exception cref="EventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventV2ClientServiceException">Thrown when an unexpected error
        /// occurs during firing.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask FireScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventV2CoordinationService eventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventV2CoordinationService>();

            try
            {
                await eventV2CoordinationService
                    .FireScheduledPendingEventV2sAsync(cancellationToken);
            }
            catch (EventV2CoordinationValidationException
                eventV2CoordinationValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyValidationException
                eventV2CoordinationDependencyValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyException
                eventV2CoordinationDependencyException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (EventV2CoordinationServiceException
                eventV2CoordinationServiceException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Removes an event by its identifier asynchronously by delegating to the coordination
        /// service and handling any exceptions that occur.
        /// </summary>
        /// <param name="eventV2Id">The identifier of the event to remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventV2}"/> representing the asynchronous operation
        /// that returns the removed event.</returns>
        /// <exception cref="EventV2ClientValidationException">Thrown when validation errors
        /// occur in the coordination service.</exception>
        /// <exception cref="EventV2ClientDependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="EventV2ClientServiceException">Thrown when an unexpected error
        /// occurs during removal.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<IReadOnlyList<EventV2>> RetrieveAllEventV2sAsync(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventV2CoordinationService eventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventV2CoordinationService>();

            try
            {
                return (await eventV2CoordinationService
                    .RetrieveEventV2sByQueryAsync(eventV2Query, cancellationToken))
                        .ToList();
            }
            catch (EventV2CoordinationValidationException
                eventV2CoordinationValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyValidationException
                eventV2CoordinationDependencyValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyException
                eventV2CoordinationDependencyException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (EventV2CoordinationServiceException
                eventV2CoordinationServiceException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<IReadOnlyList<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            EventV2Query eventV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventV2CoordinationService eventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventV2CoordinationService>();

            try
            {
                return (await eventV2CoordinationService
                    .RetrieveAllEventV2sWithEventAddressV2Async(cancellationToken))
                        .ToList();
            }
            catch (EventV2CoordinationValidationException
                eventV2CoordinationValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyValidationException
                eventV2CoordinationDependencyValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyException
                eventV2CoordinationDependencyException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (EventV2CoordinationServiceException
                eventV2CoordinationServiceException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventV2> RetrieveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventV2CoordinationService eventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventV2CoordinationService>();

            try
            {
                return await eventV2CoordinationService
                    .RetrieveEventV2ByIdAsync(eventV2Id, cancellationToken);
            }
            catch (EventV2CoordinationValidationException
                eventV2CoordinationValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyValidationException
                eventV2CoordinationDependencyValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyException
                eventV2CoordinationDependencyException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (EventV2CoordinationServiceException
                eventV2CoordinationServiceException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventV2CoordinationService eventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventV2CoordinationService>();

            try
            {
                return await eventV2CoordinationService
                    .RemoveEventV2ByIdAsync(eventV2Id, cancellationToken);
            }
            catch (EventV2CoordinationValidationException
                eventV2CoordinationValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyValidationException
                eventV2CoordinationDependencyValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyException
                eventV2CoordinationDependencyException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (EventV2CoordinationServiceException
                eventV2CoordinationServiceException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventV2ClientServiceException(exception as Xeption);
            }
        }

        private static EventV2ClientValidationException
            CreateEventV2ClientValidationException(Xeption innerException)
        {
            return new EventV2ClientValidationException(
                message: "Event client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventV2ClientDependencyException
            CreateEventV2ClientDependencyException(Xeption innerException)
        {
            return new EventV2ClientDependencyException(
                message: "Event client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventV2ClientServiceException
            CreateEventV2ClientServiceException(Xeption innerException)
        {
            return new EventV2ClientServiceException(
                message: "Event client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
