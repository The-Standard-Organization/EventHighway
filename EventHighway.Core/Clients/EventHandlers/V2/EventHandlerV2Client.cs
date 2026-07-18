// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Clients.EventHandlers.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2.Exceptions;
using EventHighway.Core.Services.Processings.EventHandlers.V2;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerV2Client"/> class with
        /// the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The application service provider used to open a fresh
        /// scope per operation.</param>
        public EventHandlerV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public async ValueTask<IEventHandler> RegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventHandlerV2ProcessingService eventHandlerV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventHandlerV2ProcessingService>();

            try
            {
                return await eventHandlerV2ProcessingService
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
                throw CreateEventHandlerV2ClientServiceException(exception);
            }
        }

        public async ValueTask<IEventHandler> RetrieveOrRegisterEventHandlerV2Async(
            IEventHandler eventHandler,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventHandlerV2ProcessingService eventHandlerV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventHandlerV2ProcessingService>();

            try
            {
                return await eventHandlerV2ProcessingService
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
                throw CreateEventHandlerV2ClientServiceException(exception);
            }
        }

        public async ValueTask<IReadOnlyList<EventHandlerV2>> RetrieveAllEventHandlerV2sAsync(
            EventHandlerV2Query eventHandlerV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventHandlerV2ProcessingService eventHandlerV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventHandlerV2ProcessingService>();

            try
            {
                return await eventHandlerV2ProcessingService
                    .RetrieveEventHandlerV2sByQueryAsync(eventHandlerV2Query, cancellationToken);
            }
            catch (EventHandlerV2ProcessingValidationException
                eventHandlerV2ProcessingValidationException)
            {
                throw CreateEventHandlerV2ClientValidationException(
                    eventHandlerV2ProcessingValidationException.InnerException as Xeption);
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
                throw CreateEventHandlerV2ClientServiceException(exception);
            }
        }

        public async ValueTask<EventHandlerV2> RemoveEventHandlerV2ByIdAsync(
            Guid eventHandlerV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventHandlerV2ProcessingService eventHandlerV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventHandlerV2ProcessingService>();

            try
            {
                return await eventHandlerV2ProcessingService
                    .RemoveEventHandlerV2ByIdAsync(eventHandlerV2Id, cancellationToken);
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
                throw CreateEventHandlerV2ClientServiceException(exception);
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
            CreateEventHandlerV2ClientServiceException(Exception exception)
        {
            Xeption innerException = exception as Xeption
                ?? new Xeption(exception?.Message, exception);

            return new EventHandlerV2ClientServiceException(
                message: "Event handler client service error occurred, contact support.",
                innerException: innerException,
                data: exception?.Data);
        }
    }
}
