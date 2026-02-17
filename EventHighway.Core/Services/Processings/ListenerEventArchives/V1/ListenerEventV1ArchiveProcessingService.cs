// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V1;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V1
{
    internal partial class ListenerEventV1ArchiveProcessingService : IListenerEventV1ArchiveProcessingService
    {
        private readonly IListenerEventV1ArchiveService listenerEventV1ArchiveService;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventV1ArchiveProcessingService(
            IListenerEventV1ArchiveService listenerEventV1ArchiveService,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventV1ArchiveService = listenerEventV1ArchiveService;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<ListenerEventV1Archive> AddListenerEventV1ArchiveAsync(
            ListenerEventV1Archive listenerEventV1Archive)
        {
            return await this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(
                listenerEventV1Archive);
        }
    }
}
