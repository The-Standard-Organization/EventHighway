// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Globalization;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Portal.Web.Components.Dashboard
{
    // Pure period/window math for the dashboard control bar. Windows are ROLLING — anchored so the
    // current window always covers "now" (past 24 hours / week / month / year) rather than the
    // calendar day/month/year — and starts are truncated to the bucket boundary (hour for Day,
    // midnight for Week/Month, first-of-month for Year) so the coordination's server-side bucket
    // truncation lines up. The coordination derives the window end from period + windowStart
    // except for Custom, where the caller supplies an explicit end.
    public static class WindowNavigator
    {
        public static DateTimeOffset Current(TrafficPeriodV2 period, DateTimeOffset now)
        {
            DateTimeOffset utc = now.ToUniversalTime();

            var hourStart = new DateTimeOffset(
                utc.Year, utc.Month, utc.Day, utc.Hour, 0, 0, TimeSpan.Zero);

            var dayStart = new DateTimeOffset(utc.Date, TimeSpan.Zero);
            DateTimeOffset nextDay = dayStart.AddDays(1);

            var nextMonth = new DateTimeOffset(utc.Year, utc.Month, 1, 0, 0, 0, TimeSpan.Zero)
                .AddMonths(1);

            return period switch
            {
                TrafficPeriodV2.Day => hourStart.AddHours(-23),
                TrafficPeriodV2.Week => nextDay.AddDays(-7),
                TrafficPeriodV2.Month => nextDay.AddMonths(-1),
                TrafficPeriodV2.Year => nextMonth.AddYears(-1),
                _ => nextDay.AddDays(-7)
            };
        }

        public static DateTimeOffset Previous(TrafficPeriodV2 period, DateTimeOffset windowStart) =>
            Shift(period, windowStart, forward: false);

        public static DateTimeOffset Next(TrafficPeriodV2 period, DateTimeOffset windowStart) =>
            Shift(period, windowStart, forward: true);

        public static bool CanGoNext(
            TrafficPeriodV2 period, DateTimeOffset windowStart, DateTimeOffset now) =>
            Next(period, windowStart) <= Current(period, now);

        public static DateTimeOffset WindowEnd(TrafficPeriodV2 period, DateTimeOffset windowStart)
        {
            return period switch
            {
                TrafficPeriodV2.Week => windowStart.AddDays(7),
                TrafficPeriodV2.Month => windowStart.AddMonths(1),
                TrafficPeriodV2.Year => windowStart.AddYears(1),
                _ => windowStart.AddHours(24)
            };
        }

        public static string Label(TrafficPeriodV2 period, DateTimeOffset windowStart) =>
            Label(period, windowStart, WindowEnd(period, windowStart));

        public static string Label(
            TrafficPeriodV2 period, DateTimeOffset windowStart, DateTimeOffset windowEnd)
        {
            DateTimeOffset start = windowStart.ToUniversalTime();
            DateTimeOffset end = windowEnd.ToUniversalTime();

            return period switch
            {
                TrafficPeriodV2.Day =>
                    $"{start.ToString("dd MMM yyyy HH:00", CultureInfo.InvariantCulture)} – " +
                    $"{end.ToString("dd MMM yyyy HH:00", CultureInfo.InvariantCulture)}",

                _ =>
                    $"{start.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)} – " +
                    $"{end.AddDays(-1).ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}"
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
                _ => windowStart
            };
        }
    }
}
