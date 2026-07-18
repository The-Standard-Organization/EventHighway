// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.ArchivingEvents.V2;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.ArchivingEvents.V2
{
    /// <summary>
    /// Represents the V2 archiving event client implementation, handling the archival of dead
    /// events and managing coordination service exceptions.
    /// </summary>
    internal class ArchivingEventV2Client : IArchivingEventV2Client
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchivingEventV2Client"/> class with
        /// the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The application service provider used to open a fresh
        /// scope per operation.</param>
        public ArchivingEventV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        /// <summary>
        /// Archives dead events asynchronously by delegating to the coordination service and
        /// handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArchivingEventV2ClientValidationException">Thrown when validation
        /// errors occur in the coordination service.</exception>
        /// <exception cref="ArchivingEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur in the coordination service.</exception>
        /// <exception cref="ArchivingEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during archiving.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask ArchiveEventV2sAsync(CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IArchivingEventV2CoordinationService archivingEventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IArchivingEventV2CoordinationService>();

            try
            {
                await archivingEventV2CoordinationService
                    .ArchiveEventV2sAsync(cancellationToken);
            }
            catch (ArchivingEventV2CoordinationValidationException
                archivingEventV2CoordinationValidationException)
            {
                throw CreateArchivingEventV2ClientValidationException(
                    archivingEventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationDependencyValidationException
                archivingEventV2CoordinationDependencyValidationException)
            {
                throw CreateArchivingEventV2ClientValidationException(
                    archivingEventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationDependencyException
                archivingEventV2CoordinationDependencyException)
            {
                throw CreateArchivingEventV2ClientDependencyException(
                    archivingEventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationServiceException
                archivingEventV2CoordinationServiceException)
            {
                throw CreateArchivingEventV2ClientDependencyException(
                    archivingEventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateArchivingEventV2ClientServiceException(exception);
            }
        }

        private static ArchivingEventV2ClientValidationException
            CreateArchivingEventV2ClientValidationException(Xeption innerException)
        {
            return new ArchivingEventV2ClientValidationException(
                message: "Archiving event client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ArchivingEventV2ClientDependencyException
            CreateArchivingEventV2ClientDependencyException(Xeption innerException)
        {
            return new ArchivingEventV2ClientDependencyException(
                message: "Archiving event client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ArchivingEventV2ClientServiceException
            CreateArchivingEventV2ClientServiceException(Exception exception)
        {
            Xeption innerException = exception as Xeption
                ?? new Xeption(exception?.Message, exception);

            return new ArchivingEventV2ClientServiceException(
                message: "Archiving event client service error occurred, contact support.",
                innerException: innerException,
                data: exception?.Data);
        }

        /// <summary>
        /// Purges archived events older than the specified date asynchronously. This operation
        /// removes archived events that are older than the provided threshold date.
        /// </summary>
        /// <param name="olderThan">The date threshold. Events archived before this date will be
        /// purged.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask PurgeEventArchiveV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IArchivingEventV2CoordinationService archivingEventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IArchivingEventV2CoordinationService>();

            try
            {
                await archivingEventV2CoordinationService
                    .PurgeEventArchiveV2sAsync(olderThan, cancellationToken);
            }
            catch (ArchivingEventV2CoordinationValidationException
                archivingEventV2CoordinationValidationException)
            {
                throw CreateArchivingEventV2ClientValidationException(
                    archivingEventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationDependencyValidationException
                archivingEventV2CoordinationDependencyValidationException)
            {
                throw CreateArchivingEventV2ClientValidationException(
                    archivingEventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationDependencyException
                archivingEventV2CoordinationDependencyException)
            {
                throw CreateArchivingEventV2ClientDependencyException(
                    archivingEventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationServiceException
                archivingEventV2CoordinationServiceException)
            {
                throw CreateArchivingEventV2ClientDependencyException(
                    archivingEventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateArchivingEventV2ClientServiceException(exception);
            }
        }

        /// <summary>
        /// Purges archived events older than the configured retention window asynchronously by
        /// delegating to the coordination service and handling any exceptions that occur. The
        /// retention window is read from <c>EventHighwayConfiguration.Purging.RetentionDays</c>.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask PurgeEventArchiveV2sAsync(
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IArchivingEventV2CoordinationService archivingEventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IArchivingEventV2CoordinationService>();

            try
            {
                await archivingEventV2CoordinationService
                    .PurgeEventArchiveV2sAsync(cancellationToken);
            }
            catch (ArchivingEventV2CoordinationValidationException
                archivingEventV2CoordinationValidationException)
            {
                throw CreateArchivingEventV2ClientValidationException(
                    archivingEventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationDependencyValidationException
                archivingEventV2CoordinationDependencyValidationException)
            {
                throw CreateArchivingEventV2ClientValidationException(
                    archivingEventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationDependencyException
                archivingEventV2CoordinationDependencyException)
            {
                throw CreateArchivingEventV2ClientDependencyException(
                    archivingEventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationServiceException
                archivingEventV2CoordinationServiceException)
            {
                throw CreateArchivingEventV2ClientDependencyException(
                    archivingEventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateArchivingEventV2ClientServiceException(exception);
            }
        }
    }
}
