// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems
{
    public interface IExternalMediaItemService
    {
        ValueTask AddExternalMediaItemAsync(ExternalMediaItem externalMediaItem);
    }
}
