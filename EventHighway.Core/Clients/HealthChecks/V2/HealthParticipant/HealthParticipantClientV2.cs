// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
    /// Represents the V2 health participant client implementation, handling per-participant
    /// summary retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthParticipantClientV2 : IHealthParticipantClientV2
    {
        private readonly IHealthV2CoordinationService healthV2CoordinationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthParticipantClientV2"/> class with the
        /// specified health coordination service.
        /// </summary>
        /// <param name="healthV2CoordinationService">The coordination service for health
        /// reports.</param>
        public HealthParticipantClientV2(IHealthV2CoordinationService healthV2CoordinationService) =>
            this.healthV2CoordinationService = healthV2CoordinationService;

        public async ValueTask<IReadOnlyList<ParticipantUsageV2>> RetrieveParticipantSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            try
            {
                HealthReportV2 healthReport = await this.healthV2CoordinationService
                    .RetrieveParticipantUsageReportV2Async(period, windowStart, cancellationToken);

                return healthReport.ParticipantUsage;
            }
            catch (HealthV2CoordinationValidationException
                healthV2CoordinationValidationException)
            {
                throw CreateHealthParticipantClientV2ValidationException(
                    healthV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyValidationException
                healthV2CoordinationDependencyValidationException)
            {
                throw CreateHealthParticipantClientV2ValidationException(
                    healthV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyException
                healthV2CoordinationDependencyException)
            {
                throw CreateHealthParticipantClientV2DependencyException(
                    healthV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationServiceException
                healthV2CoordinationServiceException)
            {
                throw CreateHealthParticipantClientV2DependencyException(
                    healthV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthParticipantClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthParticipantClientV2ValidationException
            CreateHealthParticipantClientV2ValidationException(Xeption innerException)
        {
            return new HealthParticipantClientV2ValidationException(
                message: "Health client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthParticipantClientV2DependencyException
            CreateHealthParticipantClientV2DependencyException(Xeption innerException)
        {
            return new HealthParticipantClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthParticipantClientV2ServiceException
            CreateHealthParticipantClientV2ServiceException(Xeption innerException)
        {
            return new HealthParticipantClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
