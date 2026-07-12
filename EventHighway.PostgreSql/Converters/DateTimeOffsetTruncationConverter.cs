using System;
using EventHighway.PostgreSql.Services.Foundations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventHighway.PostgreSql.Converters
{
    internal sealed class DateTimeOffsetTruncationConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
    {
        public DateTimeOffsetTruncationConverter()
            : base(
                dateTimeOffset => DateTimeService.TruncateToMicroseconds(dateTimeOffset),
                dateTimeOffset => dateTimeOffset)
        { }
    }    
}
