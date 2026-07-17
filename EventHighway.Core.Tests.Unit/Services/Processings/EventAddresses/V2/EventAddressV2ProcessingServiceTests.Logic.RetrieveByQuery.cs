// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventAddresses.V2
{
    public partial class EventAddressV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventAddressV2sByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string targetName = GetRandomString();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset createdFrom = baseDateTimeOffset;
            DateTimeOffset createdTo = baseDateTimeOffset.AddHours(1);

            List<EventAddressV2> matchingEventAddressV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    EventAddressV2 matchingEventAddressV2 = CreateRandomEventAddressV2();
                    matchingEventAddressV2.Name = targetName;
                    matchingEventAddressV2.CreatedDate = createdFrom.AddMinutes(index);

                    return matchingEventAddressV2;
                }).ToList();

            EventAddressV2 differentNameEventAddressV2 = CreateRandomEventAddressV2();
            differentNameEventAddressV2.CreatedDate = createdFrom.AddMinutes(5);

            EventAddressV2 createdBeforeWindowEventAddressV2 = CreateRandomEventAddressV2();
            createdBeforeWindowEventAddressV2.Name = targetName;
            createdBeforeWindowEventAddressV2.CreatedDate = createdFrom.AddMinutes(-5);

            EventAddressV2 createdAfterWindowEventAddressV2 = CreateRandomEventAddressV2();
            createdAfterWindowEventAddressV2.Name = targetName;
            createdAfterWindowEventAddressV2.CreatedDate = createdTo.AddMinutes(5);

            IQueryable<EventAddressV2> allEventAddressV2s = matchingEventAddressV2s
                .Append(differentNameEventAddressV2)
                .Append(createdBeforeWindowEventAddressV2)
                .Append(createdAfterWindowEventAddressV2)
                .AsQueryable();

            var inputEventAddressV2Query = new EventAddressV2Query
            {
                Name = targetName,
                CreatedFrom = createdFrom,
                CreatedTo = createdTo,
                Skip = 1,
                Take = 2
            };

            List<EventAddressV2> expectedEventAddressV2s = matchingEventAddressV2s
                .OrderByDescending(eventAddressV2 => eventAddressV2.CreatedDate)
                .ThenBy(eventAddressV2 => eventAddressV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allEventAddressV2s);

            // when
            IReadOnlyList<EventAddressV2> actualEventAddressV2s =
                await this.eventAddressV2ProcessingService.RetrieveEventAddressV2sByQueryAsync(
                    inputEventAddressV2Query, randomCancellationToken);

            // then
            actualEventAddressV2s.Should().BeEquivalentTo(expectedEventAddressV2s, options =>
                options.WithStrictOrdering());

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
