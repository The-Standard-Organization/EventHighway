// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.DateTimes
{
    public interface IDateTimeBroker
    {
        ValueTask<DateTimeOffset> GetCurrentDateTimeOffsetAsync();
    }
}
