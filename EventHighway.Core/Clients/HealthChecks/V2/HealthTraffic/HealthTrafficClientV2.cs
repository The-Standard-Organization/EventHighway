// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health traffic client implementation, handling traffic snapshot
    /// retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthTrafficClientV2 : IHealthTrafficClientV2
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthTrafficClientV2"/> class with the
        /// specified health coordination service.
        /// </summary>
        /// <param name="serviceProvider">The application service provider used to open a fresh scope per operation.</param>
        public HealthTrafficClientV2(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public async ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope serviceScope =
                this.serviceScopeFactory.CreateAsyncScope();

            IHealthV2CoordinationService healthV2CoordinationService =
                serviceScope.ServiceProvider
                    .GetRequiredService<IHealthV2CoordinationService>();

            try
            {
                HealthReportV2 healthReport = await healthV2CoordinationService
                    .RetrieveTrafficReportV2Async(period, windowStart, windowEnd, cancellationToken);

                return healthReport.Traffic;
            }
            catch (HealthV2CoordinationValidationException
                healthV2CoordinationValidationException)
            {
                throw CreateHealthTrafficClientV2ValidationException(
                    healthV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyValidationException
                healthV2CoordinationDependencyValidationException)
            {
                throw CreateHealthTrafficClientV2ValidationException(
                    healthV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyException
                healthV2CoordinationDependencyException)
            {
                throw CreateHealthTrafficClientV2DependencyException(
                    healthV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationServiceException
                healthV2CoordinationServiceException)
            {
                throw CreateHealthTrafficClientV2DependencyException(
                    healthV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthTrafficClientV2ServiceException(exception);
            }
        }

        private static HealthTrafficClientV2ValidationException
            CreateHealthTrafficClientV2ValidationException(Xeption innerException)
        {
            return new HealthTrafficClientV2ValidationException(
                message: "Health client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthTrafficClientV2DependencyException
            CreateHealthTrafficClientV2DependencyException(Xeption innerException)
        {
            return new HealthTrafficClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthTrafficClientV2ServiceException
            CreateHealthTrafficClientV2ServiceException(Exception exception)
        {
            Xeption innerException = exception as Xeption
                ?? new Xeption(exception?.Message, exception);

            return new HealthTrafficClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: exception?.Data);
        }
    }
}
