// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.Loggings
{
    public interface ILoggingBroker
    {
        void LogInformation(string message);
        void LogError(Exception exception);
    }
}
