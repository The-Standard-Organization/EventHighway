// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipants.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2;
using EventHighway.Core.Models.Services.Processings.EventParticipants.V2.Exceptions;
using EventHighway.Core.Services.Processings.EventParticipants.V2;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.EventParticipants.V2
{
    internal class EventParticipantV2Client : IEventParticipantV2Client
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public EventParticipantV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public async ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantV2ProcessingService eventParticipantV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantV2ProcessingService>();

            try
            {
                return await eventParticipantV2ProcessingService
                    .AddEventParticipantV2Async(eventParticipantV2, cancellationToken);
            }
            catch (EventParticipantV2ProcessingValidationException
                eventParticipantV2ProcessingValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyValidationException
                eventParticipantV2ProcessingDependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyException
                eventParticipantV2ProcessingDependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingServiceException
                eventParticipantV2ProcessingServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventParticipantV2> RetrieveOrAddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantV2ProcessingService eventParticipantV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantV2ProcessingService>();

            try
            {
                return await eventParticipantV2ProcessingService
                    .RetrieveOrAddEventParticipantV2Async(eventParticipantV2, cancellationToken);
            }
            catch (EventParticipantV2ProcessingValidationException
                eventParticipantV2ProcessingValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyValidationException
                eventParticipantV2ProcessingDependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyException
                eventParticipantV2ProcessingDependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingServiceException
                eventParticipantV2ProcessingServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<IReadOnlyList<EventParticipantV2>> RetrieveAllEventParticipantV2sAsync(
            EventParticipantV2Query eventParticipantV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantV2ProcessingService eventParticipantV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantV2ProcessingService>();

            try
            {
                return (await eventParticipantV2ProcessingService
                    .RetrieveAllEventParticipantV2sAsync(cancellationToken))
                        .ToList();
            }
            catch (EventParticipantV2ProcessingValidationException
                eventParticipantV2ProcessingValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyValidationException
                eventParticipantV2ProcessingDependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyException
                eventParticipantV2ProcessingDependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingServiceException
                eventParticipantV2ProcessingServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventParticipantV2> RetrieveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantV2ProcessingService eventParticipantV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantV2ProcessingService>();

            try
            {
                return await eventParticipantV2ProcessingService
                    .RetrieveEventParticipantV2ByIdAsync(eventParticipantV2Id, cancellationToken);
            }
            catch (EventParticipantV2ProcessingValidationException
                eventParticipantV2ProcessingValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyValidationException
                eventParticipantV2ProcessingDependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyException
                eventParticipantV2ProcessingDependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingServiceException
                eventParticipantV2ProcessingServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventParticipantV2> ModifyEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantV2ProcessingService eventParticipantV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantV2ProcessingService>();

            try
            {
                return await eventParticipantV2ProcessingService
                    .ModifyEventParticipantV2Async(eventParticipantV2, cancellationToken);
            }
            catch (EventParticipantV2ProcessingValidationException
                eventParticipantV2ProcessingValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyValidationException
                eventParticipantV2ProcessingDependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyException
                eventParticipantV2ProcessingDependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingServiceException
                eventParticipantV2ProcessingServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventParticipantV2> RemoveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantV2ProcessingService eventParticipantV2ProcessingService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantV2ProcessingService>();

            try
            {
                return await eventParticipantV2ProcessingService
                    .RemoveEventParticipantV2ByIdAsync(eventParticipantV2Id, cancellationToken);
            }
            catch (EventParticipantV2ProcessingValidationException
                eventParticipantV2ProcessingValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyValidationException
                eventParticipantV2ProcessingDependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ProcessingDependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingDependencyException
                eventParticipantV2ProcessingDependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (EventParticipantV2ProcessingServiceException
                eventParticipantV2ProcessingServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception as Xeption);
            }
        }

        private static EventParticipantV2ClientValidationException
            CreateClientValidationException(Xeption innerException)
        {
            return new EventParticipantV2ClientValidationException(
                message: "Event participant client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventParticipantV2ClientDependencyException
            CreateClientDependencyException(Xeption innerException)
        {
            return new EventParticipantV2ClientDependencyException(
                message: "Event participant client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventParticipantV2ClientServiceException
            CreateClientServiceException(Xeption innerException)
        {
            return new EventParticipantV2ClientServiceException(
                message: "Event participant client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
