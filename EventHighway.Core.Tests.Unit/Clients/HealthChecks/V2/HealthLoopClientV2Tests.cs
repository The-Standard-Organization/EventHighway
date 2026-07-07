// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthLoopClientV2Tests
    {
        private readonly Mock<IHealthV2CoordinationService> healthV2CoordinationServiceMock;
        private readonly IHealthLoopClientV2 healthLoopClientV2;

        public HealthLoopClientV2Tests()
        {
            this.healthV2CoordinationServiceMock =
                new Mock<IHealthV2CoordinationService>();

            this.healthLoopClientV2 =
                new HealthLoopClientV2(
                    healthV2CoordinationService:
                        this.healthV2CoordinationServiceMock.Object);
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

        private static LoopDetectionSummaryV2 CreateRandomLoopDetectionSummaryV2() =>
            new LoopDetectionSummaryV2
            {
                Period = GetRandomTrafficPeriodV2(),
                WindowStart = GetRandomDateTimeOffset(),
                WindowEnd = GetRandomDateTimeOffset(),
                WindowLabel = GetRandomString(),
                TotalActiveQuarantined = new IntRange(min: 2, max: 9).GetValue(),
                ByAddress = new List<LoopDetailV2>()
            };
    }
}
