// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2;
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

            IReadOnlyList<EventAddressV2> retrievedEventAddressV2s =
                randomEventAddressV2s.ToList();

            IReadOnlyList<EventAddressV2> expectedEventAddressV2s =
                retrievedEventAddressV2s.DeepClone();

            var inputEventAddressV2Query = new EventAddressV2Query();

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    inputEventAddressV2Query, randomCancellationToken))
                        .ReturnsAsync(retrievedEventAddressV2s);

            // when
            IReadOnlyList<EventAddressV2> actualEventAddressV2s =
                await this.eventAddressV2Client.RetrieveAllEventAddressV2sAsync(
                    inputEventAddressV2Query, randomCancellationToken);

            // then
            actualEventAddressV2s.Should().BeEquivalentTo(
                expectedEventAddressV2s);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2sByQueryAsync(
                    inputEventAddressV2Query, randomCancellationToken),
                        Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
