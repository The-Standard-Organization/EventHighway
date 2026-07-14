// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        ValueTask<EventParticipantSecretV2> AddParticipantSecretAsync(
            EventParticipantSecretV2 participantSecret,
            CancellationToken cancellationToken = default);
    }
}
