// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApp.Models.Events;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        /// <summary>
        /// The generic publish seam for services: serializes the envelope's typed content and
        /// submits it onto the event substrate as an <see cref="EventV2"/>.
        /// </summary>
        ValueTask<EventV2> EmitAsync<TContent>(
            EventEnvelope<TContent> envelope,
            CancellationToken cancellationToken = default);
    }
}
