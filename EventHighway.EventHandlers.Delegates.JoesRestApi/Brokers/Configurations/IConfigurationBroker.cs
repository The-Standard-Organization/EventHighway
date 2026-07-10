// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.EventHandlers.Delegates.JoesRestApi.Models.Brokers.Configurations;

namespace EventHighway.EventHandlers.Delegates.JoesRestApi.Brokers.Configurations
{
    internal interface IConfigurationBroker
    {
        JoesRestApiConfigurations GetJoesRestApiConfigurations();
    }
}
