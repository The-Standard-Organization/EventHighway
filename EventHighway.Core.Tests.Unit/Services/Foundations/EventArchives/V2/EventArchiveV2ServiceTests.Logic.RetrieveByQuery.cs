// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventArchiveV2sByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid targetEventAddressV2Id = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();

            List<EventArchiveV2> matchingEventArchiveV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    EventArchiveV2 matchingEventArchiveV2 = CreateRandomEventArchiveV2();
                    matchingEventArchiveV2.EventAddressV2Id = targetEventAddressV2Id;
                    matchingEventArchiveV2.Status = EventArchiveStatusV2.Active;
                    matchingEventArchiveV2.ArchivedDate = baseDateTimeOffset.AddMinutes(-index);

                    return matchingEventArchiveV2;
                }).ToList();

            EventArchiveV2 differentAddressEventArchiveV2 = CreateRandomEventArchiveV2();

            EventArchiveV2 quarantinedEventArchiveV2 = CreateRandomEventArchiveV2();
            quarantinedEventArchiveV2.EventAddressV2Id = targetEventAddressV2Id;
            quarantinedEventArchiveV2.Status = EventArchiveStatusV2.Quarantined;

            IQueryable<EventArchiveV2> allEventArchiveV2s = matchingEventArchiveV2s
                .Append(differentAddressEventArchiveV2)
                .Append(quarantinedEventArchiveV2)
                .AsQueryable();

            var inputEventArchiveV2Query = new EventArchiveV2Query
            {
                EventAddressV2Id = targetEventAddressV2Id,
                Status = EventArchiveStatusV2.Active,
                Skip = 1,
                Take = 2
            };

            List<EventArchiveV2> expectedEventArchiveV2s = matchingEventArchiveV2s
                .OrderByDescending(eventArchiveV2 => eventArchiveV2.ArchivedDate)
                .ThenBy(eventArchiveV2 => eventArchiveV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allEventArchiveV2s);

            // when
            IReadOnlyList<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2Service.RetrieveEventArchiveV2sByQueryAsync(
                    inputEventArchiveV2Query, randomCancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s, options =>
                options.WithStrictOrdering());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid targetEventAddressV2Id = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();

            List<EventArchiveV2> matchingEventArchiveV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    EventArchiveV2 matchingEventArchiveV2 = CreateRandomEventArchiveV2();
                    matchingEventArchiveV2.EventAddressV2Id = targetEventAddressV2Id;
                    matchingEventArchiveV2.Status = EventArchiveStatusV2.Active;
                    matchingEventArchiveV2.ArchivedDate = baseDateTimeOffset.AddMinutes(-index);

                    return matchingEventArchiveV2;
                }).ToList();

            EventArchiveV2 differentAddressEventArchiveV2 = CreateRandomEventArchiveV2();

            EventArchiveV2 quarantinedEventArchiveV2 = CreateRandomEventArchiveV2();
            quarantinedEventArchiveV2.EventAddressV2Id = targetEventAddressV2Id;
            quarantinedEventArchiveV2.Status = EventArchiveStatusV2.Quarantined;

            IQueryable<EventArchiveV2> allEventArchiveV2s = matchingEventArchiveV2s
                .Append(differentAddressEventArchiveV2)
                .Append(quarantinedEventArchiveV2)
                .AsQueryable();

            var inputEventArchiveV2Query = new EventArchiveV2Query
            {
                EventAddressV2Id = targetEventAddressV2Id,
                Status = EventArchiveStatusV2.Active,
                Skip = 1,
                Take = 2
            };

            List<EventArchiveV2> expectedEventArchiveV2s = matchingEventArchiveV2s
                .OrderByDescending(eventArchiveV2 => eventArchiveV2.ArchivedDate)
                .ThenBy(eventArchiveV2 => eventArchiveV2.Id)
                .Skip(1)
                .Take(2)
                .ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV2sWithEventAddressV2Async(randomCancellationToken))
                    .ReturnsAsync(allEventArchiveV2s);

            // when
            IReadOnlyList<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2Service
                    .RetrieveEventArchiveV2sWithEventAddressV2ByQueryAsync(
                        inputEventArchiveV2Query, randomCancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s, options =>
                options.WithStrictOrdering());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sWithEventAddressV2Async(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
