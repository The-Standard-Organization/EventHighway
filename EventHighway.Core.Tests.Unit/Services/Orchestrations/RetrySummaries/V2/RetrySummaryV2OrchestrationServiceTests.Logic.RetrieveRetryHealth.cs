// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RetrySummaries.V2
{
    public partial class RetrySummaryV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveRetryHealthV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid addressAId = Guid.NewGuid();
            Guid addressBId = Guid.NewGuid();
            string addressAName = GetRandomString();
            string addressBName = GetRandomString();
            DateTimeOffset anyDate = GetRandomDateTimeOffset();

            var addresses = new[]
            {
                CreateEventAddressV2(addressAId, addressAName, GetRandomString()),
                CreateEventAddressV2(addressBId, addressBName, GetRandomString())
            }.AsQueryable();

            var events = new[]
            {
                CreateEventV2ForAddress(addressAId, anyDate, "h1", 0, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, anyDate, "h2", 0, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, anyDate, "h3", 1, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, anyDate, "h4", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressBId, anyDate, "h5", 2, EventStatusV2.Active),
                CreateEventV2ForAddress(addressBId, anyDate, "h6", 4, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, anyDate, "h7", 3, EventStatusV2.Quarantined)
            }.AsQueryable();

            SetupRetryFoundationMocks(randomCancellationToken, addresses, events);

            // when
            RetryHealthSummaryV2 actualSummary =
                await this.retrySummaryV2OrchestrationService
                    .RetrieveRetryHealthV2Async(randomCancellationToken);

            // then
            actualSummary.TotalActiveEvents.Should().Be(6);
            actualSummary.DeadEvents.Should().Be(0);
            actualSummary.CriticalEvents.Should().Be(0);
            actualSummary.HealthyEvents.Should().Be(0);

            actualSummary.Distribution.Should().BeEmpty();

            actualSummary.ByAddress.Should().HaveCount(2);
            actualSummary.ByAddress.First().EventAddressV2Id.Should().Be(addressAId);

            RetryAddressDetailV2 detailA =
                actualSummary.ByAddress.Single(d => d.EventAddressV2Id == addressAId);

            detailA.EventAddressV2Name.Should().Be(addressAName);
            detailA.DeadEvents.Should().Be(0);
            detailA.CriticalEvents.Should().Be(0);
            detailA.TotalEvents.Should().Be(4);

            RetryAddressDetailV2 detailB =
                actualSummary.ByAddress.Single(d => d.EventAddressV2Id == addressBId);

            detailB.DeadEvents.Should().Be(0);
            detailB.CriticalEvents.Should().Be(0);
            detailB.TotalEvents.Should().Be(2);

            VerifyRetryFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnEmptyRetryHealthV2WhenNoActiveEventsExistAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid addressAId = Guid.NewGuid();
            DateTimeOffset anyDate = GetRandomDateTimeOffset();

            var addresses = new[]
            {
                CreateEventAddressV2(addressAId, GetRandomString(), GetRandomString())
            }.AsQueryable();

            var events = new[]
            {
                CreateEventV2ForAddress(addressAId, anyDate, "h1", 1, EventStatusV2.Quarantined)
            }.AsQueryable();

            SetupRetryFoundationMocks(randomCancellationToken, addresses, events);

            // when
            RetryHealthSummaryV2 actualSummary =
                await this.retrySummaryV2OrchestrationService
                    .RetrieveRetryHealthV2Async(randomCancellationToken);

            // then
            actualSummary.TotalActiveEvents.Should().Be(0);
            actualSummary.DeadEvents.Should().Be(0);
            actualSummary.CriticalEvents.Should().Be(0);
            actualSummary.HealthyEvents.Should().Be(0);
            actualSummary.Distribution.Should().BeEmpty();
            actualSummary.ByAddress.Should().BeEmpty();

            VerifyRetryFoundationMocksOnce(randomCancellationToken);
        }
    }
}
