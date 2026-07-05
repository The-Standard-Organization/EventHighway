// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.RetryingListenerEvents.V2;
using Xeptions;

namespace EventHighway.Core.Clients.ListenerEvents.V2
{
    /// <summary>
    /// Represents the V2 listener event client implementation, handling listener event
    /// retrieval and removal operations while managing orchestration service exceptions.
    /// </summary>
    internal class ListenerEventV2Client : IListenerEventV2Client
    {
        private readonly IListenerEventV2OrchestrationService listenerEventV2OrchestrationService;

        private readonly IRetryingListenerEventV2OrchestrationService
            retryingListenerEventV2OrchestrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerEventV2Client"/> class with
        /// the specified orchestration services.
        /// </summary>
        /// <param name="listenerEventV2OrchestrationService">The orchestration service for
        /// managing listener events.</param>
        /// <param name="retryingListenerEventV2OrchestrationService">The orchestration service
        /// for retrying failed listener events.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// listenerEventV2OrchestrationService or
        /// retryingListenerEventV2OrchestrationService is null.</exception>
        public ListenerEventV2Client(
            IListenerEventV2OrchestrationService listenerEventV2OrchestrationService,
            IRetryingListenerEventV2OrchestrationService retryingListenerEventV2OrchestrationService)
        {
            this.listenerEventV2OrchestrationService = listenerEventV2OrchestrationService;

            this.retryingListenerEventV2OrchestrationService =
                retryingListenerEventV2OrchestrationService;
        }

        /// <summary>
        /// Retrieves all listener events asynchronously by delegating to the orchestration
        /// service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of all listener events.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.listenerEventV2OrchestrationService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);
            }
            catch (ListenerEventV2OrchestrationValidationException
                listenerEventV2OrchestrationValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyValidationException
                listenerEventV2OrchestrationDependencyValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyException
                listenerEventV2OrchestrationDependencyException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationServiceException
                listenerEventV2OrchestrationServiceException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateListenerEventV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Retrieves all listener events asynchronously with their associated event listeners by 
        /// delegating to the orchestration service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of all listener events with their
        /// associated event listeners.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.listenerEventV2OrchestrationService
                    .RetrieveAllListenerEventV2sWithEventListenerV2Async(cancellationToken);
            }
            catch (ListenerEventV2OrchestrationValidationException
                listenerEventV2OrchestrationValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyValidationException
                listenerEventV2OrchestrationDependencyValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyException
                listenerEventV2OrchestrationDependencyException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationServiceException
                listenerEventV2OrchestrationServiceException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateListenerEventV2ClientServiceException(exception as Xeption);
            }
        }


        /// <summary>
        /// Removes a listener event by its identifier asynchronously by delegating to the
        /// orchestration service and handling any exceptions that occur.
        /// </summary>
        /// <param name="listenerEventV2Id">The identifier of the listener event to
        /// remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{ListenerEventV2}"/> representing the asynchronous
        /// operation that returns the removed listener event.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during removal.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.listenerEventV2OrchestrationService
                    .RemoveListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken);
            }
            catch (ListenerEventV2OrchestrationValidationException
                listenerEventV2OrchestrationValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyValidationException
                listenerEventV2OrchestrationDependencyValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyException
                listenerEventV2OrchestrationDependencyException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationServiceException
                listenerEventV2OrchestrationServiceException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateListenerEventV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Retries all failed listener events asynchronously by delegating to the retrying
        /// orchestration service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous retry
        /// operation.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during the retry sweep.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask RetryFailedListenerEventV2sAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                await this.retryingListenerEventV2OrchestrationService
                    .RetryFailedListenerEventV2sAsync(cancellationToken);
            }
            catch (RetryingListenerEventV2OrchestrationValidationException
                retryingListenerEventV2OrchestrationValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    retryingListenerEventV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (RetryingListenerEventV2OrchestrationDependencyValidationException
                retryingListenerEventV2OrchestrationDependencyValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    retryingListenerEventV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (RetryingListenerEventV2OrchestrationDependencyException
                retryingListenerEventV2OrchestrationDependencyException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    retryingListenerEventV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (RetryingListenerEventV2OrchestrationServiceException
                retryingListenerEventV2OrchestrationServiceException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    retryingListenerEventV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateListenerEventV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Resets the retry attempts for a listener event by its identifier asynchronously by
        /// delegating to the orchestration service and handling any exceptions that occur.
        /// </summary>
        /// <param name="listenerEventV2Id">The identifier of the listener event to reset
        /// retries for.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{ListenerEventV2}"/> representing the asynchronous
        /// operation that returns the listener event with reset retries.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during the reset.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<ListenerEventV2> ResetRetriesForListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.listenerEventV2OrchestrationService
                    .ResetRetriesForListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken);
            }
            catch (ListenerEventV2OrchestrationValidationException
                listenerEventV2OrchestrationValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyValidationException
                listenerEventV2OrchestrationDependencyValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyException
                listenerEventV2OrchestrationDependencyException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationServiceException
                listenerEventV2OrchestrationServiceException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateListenerEventV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Resets the retry attempts for all listener events belonging to an event listener by
        /// its identifier asynchronously by delegating to the orchestration service and
        /// handling any exceptions that occur.
        /// </summary>
        /// <param name="eventListenerV2Id">The identifier of the event listener whose listener
        /// events should have their retries reset.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous reset
        /// operation.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during the reset.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await this.listenerEventV2OrchestrationService
                    .ResetRetriesForListenerEventV2ByEventListenerV2IdAsync(
                        eventListenerV2Id, cancellationToken);
            }
            catch (ListenerEventV2OrchestrationValidationException
                listenerEventV2OrchestrationValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyValidationException
                listenerEventV2OrchestrationDependencyValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    listenerEventV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationDependencyException
                listenerEventV2OrchestrationDependencyException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (ListenerEventV2OrchestrationServiceException
                listenerEventV2OrchestrationServiceException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    listenerEventV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateListenerEventV2ClientServiceException(exception as Xeption);
            }
        }

        private static ListenerEventV2ClientValidationException
            CreateListenerEventV2ClientValidationException(Xeption innerException)
        {
            return new ListenerEventV2ClientValidationException(
                message: "Listener event client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ListenerEventV2ClientDependencyException
            CreateListenerEventV2ClientDependencyException(Xeption innerException)
        {
            return new ListenerEventV2ClientDependencyException(
                message: "Listener event client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ListenerEventV2ClientServiceException
            CreateListenerEventV2ClientServiceException(Xeption innerException)
        {
            return new ListenerEventV2ClientServiceException(
                message: "Listener event client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
