// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V1
{
    internal partial class ListenerEventArchiveV1ProcessingService : IListenerEventArchiveV1ProcessingService
    {
        private readonly IListenerEventArchiveV1Service listenerEventArchiveV1Service;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventArchiveV1ProcessingService(
            IListenerEventArchiveV1Service listenerEventArchiveV1Service,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventArchiveV1Service = listenerEventArchiveV1Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventArchiveV1> AddListenerEventArchiveV1Async(
            ListenerEventArchiveV1 listenerEventArchiveV1) => TryCatch(async () =>
        {
            ValidateListenerEventArchiveV1(listenerEventArchiveV1);

            return await this.listenerEventArchiveV1Service
                .AddListenerEventArchiveV1Async(listenerEventArchiveV1);
        });
    }
}
