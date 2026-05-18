// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V1
{
    internal interface IListenerEventArchiveV1ProcessingService
    {
        ValueTask<ListenerEventArchiveV1> AddListenerEventArchiveV1Async(
            ListenerEventArchiveV1 listenerEventArchiveV1);
    }
}
