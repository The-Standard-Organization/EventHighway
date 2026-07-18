// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveListenerEventArchiveV2sWithEventListenerV2ByQueryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid targetEventAddressV2Id = GetRandomId();
            DateTimeOffset baseDateTimeOffset = GetRandomDateTimeOffset();

            List<ListenerEventArchiveV2> matchingListenerEventArchiveV2s =
                Enumerable.Range(start: 0, count: 4).Select(index =>
                {
                    ListenerEventArchiveV2 matchingListenerEventArchiveV2 =
                        CreateRandomListenerEventArchiveV2();

                    matchingListenerEventArchiveV2.EventAddressV2Id = targetEventAddressV2Id;
                    matchingListenerEventArchiveV2.Status = ListenerEventArchiveStatusV2.Success;

                    matchingListenerEventArchiveV2.ArchivedDate =
                        baseDateTimeOffset.AddMinutes(-index);

                    return matchingListenerEventArchiveV2;
                }).ToList();

            ListenerEventArchiveV2 differentAddressListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2();

            ListenerEventArchiveV2 erroredListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2();

            erroredListenerEventArchiveV2.EventAddressV2Id = targetEventAddressV2Id;
            erroredListenerEventArchiveV2.Status = ListenerEventArchiveStatusV2.Error;

            IQueryable<ListenerEventArchiveV2> allListenerEventArchiveV2s =
                matchingListenerEventArchiveV2s
                    .Append(differentAddressListenerEventArchiveV2)
                    .Append(erroredListenerEventArchiveV2)
                    .AsQueryable();

            var inputListenerEventArchiveV2Query = new ListenerEventArchiveV2Query
            {
                EventAddressV2Id = targetEventAddressV2Id,
                Status = ListenerEventArchiveStatusV2.Success,
                Skip = 1,
                Take = 2
            };

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                matchingListenerEventArchiveV2s
                    .OrderByDescending(listenerEventArchiveV2 => listenerEventArchiveV2.ArchivedDate)
                    .ThenBy(listenerEventArchiveV2 => listenerEventArchiveV2.Id)
                    .Skip(1)
                    .Take(2)
                    .ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventArchiveV2sWithEventListenerV2Async(randomCancellationToken))
                    .ReturnsAsync(allListenerEventArchiveV2s);

            // when
            IReadOnlyList<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2Service
                    .RetrieveListenerEventArchiveV2sWithEventListenerV2ByQueryAsync(
                        inputListenerEventArchiveV2Query, randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2s, options => options.WithStrictOrdering());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventArchiveV2sWithEventListenerV2Async(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
