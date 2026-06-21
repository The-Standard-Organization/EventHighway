// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Hashings;

namespace EventHighway.Core.Tests.Unit.Brokers.Hashings
{
    public partial class HashBrokerTests
    {
        private readonly IHashBroker hashBroker;

        public HashBrokerTests() =>
            this.hashBroker = new HashBroker();

        private static string GetRandomString() =>
            Guid.NewGuid().ToString();
    }
}
