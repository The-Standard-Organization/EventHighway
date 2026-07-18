// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Fact]
        public async Task ShouldDispatchScheduledEventV2OnlyOnceWhenSweptConcurrentlyAsync()
        {
            // given
            int randomNumberA = GetRandomPositiveInt();
            int randomNumberB = GetRandomPositiveInt();
            string inputContent = $"{randomNumberA},{randomNumberB}";
            string expectedResponse = $"{randomNumberA + randomNumberB}";

            EventAddressV2 eventAddressV2 =
                await CreateRandomEventAddressV2Async();

            EventParticipantV2 eventParticipantV2 =
                await CreateRandomEventParticipantV2Async();

            EventListenerV2 listenerV2 =
                CreateDelegateHandlerListenerV2(eventAddressV2.Id, eventParticipantV2.Id);

            await this.clientBroker.RegisterEventListenerV2Async(listenerV2);

            EventV2 eventV2 =
                await SubmitScheduledEventV2Async(
                    eventAddressV2.Id,
                    eventParticipantV2.Id,
                    content: inputContent);

            // when — two overlapping sweeps race to claim and fire the same scheduled event
            await Task.WhenAll(
                this.clientBroker.FireScheduledPendingEventV2sAsync().AsTask(),
                this.clientBroker.FireScheduledPendingEventV2sAsync().AsTask());

            IReadOnlyList<ListenerEventV2> allListenerEventV2s =
                await RetrieveAllListenerEventV2sUntilAsync(listenerEventV2 =>
                    listenerEventV2.EventV2Id == eventV2.Id &&
                    listenerEventV2.Status == ListenerEventStatusV2.Success &&
                    listenerEventV2.Response == expectedResponse);

            // then — the atomic claim guarantees exactly one dispatch, no duplicates
            allListenerEventV2s
                .Where(listenerEventV2 => listenerEventV2.EventV2Id == eventV2.Id)
                .Should().ContainSingle(listenerEventV2 =>
                    listenerEventV2.Status == ListenerEventStatusV2.Success &&
                    listenerEventV2.Response == expectedResponse);

            // cleanup
            List<ListenerEventV2> listenerEventV2sToRemove = allListenerEventV2s
               .Where(listenerEventV2 => listenerEventV2.EventV2Id == eventV2.Id)
                    .ToList();

            foreach (ListenerEventV2 listenerEventV2 in listenerEventV2sToRemove)
                await this.clientBroker.RemoveListenerEventV2ByIdAsync(listenerEventV2.Id);

            await this.clientBroker.RemoveEventV2ByIdAsync(eventV2.Id);
            await this.clientBroker.RemoveEventListenerV2ByIdAsync(listenerV2.Id);
            await this.clientBroker.RemoveEventAddressV2ByIdAsync(eventAddressV2.Id);
        }
    }
}
