// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventArchiveV2sWithEventListenerV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            IReadOnlyList<ListenerEventArchiveV2> retrievedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s.ToList();

            IReadOnlyList<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                retrievedListenerEventArchiveV2s.DeepClone();

            var inputListenerEventArchiveV2Query = new ListenerEventArchiveV2Query();

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventArchiveV2sWithEventListenerV2ByQueryAsync(
                    inputListenerEventArchiveV2Query, randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventArchiveV2s);

            // when
            IReadOnlyList<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2Client
                    .RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                        inputListenerEventArchiveV2Query, randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventArchiveV2sWithEventListenerV2ByQueryAsync(
                    inputListenerEventArchiveV2Query, randomCancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
