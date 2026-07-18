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
using Microsoft.Extensions.DependencyInjection;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health address client implementation, handling per-event-address
    /// summary retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthAddressClientV2 : IHealthAddressClientV2
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthAddressClientV2"/> class with the
        /// specified health coordination service.
        /// </summary>
        /// <param name="serviceProvider">The application service provider used to open a fresh scope per operation.</param>
        public HealthAddressClientV2(IServiceProvider serviceProvider) =>
            this.serviceScopeFactory =
                serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public async ValueTask<IReadOnlyList<EventAddressUsageV2>> RetrieveEventAddressSummaryV2Async(
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
                    .RetrieveAddressUsageReportV2Async(period, windowStart, windowEnd, cancellationToken);

                return healthReport.AddressUsage;
            }
            catch (HealthV2CoordinationValidationException
                healthV2CoordinationValidationException)
            {
                throw CreateHealthAddressClientV2ValidationException(
                    healthV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyValidationException
                healthV2CoordinationDependencyValidationException)
            {
                throw CreateHealthAddressClientV2ValidationException(
                    healthV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyException
                healthV2CoordinationDependencyException)
            {
                throw CreateHealthAddressClientV2DependencyException(
                    healthV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationServiceException
                healthV2CoordinationServiceException)
            {
                throw CreateHealthAddressClientV2DependencyException(
                    healthV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthAddressClientV2ServiceException(exception);
            }
        }

        private static HealthAddressClientV2ValidationException
            CreateHealthAddressClientV2ValidationException(Xeption innerException)
        {
            return new HealthAddressClientV2ValidationException(
                message: "Health client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthAddressClientV2DependencyException
            CreateHealthAddressClientV2DependencyException(Xeption innerException)
        {
            return new HealthAddressClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthAddressClientV2ServiceException
            CreateHealthAddressClientV2ServiceException(Exception exception)
        {
            Xeption innerException = exception as Xeption
                ?? new Xeption(exception?.Message, exception);

            return new HealthAddressClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: exception?.Data);
        }
    }
}
