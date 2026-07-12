// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace EventHighway.Core.Brokers.Times
{
    internal class DateTimeBroker : IDateTimeBroker
    {
        // Timestamps carry microsecond precision — the least common denominator of the
        // supported storage providers (PostgreSQL timestamptz(6)) — so values written by
        // any service compare equal to their stored counterparts on every provider.
        public async ValueTask<DateTimeOffset> GetDateTimeOffsetAsync() =>
            DateTimeOffset.UtcNow;
    }
}
