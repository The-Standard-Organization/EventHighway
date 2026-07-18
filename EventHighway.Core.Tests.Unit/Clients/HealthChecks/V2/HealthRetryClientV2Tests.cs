// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
    public partial class HealthRetryClientV2Tests
    {
        private readonly Mock<IHealthV2CoordinationService> healthV2CoordinationServiceMock;
        private int coordinationServiceResolutionCount;
        private readonly IHealthRetryClientV2 healthRetryClientV2;

        public HealthRetryClientV2Tests()
        {
            this.healthV2CoordinationServiceMock =
                new Mock<IHealthV2CoordinationService>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped(_ =>
            {
                this.coordinationServiceResolutionCount++;

                return this.healthV2CoordinationServiceMock.Object;
            });

            this.healthRetryClientV2 =
                new HealthRetryClientV2(
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

        private static TrafficPeriodV2 GetRandomTrafficPeriodV2() =>
            TrafficPeriodV2.Day;

        private static RetryHealthSummaryV2 CreateRandomRetryHealthSummaryV2() =>
            new RetryHealthSummaryV2
            {
                TotalActiveEvents = new IntRange(min: 2, max: 9).GetValue(),
                DeadEvents = new IntRange(min: 0, max: 5).GetValue(),
                CriticalEvents = new IntRange(min: 0, max: 5).GetValue(),
                HealthyEvents = new IntRange(min: 0, max: 5).GetValue(),
                Distribution = new List<RetryBucketV2>(),
                ByAddress = new List<RetryAddressDetailV2>()
            };
    }
}
