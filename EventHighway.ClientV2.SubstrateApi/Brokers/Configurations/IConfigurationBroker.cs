// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApi.Models.Brokers.Configurations;

namespace EventHighway.ClientV2.SubstrateApi.Brokers.Configurations
{
    public interface IConfigurationBroker
    {
        SubstrateApiConfigurations GetSubstrateApiConfigurations();
    }
}
