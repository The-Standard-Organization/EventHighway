// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
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

        public ValueTask<ListenerEventV1> AddListenerEventV1Async(ListenerEventV1 listenerEventV1) =>
        TryCatch(async () =>
        {
            ValidateListenerEventV1IsNotNull(listenerEventV1);

            return await this.listenerEventV1Service.AddListenerEventAsync(listenerEventV1);
        });

        public ValueTask<IQueryable<ListenerEventV1>> RetrieveAllListenerEventV1sAsync() =>
        TryCatch(async () => await this.listenerEventV1Service.RetrieveAllListenerEventsAsync());

        public ValueTask<ListenerEventV1> ModifyListenerEventV1Async(ListenerEventV1 listenerEventV1) =>
        TryCatch(async () =>
        {
            ValidateListenerEventV1IsNotNull(listenerEventV1);

            return await this.listenerEventV1Service.ModifyListenerEventAsync(listenerEventV1);
        });

        public ValueTask<ListenerEventV1> RemoveListenerEventV1ByIdAsync(
            Guid listenerEventV1Id) => TryCatch(async () =>
        {
            ValidateListenerEventV1Id(listenerEventV1Id);

            return await this.listenerEventV1Service
                .RemoveListenerEventByIdAsync(listenerEventV1Id);
        });
    }
}
