// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListenerV2Client"/> class with
        /// the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The application service provider used to open a fresh
        /// scope per operation.</param>
        public EventListenerV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public async ValueTask<EventListenerV2> RegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventListenerV2OrchestrationService eventListenerV2OrchestrationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventListenerV2OrchestrationService>();

            try
            {
                return await eventListenerV2OrchestrationService
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventListenerV2ClientServiceException(exception);
            }
        }

        public async ValueTask<IReadOnlyList<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdAsync(
            Guid eventAddressId,
            EventListenerV2Query eventListenerV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventListenerV2OrchestrationService eventListenerV2OrchestrationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventListenerV2OrchestrationService>();

            try
            {
                return await eventListenerV2OrchestrationService
                    .RetrieveEventListenerV2sByEventAddressIdByQueryAsync(
                        eventAddressId, eventListenerV2Query, cancellationToken);
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventListenerV2ClientServiceException(exception);
            }
        }

        public async ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventListenerV2OrchestrationService eventListenerV2OrchestrationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventListenerV2OrchestrationService>();

            try
            {
                return await eventListenerV2OrchestrationService
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventListenerV2ClientServiceException(exception);
            }
        }

        public async ValueTask<EventListenerV2> RetrieveOrRegisterEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventListenerV2OrchestrationService eventListenerV2OrchestrationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventListenerV2OrchestrationService>();

            try
            {
                return await eventListenerV2OrchestrationService
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventListenerV2ClientServiceException(exception);
            }
        }

        private static EventListenerV2ClientValidationException
            CreateEventListenerV2ClientValidationException(Xeption innerException)
        {
            return new EventListenerV2ClientValidationException(
                message: "Event listener client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventListenerV2ClientDependencyException
            CreateEventListenerV2ClientDependencyException(Xeption innerException)
        {
            return new EventListenerV2ClientDependencyException(
                message: "Event listener client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventListenerV2ClientServiceException
            CreateEventListenerV2ClientServiceException(Exception exception)
        {
            Xeption innerException = exception as Xeption
                ?? new Xeption(exception?.Message, exception);

            return new EventListenerV2ClientServiceException(
                message: "Event listener client service error occurred, contact support.",
                innerException: innerException,
                data: exception?.Data);
        }
    }
}
