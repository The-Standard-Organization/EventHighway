// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventHandlers;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.Foundations.EventHandlers
{
    public partial class EventHandlersViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventHandlersAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventHandlerV2> randomEventHandlerV2s = CreateRandomEventHandlerV2s();
            IReadOnlyList<EventHandlerV2> retrievedEventHandlerV2s = randomEventHandlerV2s;

            List<EventHandlerView> expectedEventHandlerViews =
                MapToViews(randomEventHandlerV2s);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventHandlerV2sAsync(
                    It.Is<EventHandlerV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(retrievedEventHandlerV2s);

            // when
            List<EventHandlerView> actualEventHandlerViews =
                await this.eventHandlersViewService.RetrieveAllEventHandlersAsync(
                    randomCancellationToken);

            // then
            actualEventHandlerViews.Should().BeEquivalentTo(expectedEventHandlerViews);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventHandlerV2sAsync(
                    It.Is<EventHandlerV2Query>(query => query.Take == 1000),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
