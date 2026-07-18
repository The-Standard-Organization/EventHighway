// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventArchives.V2.Exceptions;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.EventArchives.V2
{
    /// <summary>
    /// Represents the V2 event archive client implementation, exposing read operations over
    /// archived events while managing foundation service exceptions.
    /// </summary>
    internal class EventArchiveV2Client : IEventArchiveV2Client
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArchiveV2Client"/> class with the
        /// specified event archive service.
        /// </summary>
        /// <param name="eventArchiveV2Service">The foundation service for archived events.</param>
        public EventArchiveV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public async ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventArchiveV2Service eventArchiveV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventArchiveV2Service>();

            try
            {
                return await eventArchiveV2Service
                    .RetrieveEventArchiveV2sByQueryAsync(
                        eventArchiveV2Query, cancellationToken);
            }
            catch (EventArchiveV2ValidationException
                eventArchiveV2ValidationException)
            {
                throw CreateEventArchiveV2ClientValidationException(
                    eventArchiveV2ValidationException.InnerException as Xeption);
            }
            catch (EventArchiveV2DependencyValidationException
                eventArchiveV2DependencyValidationException)
            {
                throw CreateEventArchiveV2ClientValidationException(
                    eventArchiveV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventArchiveV2DependencyException
                eventArchiveV2DependencyException)
            {
                throw CreateEventArchiveV2ClientDependencyException(
                    eventArchiveV2DependencyException.InnerException as Xeption);
            }
            catch (EventArchiveV2ServiceException
                eventArchiveV2ServiceException)
            {
                throw CreateEventArchiveV2ClientDependencyException(
                    eventArchiveV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventArchiveV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<IReadOnlyList<EventArchiveV2>> RetrieveAllEventArchiveV2sWithEventAddressV2Async(
            EventArchiveV2Query eventArchiveV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventArchiveV2Service eventArchiveV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventArchiveV2Service>();

            try
            {
                return await eventArchiveV2Service
                    .RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                        eventArchiveV2Query, cancellationToken);
            }
            catch (EventArchiveV2ValidationException
                eventArchiveV2ValidationException)
            {
                throw CreateEventArchiveV2ClientValidationException(
                    eventArchiveV2ValidationException.InnerException as Xeption);
            }
            catch (EventArchiveV2DependencyValidationException
                eventArchiveV2DependencyValidationException)
            {
                throw CreateEventArchiveV2ClientValidationException(
                    eventArchiveV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventArchiveV2DependencyException
                eventArchiveV2DependencyException)
            {
                throw CreateEventArchiveV2ClientDependencyException(
                    eventArchiveV2DependencyException.InnerException as Xeption);
            }
            catch (EventArchiveV2ServiceException
                eventArchiveV2ServiceException)
            {
                throw CreateEventArchiveV2ClientDependencyException(
                    eventArchiveV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventArchiveV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventArchiveV2> RetrieveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IEventArchiveV2Service eventArchiveV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IEventArchiveV2Service>();

            try
            {
                return await eventArchiveV2Service
                    .RetrieveEventArchiveV2ByIdAsync(eventArchiveV2Id, cancellationToken);
            }
            catch (EventArchiveV2ValidationException
                eventArchiveV2ValidationException)
            {
                throw CreateEventArchiveV2ClientValidationException(
                    eventArchiveV2ValidationException.InnerException as Xeption);
            }
            catch (EventArchiveV2DependencyValidationException
                eventArchiveV2DependencyValidationException)
            {
                throw CreateEventArchiveV2ClientValidationException(
                    eventArchiveV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventArchiveV2DependencyException
                eventArchiveV2DependencyException)
            {
                throw CreateEventArchiveV2ClientDependencyException(
                    eventArchiveV2DependencyException.InnerException as Xeption);
            }
            catch (EventArchiveV2ServiceException
                eventArchiveV2ServiceException)
            {
                throw CreateEventArchiveV2ClientDependencyException(
                    eventArchiveV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventArchiveV2ClientServiceException(exception as Xeption);
            }
        }

        private static EventArchiveV2ClientValidationException
            CreateEventArchiveV2ClientValidationException(Xeption innerException)
        {
            return new EventArchiveV2ClientValidationException(
                message: "Event archive client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventArchiveV2ClientDependencyException
            CreateEventArchiveV2ClientDependencyException(Xeption innerException)
        {
            return new EventArchiveV2ClientDependencyException(
                message: "Event archive client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventArchiveV2ClientServiceException
            CreateEventArchiveV2ClientServiceException(Xeption innerException)
        {
            return new EventArchiveV2ClientServiceException(
                message: "Event archive client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
