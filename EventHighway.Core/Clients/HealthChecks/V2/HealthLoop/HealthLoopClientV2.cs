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
    /// Represents the V2 health loop client implementation, handling loop-detection summary
    /// retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthLoopClientV2 : IHealthLoopClientV2
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthLoopClientV2"/> class with the
        /// specified health coordination service.
        /// </summary>
        /// <param name="serviceProvider">The application service provider used to open a fresh scope per operation.</param>
        public HealthLoopClientV2(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public async ValueTask<LoopDetectionSummaryV2> RetrieveLoopDetectionSummaryV2Async(
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
                    .RetrieveLoopDetectionReportV2Async(period, windowStart, windowEnd, cancellationToken);

                return healthReport.LoopDetection;
            }
            catch (HealthV2CoordinationValidationException
                healthV2CoordinationValidationException)
            {
                throw CreateHealthLoopClientV2ValidationException(
                    healthV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyValidationException
                healthV2CoordinationDependencyValidationException)
            {
                throw CreateHealthLoopClientV2ValidationException(
                    healthV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyException
                healthV2CoordinationDependencyException)
            {
                throw CreateHealthLoopClientV2DependencyException(
                    healthV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationServiceException
                healthV2CoordinationServiceException)
            {
                throw CreateHealthLoopClientV2DependencyException(
                    healthV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthLoopClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthLoopClientV2ValidationException
            CreateHealthLoopClientV2ValidationException(Xeption innerException)
        {
            return new HealthLoopClientV2ValidationException(
                message: "Health client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthLoopClientV2DependencyException
            CreateHealthLoopClientV2DependencyException(Xeption innerException)
        {
            return new HealthLoopClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthLoopClientV2ServiceException
            CreateHealthLoopClientV2ServiceException(Xeption innerException)
        {
            return new HealthLoopClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
