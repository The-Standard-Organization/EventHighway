// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.ReplayingEvents.V2;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.ReplayingEvents.V2
{
    internal class ReplayingEventV2Client : IReplayingEventV2Client
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ReplayingEventV2Client(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        /// <inheritdoc/>
        public async ValueTask ReplayEventArchiveV2sAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IReplayingEventV2CoordinationService replayingEventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IReplayingEventV2CoordinationService>();

            try
            {
                await replayingEventV2CoordinationService
                    .ReplayEventArchiveV2sAsync(
                        eventAddressId, eventListenerIds, startDate, endDate, cancellationToken);
            }
            catch (ReplayingEventV2CoordinationValidationException
                replayingEventV2CoordinationValidationException)
            {
                throw CreateReplayingEventV2ClientValidationException(
                    replayingEventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (ReplayingEventV2CoordinationDependencyValidationException
                replayingEventV2CoordinationDependencyValidationException)
            {
                throw CreateReplayingEventV2ClientValidationException(
                    replayingEventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (ReplayingEventV2CoordinationDependencyException
                replayingEventV2CoordinationDependencyException)
            {
                throw CreateReplayingEventV2ClientDependencyException(
                    replayingEventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (ReplayingEventV2CoordinationServiceException
                replayingEventV2CoordinationServiceException)
            {
                throw CreateReplayingEventV2ClientDependencyException(
                    replayingEventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateReplayingEventV2ClientServiceException(exception as Xeption);
            }
        }

        /// <inheritdoc/>
        public async ValueTask ReplayEventArchiveV2sAsync(
            Guid eventV2Id,
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            bool allowReplayOfQuarantinedItem = false,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IReplayingEventV2CoordinationService replayingEventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IReplayingEventV2CoordinationService>();

            try
            {
                await replayingEventV2CoordinationService
                    .ReplayEventArchiveV2sAsync(
                        eventV2Id,
                        eventAddressId,
                        eventListenerIds,
                        allowReplayOfQuarantinedItem,
                        cancellationToken);
            }
            catch (ReplayingEventV2CoordinationValidationException
                replayingEventV2CoordinationValidationException)
            {
                throw CreateReplayingEventV2ClientValidationException(
                    replayingEventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (ReplayingEventV2CoordinationDependencyValidationException
                replayingEventV2CoordinationDependencyValidationException)
            {
                throw CreateReplayingEventV2ClientValidationException(
                    replayingEventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (ReplayingEventV2CoordinationDependencyException
                replayingEventV2CoordinationDependencyException)
            {
                throw CreateReplayingEventV2ClientDependencyException(
                    replayingEventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (ReplayingEventV2CoordinationServiceException
                replayingEventV2CoordinationServiceException)
            {
                throw CreateReplayingEventV2ClientDependencyException(
                    replayingEventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateReplayingEventV2ClientServiceException(exception as Xeption);
            }
        }

        /// <inheritdoc/>
        public async ValueTask ProcessReplayedListenerEventV2sAsync(
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IReplayingEventV2CoordinationService replayingEventV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IReplayingEventV2CoordinationService>();

            try
            {
                await replayingEventV2CoordinationService
                    .ProcessReplayedListenerEventV2sAsync(cancellationToken);
            }
            catch (ReplayingEventV2CoordinationValidationException
                replayingEventV2CoordinationValidationException)
            {
                throw CreateReplayingEventV2ClientValidationException(
                    replayingEventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (ReplayingEventV2CoordinationDependencyValidationException
                replayingEventV2CoordinationDependencyValidationException)
            {
                throw CreateReplayingEventV2ClientValidationException(
                    replayingEventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (ReplayingEventV2CoordinationDependencyException
                replayingEventV2CoordinationDependencyException)
            {
                throw CreateReplayingEventV2ClientDependencyException(
                    replayingEventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (ReplayingEventV2CoordinationServiceException
                replayingEventV2CoordinationServiceException)
            {
                throw CreateReplayingEventV2ClientDependencyException(
                    replayingEventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateReplayingEventV2ClientServiceException(exception as Xeption);
            }
        }

        private static ReplayingEventV2ClientValidationException
            CreateReplayingEventV2ClientValidationException(Xeption innerException)
        {
            return new ReplayingEventV2ClientValidationException(
                message: "Replaying event client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ReplayingEventV2ClientDependencyException
            CreateReplayingEventV2ClientDependencyException(Xeption innerException)
        {
            return new ReplayingEventV2ClientDependencyException(
                message: "Replaying event client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ReplayingEventV2ClientServiceException
            CreateReplayingEventV2ClientServiceException(Xeption innerException)
        {
            return new ReplayingEventV2ClientServiceException(
                message: "Replaying event client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
