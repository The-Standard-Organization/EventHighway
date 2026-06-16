// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V2
{
    public partial class EventAddressV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventAddressV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventAddressV2> randomEventAddressV2s =
                CreateRandomEventAddressV2s();

            IQueryable<EventAddressV2> retrievedEventAddressV2s =
                randomEventAddressV2s;

            IQueryable<EventAddressV2> expectedEventAddressV2s =
                retrievedEventAddressV2s.DeepClone();

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync())
                    .ReturnsAsync(retrievedEventAddressV2s);

            // when
            IQueryable<EventAddressV2> actualEventAddressV2s =
                await this.eventAddressV2Client
                    .RetrieveAllEventAddressV2sAsync(randomCancellationToken);

            // then
            actualEventAddressV2s.Should().BeEquivalentTo(
                expectedEventAddressV2s);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(),
                    Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
