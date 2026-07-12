// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace EventHighway.PostgreSql.Brokers.Times
{
    internal interface IDateTimeBroker
    {
        ValueTask<DateTimeOffset> GetDateTimeOffsetAsync();
    }
}
