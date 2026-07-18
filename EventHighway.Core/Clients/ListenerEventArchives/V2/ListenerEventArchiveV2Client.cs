// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.ListenerEventArchives.V2
{
    /// <summary>
    /// Represents the V2 listener event archive client implementation, exposing read operations
    /// over archived listener events while managing foundation service exceptions.
    /// </summary>
    internal class ListenerEventArchiveV2Client : IListenerEventArchiveV2Client
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerEventArchiveV2Client"/> class with
        /// the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The application service provider used to open a fresh
        /// scope per operation.</param>
        public ListenerEventArchiveV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public async ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IListenerEventArchiveV2Service listenerEventArchiveV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IListenerEventArchiveV2Service>();

            try
            {
                return await listenerEventArchiveV2Service
                    .RetrieveListenerEventArchiveV2sByQueryAsync(
                        listenerEventArchiveV2Query, cancellationToken);
            }
            catch (ListenerEventArchiveV2ValidationException
                listenerEventArchiveV2ValidationException)
            {
                throw CreateListenerEventArchiveV2ClientValidationException(
                    listenerEventArchiveV2ValidationException.InnerException as Xeption);
            }
            catch (ListenerEventArchiveV2DependencyValidationException
                listenerEventArchiveV2DependencyValidationException)
            {
                throw CreateListenerEventArchiveV2ClientValidationException(
                    listenerEventArchiveV2DependencyValidationException.InnerException as Xeption);
            }
            catch (ListenerEventArchiveV2DependencyException
                listenerEventArchiveV2DependencyException)
            {
                throw CreateListenerEventArchiveV2ClientDependencyException(
                    listenerEventArchiveV2DependencyException.InnerException as Xeption);
            }
            catch (ListenerEventArchiveV2ServiceException
                listenerEventArchiveV2ServiceException)
            {
                throw CreateListenerEventArchiveV2ClientDependencyException(
                    listenerEventArchiveV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateListenerEventArchiveV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<IReadOnlyList<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
            ListenerEventArchiveV2Query listenerEventArchiveV2Query,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IListenerEventArchiveV2Service listenerEventArchiveV2Service =
                serviceScope.ServiceProvider
                    .GetRequiredService<IListenerEventArchiveV2Service>();

            try
            {
                return await listenerEventArchiveV2Service
                    .RetrieveListenerEventArchiveV2sWithEventListenerV2ByQueryAsync(
                        listenerEventArchiveV2Query, cancellationToken);
            }
            catch (ListenerEventArchiveV2ValidationException
                listenerEventArchiveV2ValidationException)
            {
                throw CreateListenerEventArchiveV2ClientValidationException(
                    listenerEventArchiveV2ValidationException.InnerException as Xeption);
            }
            catch (ListenerEventArchiveV2DependencyValidationException
                listenerEventArchiveV2DependencyValidationException)
            {
                throw CreateListenerEventArchiveV2ClientValidationException(
                    listenerEventArchiveV2DependencyValidationException.InnerException as Xeption);
            }
            catch (ListenerEventArchiveV2DependencyException
                listenerEventArchiveV2DependencyException)
            {
                throw CreateListenerEventArchiveV2ClientDependencyException(
                    listenerEventArchiveV2DependencyException.InnerException as Xeption);
            }
            catch (ListenerEventArchiveV2ServiceException
                listenerEventArchiveV2ServiceException)
            {
                throw CreateListenerEventArchiveV2ClientDependencyException(
                    listenerEventArchiveV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateListenerEventArchiveV2ClientServiceException(exception as Xeption);
            }
        }

        private static ListenerEventArchiveV2ClientValidationException
            CreateListenerEventArchiveV2ClientValidationException(Xeption innerException)
        {
            return new ListenerEventArchiveV2ClientValidationException(
                message: "Listener event archive client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ListenerEventArchiveV2ClientDependencyException
            CreateListenerEventArchiveV2ClientDependencyException(Xeption innerException)
        {
            return new ListenerEventArchiveV2ClientDependencyException(
                message: "Listener event archive client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ListenerEventArchiveV2ClientServiceException
            CreateListenerEventArchiveV2ClientServiceException(Xeption innerException)
        {
            return new ListenerEventArchiveV2ClientServiceException(
                message: "Listener event archive client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
