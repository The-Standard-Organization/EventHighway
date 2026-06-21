// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Configurations.LoopDetections;

namespace EventHighway.Core.Tests.Unit.Brokers.Configurations
{
    public partial class ConfigurationBrokerTests
    {
        private static LoopDetection CreateRandomLoopDetection()
        {
            return new LoopDetection
            {
                Enabled = true,
                Threshold = GetRandomNumber(),
                Window = TimeSpan.FromSeconds(GetRandomNumber()),
                VolatilePaths = new List<VolatilePaths>
                {
                    new VolatilePaths
                    {
                        EventAddressId = Guid.NewGuid(),
                        Name = GetRandomString(),
                        VolatileContentPaths = new[] { GetRandomString(), GetRandomString() }
                    }
                }
            };
        }

        private static int GetRandomNumber() =>
            new Random().Next(minValue: 2, maxValue: 10);

        private static string GetRandomString() =>
            Guid.NewGuid().ToString();
    }
}
