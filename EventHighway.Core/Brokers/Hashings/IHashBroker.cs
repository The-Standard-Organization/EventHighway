// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Brokers.Hashings
{
    internal interface IHashBroker
    {
        string GenerateSha256Hash(string content);
    }
}
