// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace EventHighway.Core.Services.Foundations.VolatilePaths
{
    internal interface IVolatilePathService
    {
        ValueTask<string> RemoveVolatilePathsAsync(
            string content,
            string[] volatileContentPaths);
    }
}
