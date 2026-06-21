// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Jsons;

namespace EventHighway.Core.Tests.Unit.Brokers.Jsons
{
    public partial class JsonBrokerTests
    {
        private readonly IJsonBroker jsonBroker;

        public JsonBrokerTests() =>
            this.jsonBroker = new JsonBroker();

        private static string GetRandomString() =>
            Guid.NewGuid().ToString();
    }
}
