// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.Loggings
{
    public sealed class LoggingBroker : ILoggingBroker
    {
        public void LogInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  [{DateTime.UtcNow:HH:mm:ss.fff}] [INFO] {message}");
            Console.ResetColor();
        }

        public void LogError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"  [{DateTime.UtcNow:HH:mm:ss.fff}] [ERROR] {exception.Message}");
            Console.ResetColor();
        }
    }
}
