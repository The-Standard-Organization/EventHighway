// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
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

            IReadOnlyList<EventHandlerV2> retrievedEventHandlerV2s =
                CreateRandomEventHandlerV2s().ToList();

            IReadOnlyList<EventHandlerV2> expectedEventHandlerV2s = retrievedEventHandlerV2s;

            var inputEventHandlerV2Query = new EventHandlerV2Query();

            this.eventHandlerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventHandlerV2sByQueryAsync(
                    inputEventHandlerV2Query, randomCancellationToken))
                        .ReturnsAsync(retrievedEventHandlerV2s);

            // when
            IReadOnlyList<EventHandlerV2> actualEventHandlerV2s =
                await this.eventHandlerV2Client.RetrieveAllEventHandlerV2sAsync(
                    inputEventHandlerV2Query, randomCancellationToken);

            // then
            actualEventHandlerV2s.Should().BeEquivalentTo(expectedEventHandlerV2s);

            this.eventHandlerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventHandlerV2sByQueryAsync(
                    inputEventHandlerV2Query, randomCancellationToken),
                        Times.Once);

            this.eventHandlerV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
