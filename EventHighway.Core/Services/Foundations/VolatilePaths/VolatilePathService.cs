// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Loggings;

namespace EventHighway.Core.Services.Foundations.VolatilePaths
{
    internal partial class VolatilePathService : IVolatilePathService
    {
        private readonly IJsonBroker jsonBroker;
        private readonly ILoggingBroker loggingBroker;

        public VolatilePathService(
            IJsonBroker jsonBroker,
            ILoggingBroker loggingBroker)
        {
            this.jsonBroker = jsonBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<string> RemoveVolatilePathsAsync(
            string content,
            string[] volatileContentPaths) =>
                throw new NotImplementedException();
    }
}
