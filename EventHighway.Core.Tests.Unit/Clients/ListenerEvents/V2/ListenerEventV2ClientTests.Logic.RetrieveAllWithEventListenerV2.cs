// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventV2sWithEventListenerV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventV2> randomListenerEventV2s =
                CreateRandomListenerEventV2s();

            IReadOnlyList<ListenerEventV2> retrievedListenerEventV2s =
                randomListenerEventV2s.ToList();

            IReadOnlyList<ListenerEventV2> expectedListenerEventV2s =
                retrievedListenerEventV2s.DeepClone();

            var inputListenerEventV2Query = new ListenerEventV2Query();

            this.listenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveListenerEventV2sWithEventListenerV2ByQueryAsync(
                    inputListenerEventV2Query, randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IReadOnlyList<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Client
                    .RetrieveAllListenerEventV2sWithEventListenerV2Async(
                        inputListenerEventV2Query, randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.listenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sWithEventListenerV2ByQueryAsync(
                    inputListenerEventV2Query, randomCancellationToken),
                        Times.Once);

            this.listenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
