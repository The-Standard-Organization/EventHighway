// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventHandlerV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<IEventHandler> randomEventHandlers = CreateRandomEventHandlers();
            IEnumerable<IEventHandler> retrievedEventHandlers = randomEventHandlers;

            IQueryable<IEventHandler> expectedEventHandlers =
                retrievedEventHandlers.AsQueryable();

            this.eventHandlerBrokerMock.Setup(broker =>
                broker.GetAll())
                    .Returns(retrievedEventHandlers);

            // when
            IQueryable<IEventHandler> actualEventHandlers =
                await this.eventHandlerV2Service.RetrieveAllEventHandlerV2sAsync(
                    randomCancellationToken);

            // then
            actualEventHandlers.Should().BeEquivalentTo(expectedEventHandlers);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.GetAll(),
                    Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
