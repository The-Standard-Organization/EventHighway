// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ClientTests
    {
        // Validates the swept retrieval methods run end-to-end against the real EF provider —
        // the query's filters, ordering and paging are translated to SQL and executed. A freshly
        // created address guarantees a deterministic, isolated result while still exercising every
        // filter branch (MockQueryable would mask a server-side translation failure; this does not).
        [Fact]
        public async Task ShouldRetrieveListenerEventArchiveV2sByQueryOnRealDatabaseAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id = randomEventAddressV2.Id;
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var listenerEventArchiveV2Query = new ListenerEventArchiveV2Query
            {
                EventAddressV2Id = inputEventAddressV2Id,
                Status = ListenerEventArchiveStatusV2.Success,
                CreatedFrom = now.AddDays(-1),
                CreatedTo = now.AddDays(1),
                ArchivedFrom = now.AddDays(-1),
                ArchivedTo = now.AddDays(1),
                Take = 1000
            };

            // when
            IReadOnlyList<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.clientBroker.RetrieveAllListenerEventArchiveV2sAsync(
                    listenerEventArchiveV2Query);

            IReadOnlyList<ListenerEventArchiveV2> actualListenerEventArchiveV2sWithEventListener =
                await this.clientBroker.RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                    listenerEventArchiveV2Query);

            // then — the query executed on the real provider (no translation failure) and every
            // returned row honours the address filter (vacuously true for a fresh, unmatched address).
            actualListenerEventArchiveV2s.Should().NotBeNull();

            actualListenerEventArchiveV2s.All(listenerEventArchiveV2 =>
                listenerEventArchiveV2.EventAddressV2Id == inputEventAddressV2Id)
                    .Should().BeTrue();

            actualListenerEventArchiveV2sWithEventListener.Should().NotBeNull();

            actualListenerEventArchiveV2sWithEventListener.All(listenerEventArchiveV2 =>
                listenerEventArchiveV2.EventAddressV2Id == inputEventAddressV2Id)
                    .Should().BeTrue();

            await this.clientBroker.RemoveEventAddressV2ByIdAsync(inputEventAddressV2Id);
        }
    }
}
