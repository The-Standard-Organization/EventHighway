// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.Replays;

namespace EventHighway.Portal.Web.Services.Views.Foundations.Replays
{
    public partial class ReplayViewService : IReplayViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public ReplayViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ReplayAsync(
            ReplayRequestView replayRequest,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.eventHighwayBroker.ReplayEventArchiveV2sAsync(
                eventAddressId: replayRequest.EventAddressV2Id,
                eventListenerIds: replayRequest.EventListenerV2Ids ?? new List<Guid>(),
                startDate: replayRequest.StartDate,
                endDate: replayRequest.EndDate,
                cancellationToken: cancellationToken);

            await this.eventHighwayBroker.ProcessReplayedListenerEventV2sAsync(
                cancellationToken);
        });

        public ValueTask ReplayListenerEventArchiveAsync(
            Guid eventV2Id,
            Guid eventAddressId,
            Guid eventListenerId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.eventHighwayBroker.ReplayEventArchiveV2sAsync(
                eventV2Id: eventV2Id,
                eventAddressId: eventAddressId,
                eventListenerIds: new[] { eventListenerId },
                allowReplayOfQuarantinedItem: true,
                cancellationToken: cancellationToken);

            await this.eventHighwayBroker.ProcessReplayedListenerEventV2sAsync(
                cancellationToken);
        });
    }
}
