// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EventHighway.Core.Brokers.Loggings
{
    internal class LoggingBroker : ILoggingBroker
    {
        private readonly ILogger<LoggingBroker> logger;

        public LoggingBroker(ILogger<LoggingBroker> logger) =>
            this.logger = logger;

        public async ValueTask LogErrorAsync(Exception exception)
        {
            this.logger.LogError(
                exception: exception,
                message: exception.Message);
        }

        public async ValueTask LogCriticalAsync(Exception exception)
        {
            this.logger.LogCritical(
                exception: exception,
                message: exception.Message);
        }
    }
}
