// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V1
{
    internal interface IListenerEventArchiveV1ProcessingService
    {
        ValueTask<ListenerEventArchiveV1> AddListenerEventArchiveAsync(
            ListenerEventArchiveV1 listenerEventArchive);
    }
}
