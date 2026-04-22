// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant Logging Broker
// Demonstrates: local interface, async ValueTask, direct logger calls (no Task.Run),
// no flow control, no exception handling, registered as Transient.

using System;
using Microsoft.Extensions.Logging;

namespace MyProject.Brokers.Loggings
{
    // arch-001: Implements a local interface (ILoggingBroker)
    // arch-009: All methods are async ValueTask
    // arch-002: No flow control
    // arch-003: No exception handling — raw exceptions propagate to the service
    // arch-010: Support broker — cross-cutting infrastructure only
    public class LoggingBroker : ILoggingBroker
    {
        private readonly ILogger<LoggingBroker> logger;

        public LoggingBroker(ILogger<LoggingBroker> logger) =>
            this.logger = logger;

        // CORRECT: async expression-bodied method delegates directly to ILogger<T>.
        // DO NOT wrap with Task.Run() — ILogger<T> is synchronous by design.
        // Task.Run() introduces thread-pool overhead, can swallow async context,
        // and produces a ValueTask backed by a Task unnecessarily.
        public async ValueTask LogInformationAsync(string message) =>
            this.logger.LogInformation(message);

        public async ValueTask LogTraceAsync(string message) =>
            this.logger.LogTrace(message);

        public async ValueTask LogDebugAsync(string message) =>
            this.logger.LogDebug(message);

        public async ValueTask LogWarningAsync(string message) =>
            this.logger.LogWarning(message);

        public async ValueTask LogErrorAsync(Exception exception) =>
            this.logger.LogError(exception, exception.Message);

        public async ValueTask LogCriticalAsync(Exception exception) =>
            this.logger.LogCritical(exception, exception.Message);
    }
}

// ---------------------------------------------------------------
// Interface (correct)
// ---------------------------------------------------------------

namespace MyProject.Brokers.Loggings
{
    // arch-001: Local interface pattern — I{Resource}Broker
    public interface ILoggingBroker
    {
        ValueTask LogInformationAsync(string message);
        ValueTask LogTraceAsync(string message);
        ValueTask LogDebugAsync(string message);
        ValueTask LogWarningAsync(string message);
        ValueTask LogErrorAsync(Exception exception);
        ValueTask LogCriticalAsync(Exception exception);
    }
}

// ---------------------------------------------------------------
// DI Registration in Program.cs (correct)
// ---------------------------------------------------------------
//
// builder.Services.AddLogging();   // registers ILogger<T> from Microsoft.Extensions.Logging
// builder.Services.AddTransient<ILoggingBroker, LoggingBroker>();
//
// NOTE: AddLogging() must be called (or AddDefaultLogging() via the host builder)
// so that ILogger<LoggingBroker> is resolvable. Omitting this registration causes
// a runtime DI resolution failure.
