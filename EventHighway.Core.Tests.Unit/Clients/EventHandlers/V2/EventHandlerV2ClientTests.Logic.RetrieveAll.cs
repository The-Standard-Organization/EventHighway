// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventHandlers.V2
{
    public partial class EventHandlerV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventHandlerV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventHandlerV2> randomEventHandlerV2s = CreateRandomEventHandlerV2s();
            IQueryable<EventHandlerV2> retrievedEventHandlerV2s = randomEventHandlerV2s;
            IQueryable<EventHandlerV2> expectedEventHandlerV2s = retrievedEventHandlerV2s;

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventHandlerV2s);

            // when
            IQueryable<EventHandlerV2> actualEventHandlerV2s =
                await this.eventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                    randomCancellationToken);

            // then
            actualEventHandlerV2s.Should().BeEquivalentTo(expectedEventHandlerV2s);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
