// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using EventHighway.Core.Services.Foundations.ListenerEvents.V1;

namespace EventHighway.Core.Services.Processings.ListenerEvents.V1
{
    internal partial class ListenerEventV1ProcessingService : IListenerEventV1ProcessingService
    {
        private readonly IListenerEventV1Service listenerEventV1Service;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventV1ProcessingService(
            IListenerEventV1Service listenerEventV1Service,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventV1Service = listenerEventV1Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventV1> AddListenerEventAsync(ListenerEventV1 listenerEvent) =>
        TryCatch(async () =>
        {
            ValidateListenerEventIsNotNull(listenerEvent);

            return await this.listenerEventV1Service.AddListenerEventAsync(listenerEvent);
        });

        public ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventsAsync() =>
        TryCatch(async () => await this.listenerEventV1Service.RetrieveAllListenerEventsAsync());

        public ValueTask<ListenerEventV1> ModifyListenerEventAsync(ListenerEventV1 listenerEvent) =>
        TryCatch(async () =>
        {
            ValidateListenerEventIsNotNull(listenerEvent);

            return await this.listenerEventV1Service.ModifyListenerEventAsync(listenerEvent);
        });

        public ValueTask<ListenerEventV1> RemoveListenerEventByIdAsync(
            Guid listenerEventId) => TryCatch(async () =>
        {
            ValidateListenerEventId(listenerEventId);

            return await this.listenerEventV1Service
                .RemoveListenerEventByIdAsync(listenerEventId);
        });
    }
}
