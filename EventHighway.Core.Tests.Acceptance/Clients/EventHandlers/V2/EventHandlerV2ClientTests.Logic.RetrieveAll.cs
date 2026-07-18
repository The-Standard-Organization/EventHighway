// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2;
using EventHighway.Core.Models.Services.Processings.EventHandlers.V2;
using EventHighway.EventHandlers;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventHandlers.V2
{
    public partial class EventHandlerV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventHandlerV2sAsync()
        {
            // given
            DelegateEventHandler inputEventHandler =
                CreateRandomDelegateEventHandler();

            this.clientBroker.RegisterEventHandler(inputEventHandler);

            // when
            IReadOnlyList<EventHandlerV2> actualEventHandlerV2s =
                await this.clientBroker.RetrieveAllEventHandlerV2sAsync(
                    new EventHandlerV2Query { Take = 1000 });

            // then
            actualEventHandlerV2s.Should().Contain(eventHandlerV2 =>
                eventHandlerV2.Id == inputEventHandler.Id
                    && eventHandlerV2.Name == inputEventHandler.Name);

            await this.clientBroker.RemoveEventHandlerV2ByIdAsync(inputEventHandler.Id);
        }

        [Fact]
        public async Task ShouldRegisterEventHandlerV2AsyncAndRetrieveAsync()
        {
            // given
            DelegateEventHandler inputEventHandler =
                CreateRandomDelegateEventHandler();

            await this.clientBroker.RegisterEventHandlerAsync(inputEventHandler);

            // when
            IReadOnlyList<EventHandlerV2> actualEventHandlerV2s =
                await this.clientBroker.RetrieveAllEventHandlerV2sAsync(
                    new EventHandlerV2Query { Take = 1000 });

            // then
            actualEventHandlerV2s.Should().Contain(eventHandlerV2 =>
                eventHandlerV2.Id == inputEventHandler.Id
                    && eventHandlerV2.Name == inputEventHandler.Name);

            await this.clientBroker.RemoveEventHandlerV2ByIdAsync(inputEventHandler.Id);
        }
    }
}
