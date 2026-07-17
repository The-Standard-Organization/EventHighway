// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventV2sWithEventAddressV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> randomEventV2s =
                CreateRandomEventV2s();

            IQueryable<EventV2> retrievedEventV2s =
                randomEventV2s;

            IQueryable<EventV2> expectedEventV2s =
                retrievedEventV2s.DeepClone();

            this.eventV2CoordinationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithEventAddressV2Async(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IReadOnlyList<EventV2> actualEventV2s =
                await this.eventV2Client
                    .RetrieveAllEventV2sWithEventAddressV2Async(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(
                expectedEventV2s);

            this.eventV2CoordinationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithEventAddressV2Async(randomCancellationToken),
                    Times.Once);

            this.eventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
