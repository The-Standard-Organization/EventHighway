using System;
using EventHighway.Core.Brokers.Times;

namespace EventHighway.PostgreSql.Services.Foundations
{
    internal partial class DateTimeService
    {
        private readonly IDateTimeBroker dateTimeBroker;

        public DateTimeService(IDateTimeBroker dateTimeBroker) =>
            this.dateTimeBroker = dateTimeBroker;

        public static DateTimeOffset TruncateToMicroseconds(DateTimeOffset dateTimeOffset) =>
            dateTimeOffset.AddTicks(-(dateTimeOffset.Ticks % TimeSpan.TicksPerMicrosecond));
    }
}