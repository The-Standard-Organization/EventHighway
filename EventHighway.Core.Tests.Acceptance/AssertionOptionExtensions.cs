// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace EventHighway.Core.Tests.Acceptance.Extensions
{
    internal static class AssertionOptionsExtensions
    {
        // A tight tolerance: 1 microsecond covers the truncation applied by
        // PostgreSQL's timestamptz(6) storage, while still catching data corruption.
        private static readonly TimeSpan DateTimeOffsetTolerance = TimeSpan.FromMicroseconds(1);

        public static EquivalencyAssertionOptions<T> WithDateTimeOffsetTolerance<T>(
            this EquivalencyAssertionOptions<T> options) =>
                options.Using<DateTimeOffset>(context =>
                    context.Subject.Should().BeCloseTo(
                        context.Expectation,
                        DateTimeOffsetTolerance))
                .WhenTypeIs<DateTimeOffset>();
    }
}