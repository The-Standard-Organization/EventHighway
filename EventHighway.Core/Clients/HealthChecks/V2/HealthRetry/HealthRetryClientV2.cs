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
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health retry client implementation, handling retry-health summary
    /// retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthRetryClientV2 : IHealthRetryClientV2
    {
        private readonly IHealthV2CoordinationService healthV2CoordinationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthRetryClientV2"/> class with the
        /// specified health coordination service.
        /// </summary>
        /// <param name="healthV2CoordinationService">The coordination service for health
        /// reports.</param>
        public HealthRetryClientV2(IHealthV2CoordinationService healthV2CoordinationService) =>
            this.healthV2CoordinationService = healthV2CoordinationService;

        public async ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset? windowEnd = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                HealthReportV2 healthReport = await this.healthV2CoordinationService
                    .RetrieveRetryReportV2Async(period, windowStart, cancellationToken: cancellationToken);

                return healthReport.Retry;
            }
            catch (HealthV2CoordinationValidationException
                healthV2CoordinationValidationException)
            {
                throw CreateHealthRetryClientV2ValidationException(
                    healthV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyValidationException
                healthV2CoordinationDependencyValidationException)
            {
                throw CreateHealthRetryClientV2ValidationException(
                    healthV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyException
                healthV2CoordinationDependencyException)
            {
                throw CreateHealthRetryClientV2DependencyException(
                    healthV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationServiceException
                healthV2CoordinationServiceException)
            {
                throw CreateHealthRetryClientV2DependencyException(
                    healthV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthRetryClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthRetryClientV2ValidationException
            CreateHealthRetryClientV2ValidationException(Xeption innerException)
        {
            return new HealthRetryClientV2ValidationException(
                message: "Health client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthRetryClientV2DependencyException
            CreateHealthRetryClientV2DependencyException(Xeption innerException)
        {
            return new HealthRetryClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthRetryClientV2ServiceException
            CreateHealthRetryClientV2ServiceException(Xeption innerException)
        {
            return new HealthRetryClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
