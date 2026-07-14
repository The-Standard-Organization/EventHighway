// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        public ValueTask<EventAddressV2> RetrieveOrRegisterAddressAsync(
            EventAddressV2 eventAddress,
            CancellationToken cancellationToken = default) =>
            this.databaseGate.ExecuteAsync(
                () => this.eventHighwayClient.V2.EventAddressV2Client
                    .RetrieveOrRegisterEventAddressV2Async(eventAddress, cancellationToken),
                cancellationToken);
    }
}
