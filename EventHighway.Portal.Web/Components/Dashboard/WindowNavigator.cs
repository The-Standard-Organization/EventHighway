// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Globalization;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Portal.Web.Components.Dashboard
{
    // Pure period/window math for the dashboard control bar. The coordination service derives the
    // window end from period + windowStart (Spec §4.2); this navigator only produces the inclusive
    // UTC window start for "current", "previous" and "next", the future-guard, and a human label.
    public static class WindowNavigator
    {
        public static DateTimeOffset Current(TrafficPeriodV2 period, DateTimeOffset now)
        {
            DateTimeOffset utc = now.ToUniversalTime();
            var date = new DateTimeOffset(utc.Date, TimeSpan.Zero);

            return period switch
            {
                TrafficPeriodV2.Day => date,
                TrafficPeriodV2.Week => date.AddDays(-(((int)date.DayOfWeek + 6) % 7)),
                TrafficPeriodV2.Month => new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, TimeSpan.Zero),
                TrafficPeriodV2.Year => new DateTimeOffset(date.Year, 1, 1, 0, 0, 0, TimeSpan.Zero),
                _ => date
            };
        }

        public static DateTimeOffset Previous(TrafficPeriodV2 period, DateTimeOffset windowStart) =>
            Shift(period, windowStart, forward: false);

        public static DateTimeOffset Next(TrafficPeriodV2 period, DateTimeOffset windowStart) =>
            Shift(period, windowStart, forward: true);

        public static bool CanGoNext(
            TrafficPeriodV2 period, DateTimeOffset windowStart, DateTimeOffset now) =>
            Next(period, windowStart) <= Current(period, now);

        public static DateTimeOffset WindowEnd(TrafficPeriodV2 period, DateTimeOffset windowStart) =>
            throw new NotImplementedException();

        public static string Label(TrafficPeriodV2 period, DateTimeOffset windowStart, DateTimeOffset windowEnd) =>
            throw new NotImplementedException();

        public static string Label(TrafficPeriodV2 period, DateTimeOffset windowStart)
        {
            DateTimeOffset start = windowStart.ToUniversalTime();

            return period switch
            {
                TrafficPeriodV2.Day => start.ToString("dd MMM yyyy", CultureInfo.InvariantCulture),
                TrafficPeriodV2.Week =>
                    $"{start:dd MMM} – {start.AddDays(6):dd MMM yyyy}",
                TrafficPeriodV2.Month => start.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                TrafficPeriodV2.Year => start.ToString("yyyy", CultureInfo.InvariantCulture),
                _ => start.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)
            };
        }

        private static DateTimeOffset Shift(
            TrafficPeriodV2 period, DateTimeOffset windowStart, bool forward)
        {
            int sign = forward ? 1 : -1;

            return period switch
            {
                TrafficPeriodV2.Day => windowStart.AddDays(1 * sign),
                TrafficPeriodV2.Week => windowStart.AddDays(7 * sign),
                TrafficPeriodV2.Month => windowStart.AddMonths(1 * sign),
                TrafficPeriodV2.Year => windowStart.AddYears(1 * sign),
                _ => windowStart.AddDays(1 * sign)
            };
        }
    }
}
