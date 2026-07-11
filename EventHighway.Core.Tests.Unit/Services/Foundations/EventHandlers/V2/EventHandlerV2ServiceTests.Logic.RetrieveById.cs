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
        public async Task ShouldRetrieveEventHandlerV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<IEventHandler> randomEventHandlers = CreateRandomEventHandlers().ToList();
            IEventHandler targetEventHandler = randomEventHandlers.First();
            IEventHandler expectedEventHandler = targetEventHandler;

            this.eventHandlerBrokerMock.Setup(broker =>
                broker.GetAll())
                    .Returns(randomEventHandlers);

            // when
            IEventHandler actualEventHandler =
                await this.eventHandlerV2Service.RetrieveEventHandlerV2ByIdAsync(
                    targetEventHandler.Id, randomCancellationToken);

            // then
            actualEventHandler.Should().BeEquivalentTo(expectedEventHandler);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.GetAll(),
                    Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
