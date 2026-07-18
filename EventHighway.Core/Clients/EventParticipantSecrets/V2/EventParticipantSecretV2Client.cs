// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipantSecrets.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.EventParticipantSecrets.V2
{
    internal class EventParticipantSecretV2Client : IEventParticipantSecretV2Client
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public EventParticipantSecretV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public async ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantSecretV2Service eventParticipantSecretV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantSecretV2Service>();

            try
            {
                return await eventParticipantSecretV2Service
                    .AddEventParticipantSecretV2Async(eventParticipantSecretV2, cancellationToken);
            }
            catch (EventParticipantSecretV2ValidationException eventParticipantSecretV2ValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2ValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyValidationException
                eventParticipantSecretV2DependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyException eventParticipantSecretV2DependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2DependencyException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2ServiceException eventParticipantSecretV2ServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception);
            }
        }

        public async ValueTask<IReadOnlyList<EventParticipantSecretV2>> RetrieveAllEventParticipantSecretV2sAsync(
            EventParticipantSecretV2Query eventParticipantSecretV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantSecretV2Service eventParticipantSecretV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantSecretV2Service>();

            try
            {
                return await eventParticipantSecretV2Service
                    .RetrieveEventParticipantSecretV2sByQueryAsync(
                        eventParticipantSecretV2Query, cancellationToken);
            }
            catch (EventParticipantSecretV2ValidationException eventParticipantSecretV2ValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2ValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyValidationException
                eventParticipantSecretV2DependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyException eventParticipantSecretV2DependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2DependencyException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2ServiceException eventParticipantSecretV2ServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception);
            }
        }

        public async ValueTask<EventParticipantSecretV2> RetrieveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantSecretV2Service eventParticipantSecretV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantSecretV2Service>();

            try
            {
                return await eventParticipantSecretV2Service
                    .RetrieveEventParticipantSecretV2ByIdAsync(eventParticipantSecretV2Id, cancellationToken);
            }
            catch (EventParticipantSecretV2ValidationException eventParticipantSecretV2ValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2ValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyValidationException
                eventParticipantSecretV2DependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyException eventParticipantSecretV2DependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2DependencyException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2ServiceException eventParticipantSecretV2ServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception);
            }
        }

        public async ValueTask<EventParticipantSecretV2> ModifyEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantSecretV2Service eventParticipantSecretV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantSecretV2Service>();

            try
            {
                return await eventParticipantSecretV2Service
                    .ModifyEventParticipantSecretV2Async(eventParticipantSecretV2, cancellationToken);
            }
            catch (EventParticipantSecretV2ValidationException eventParticipantSecretV2ValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2ValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyValidationException
                eventParticipantSecretV2DependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyException eventParticipantSecretV2DependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2DependencyException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2ServiceException eventParticipantSecretV2ServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception);
            }
        }

        public async ValueTask<EventParticipantSecretV2> RemoveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventParticipantSecretV2Service eventParticipantSecretV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventParticipantSecretV2Service>();

            try
            {
                return await eventParticipantSecretV2Service
                    .RemoveEventParticipantSecretV2ByIdAsync(eventParticipantSecretV2Id, cancellationToken);
            }
            catch (EventParticipantSecretV2ValidationException eventParticipantSecretV2ValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2ValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyValidationException
                eventParticipantSecretV2DependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantSecretV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2DependencyException eventParticipantSecretV2DependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2DependencyException.InnerException as Xeption);
            }
            catch (EventParticipantSecretV2ServiceException eventParticipantSecretV2ServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantSecretV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception);
            }
        }

        private static EventParticipantSecretV2ClientValidationException
            CreateClientValidationException(Xeption innerException)
        {
            return new EventParticipantSecretV2ClientValidationException(
                message: "Event participant secret client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventParticipantSecretV2ClientDependencyException
            CreateClientDependencyException(Xeption innerException)
        {
            return new EventParticipantSecretV2ClientDependencyException(
                message: "Event participant secret client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventParticipantSecretV2ClientServiceException
            CreateClientServiceException(Exception exception)
        {
            Xeption innerException = exception as Xeption
                ?? new Xeption(exception?.Message, exception);

            return new EventParticipantSecretV2ClientServiceException(
                message: "Event participant secret client service error occurred, contact support.",
                innerException: innerException,
                data: exception?.Data);
        }
    }
}
