// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.EventListeners;

namespace EventHighway.Portal.Web.Services.Views.Foundations.EventListeners
{
    public partial class EventListenersViewService : IEventListenersViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventListenersViewService(
            IEventHighwayBroker eventHighwayBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<EventListenerView>> RetrieveListenersByAddressAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IQueryable<EventListenerV2> listeners =
                await this.eventHighwayBroker
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        eventAddressId, cancellationToken);

            return listeners.Select(AsView).ToList();
        });

        public ValueTask<EventListenerView> RegisterListenerAsync(
            EventListenerView listener,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            var listenerToRegister = new EventListenerV2
            {
                Id = Guid.NewGuid(),
                Name = listener.Name,
                Description = listener.Description,
                HandlerId = listener.HandlerId,
                HandlerName = listener.HandlerName,
                PromotedProperties = listener.PromotedProperties ?? string.Empty,
                FilterCriteria = listener.FilterCriteria ?? string.Empty,
                EventAddressV2Id = listener.EventAddressV2Id,
                EventParticipantV2Id = listener.EventParticipantV2Id,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventListenerV2 registeredListener =
                await this.eventHighwayBroker.RegisterEventListenerV2Async(
                    listenerToRegister, cancellationToken);

            return AsView(registeredListener);
        });

        public ValueTask<EventListenerView> RemoveListenerByIdAsync(
            Guid listenerId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            EventListenerV2 removedListener =
                await this.eventHighwayBroker.RemoveEventListenerV2ByIdAsync(
                    listenerId, cancellationToken);

            return AsView(removedListener);
        });

        private static EventListenerView AsView(EventListenerV2 listener) =>
            new EventListenerView
            {
                Id = listener.Id,
                Name = listener.Name,
                Description = listener.Description,
                HandlerName = listener.HandlerName,
                HandlerId = listener.HandlerId,
                EventAddressV2Id = listener.EventAddressV2Id,
                EventParticipantV2Id = listener.EventParticipantV2Id,
                PromotedProperties = listener.PromotedProperties ?? string.Empty,
                FilterCriteria = listener.FilterCriteria ?? string.Empty
            };
    }
}
