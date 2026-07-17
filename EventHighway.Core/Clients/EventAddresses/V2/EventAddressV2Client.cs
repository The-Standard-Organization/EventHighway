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
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.EventAddresses.V2
{
    /// <summary>
    /// Represents the V2 event address client implementation, handling event address
    /// registration, retrieval, and removal operations while managing processing service
    /// exceptions.
    /// </summary>
    internal class EventAddressV2Client : IEventAddressV2Client
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAddressV2Client"/> class with the
        /// specified service provider used to resolve the event address processing service.
        /// Every operation resolves its dependencies within a new service scope so that
        /// concurrent operations never share a storage context.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to resolve
        /// dependencies.</param>
        /// <exception cref="ArgumentNullException">Thrown when serviceProvider is
        /// null.</exception>
        public EventAddressV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        /// <summary>
        /// Registers a new event address asynchronously by delegating to the processing
        /// service and handling any exceptions that occur.
        /// </summary>
        /// <param name="eventAddressV2">The event address to register.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventAddressV2}"/> representing the asynchronous
        /// operation that returns the registered event address.</returns>
        /// <exception cref="EventAddressV2ClientValidationException">Thrown when validation
        /// errors occur in the processing service.</exception>
        /// <exception cref="EventAddressV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventAddressV2ClientServiceException">Thrown when an unexpected
        /// error occurs during registration.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<EventAddressV2> RegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventAddressV2ProcessingService eventAddressV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventAddressV2ProcessingService>();

            try
            {
                return await eventAddressV2ProcessingService
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Retrieves an existing event address or registers a new one asynchronously by
        /// delegating to the processing service and handling any exceptions that occur.
        /// </summary>
        /// <param name="eventAddressV2">The event address to retrieve or register.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventAddressV2}"/> representing the asynchronous
        /// operation that returns the retrieved or registered event address.</returns>
        /// <exception cref="EventAddressV2ClientValidationException">Thrown when validation
        /// errors occur in the processing service.</exception>
        /// <exception cref="EventAddressV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventAddressV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval or registration.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<EventAddressV2> RetrieveOrRegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventAddressV2ProcessingService eventAddressV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventAddressV2ProcessingService>();

            try
            {
                return await eventAddressV2ProcessingService
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Retrieves all event addresses asynchronously by delegating to the processing
        /// service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of event addresses.</returns>
        /// <exception cref="EventAddressV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventAddressV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventAddressV2ProcessingService eventAddressV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventAddressV2ProcessingService>();

            try
            {
                return (await eventAddressV2ProcessingService
                    .RetrieveAllEventAddressV2sAsync(cancellationToken))
                        .ToList()
                        .AsQueryable();
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Removes an event address by its identifier asynchronously by delegating to the
        /// processing service and handling any exceptions that occur.
        /// </summary>
        /// <param name="eventAddressV2Id">The identifier of the event address to remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventAddressV2}"/> representing the asynchronous
        /// operation that returns the removed event address.</returns>
        /// <exception cref="EventAddressV2ClientValidationException">Thrown when validation
        /// errors occur in the processing service.</exception>
        /// <exception cref="EventAddressV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventAddressV2ClientServiceException">Thrown when an unexpected
        /// error occurs during removal.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventAddressV2ProcessingService eventAddressV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventAddressV2ProcessingService>();

            try
            {
                return await eventAddressV2ProcessingService
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
            catch (OperationCanceledException)
            {
                throw;
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
                data: innerException?.Data);
        }

        private static EventAddressV2ClientDependencyException
            CreateEventAddressV2ClientDependencyException(Xeption innerException)
        {
            return new EventAddressV2ClientDependencyException(
                message: "Event address client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventAddressV2ClientServiceException
            CreateEventAddressV2ClientServiceException(Xeption innerException)
        {
            return new EventAddressV2ClientServiceException(
                message: "Event address client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
