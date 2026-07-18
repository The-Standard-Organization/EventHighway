// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthStatusClientV2Tests
    {
        private readonly Mock<IHealthV2CoordinationService> healthV2CoordinationServiceMock;
        private int coordinationServiceResolutionCount;
        private readonly IHealthStatusClientV2 healthV2Client;

        public HealthStatusClientV2Tests()
        {
            this.healthV2CoordinationServiceMock =
                new Mock<IHealthV2CoordinationService>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped(_ =>
            {
                this.coordinationServiceResolutionCount++;

                return this.healthV2CoordinationServiceMock.Object;
            });

            this.healthV2Client =
                new HealthStatusClientV2(
                    serviceProvider: serviceCollection.BuildServiceProvider());
        }

        public static TheoryData<Xeption> ClientDependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new HealthV2CoordinationDependencyException(someMessage, someInnerException),
                new HealthV2CoordinationServiceException(someMessage, someInnerException)
            };
        }

        public static TheoryData<Xeption> ClientValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new HealthV2CoordinationValidationException(someMessage, someInnerException),
                new HealthV2CoordinationDependencyValidationException(someMessage, someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static TrafficPeriodV2 GetRandomTrafficPeriod()
        {
            TrafficPeriodV2[] periods = Enum.GetValues<TrafficPeriodV2>();

            return periods[new IntRange(min: 0, max: periods.Length - 1).GetValue()];
        }

        private static IReadOnlyList<HealthCheckItemV2> CreateRandomHealthCheckItemV2s() =>
            Enumerable.Range(0, new IntRange(min: 2, max: 9).GetValue())
                .Select(_ => CreateRandomHealthCheckItemV2())
                    .ToList();

        private static HealthCheckItemV2 CreateRandomHealthCheckItemV2() =>
            new HealthCheckItemV2
            {
                Grouping = GetRandomString(),
                Item = GetRandomString(),
                Value = GetRandomString(),
                Description = GetRandomString(),
                StatusCode = 0,
                Status = HealthStatusV2.NA.ToString()
            };
    }
}
