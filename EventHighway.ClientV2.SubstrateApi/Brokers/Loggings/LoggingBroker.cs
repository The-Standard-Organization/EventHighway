// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.Loggings
{
    // The console counterpart in the SubstrateApp sample writes straight to Console; a web host has
    // a logging pipeline of its own, so this one hands over to it.
    public sealed class LoggingBroker : ILoggingBroker
    {
        private readonly ILogger<LoggingBroker> logger;

        public LoggingBroker(ILogger<LoggingBroker> logger) =>
            this.logger = logger;

        public void LogInformation(string message) =>
            this.logger.LogInformation("{Message}", message);

        public void LogError(Exception exception) =>
            this.logger.LogError(exception, "{Message}", exception.Message);
    }
}
