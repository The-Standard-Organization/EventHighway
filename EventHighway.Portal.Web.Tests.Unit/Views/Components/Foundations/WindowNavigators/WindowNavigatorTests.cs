// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Views.Components.Foundations.Dashboards;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Components.Foundations.Dashboards
{
    public class WindowNavigatorTests
    {
        private static readonly DateTimeOffset Now =
            new DateTimeOffset(2026, 6, 24, 13, 45, 0, TimeSpan.Zero); // Wed 24 Jun 2026

        [Fact]
        public void ShouldAnchorCurrentToRollingWindowStart()
        {
            // Past 24 hours: hour-aligned start covering now (window end = start + 24h = 14:00).
            WindowNavigator.Current(TrafficPeriodV2.Day, Now)
                .Should().Be(new DateTimeOffset(2026, 6, 23, 14, 0, 0, TimeSpan.Zero));

            // Past week: day-aligned start covering today (window end = tomorrow midnight).
            WindowNavigator.Current(TrafficPeriodV2.Week, Now)
                .Should().Be(new DateTimeOffset(2026, 6, 18, 0, 0, 0, TimeSpan.Zero));

            // Past month: day-aligned start one month before tomorrow midnight.
            WindowNavigator.Current(TrafficPeriodV2.Month, Now)
                .Should().Be(new DateTimeOffset(2026, 5, 25, 0, 0, 0, TimeSpan.Zero));

            // Past year: month-aligned start one year before the next month boundary.
            WindowNavigator.Current(TrafficPeriodV2.Year, Now)
                .Should().Be(new DateTimeOffset(2025, 7, 1, 0, 0, 0, TimeSpan.Zero));
        }

        [Fact]
        public void ShouldComputePreviousWindow()
        {
            var dayStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);

            WindowNavigator.Previous(TrafficPeriodV2.Day, dayStart)
                .Should().Be(new DateTimeOffset(2026, 6, 23, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Previous(TrafficPeriodV2.Week, dayStart)
                .Should().Be(new DateTimeOffset(2026, 6, 17, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Previous(TrafficPeriodV2.Month, dayStart)
                .Should().Be(new DateTimeOffset(2026, 5, 24, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Previous(TrafficPeriodV2.Year, dayStart)
                .Should().Be(new DateTimeOffset(2025, 6, 24, 0, 0, 0, TimeSpan.Zero));
        }

        [Fact]
        public void ShouldComputeNextWindow()
        {
            var dayStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);

            WindowNavigator.Next(TrafficPeriodV2.Day, dayStart)
                .Should().Be(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Next(TrafficPeriodV2.Month, dayStart)
                .Should().Be(new DateTimeOffset(2026, 7, 24, 0, 0, 0, TimeSpan.Zero));
        }

        [Fact]
        public void ShouldGuardFutureOnCanGoNext()
        {
            DateTimeOffset currentDay = WindowNavigator.Current(TrafficPeriodV2.Day, Now);

            // current window — cannot advance into the future
            WindowNavigator.CanGoNext(TrafficPeriodV2.Day, currentDay, Now).Should().BeFalse();

            // a past window — can advance
            DateTimeOffset previousDay = WindowNavigator.Previous(TrafficPeriodV2.Day, currentDay);
            WindowNavigator.CanGoNext(TrafficPeriodV2.Day, previousDay, Now).Should().BeTrue();
        }

        [Fact]
        public void ShouldComputeWindowEnd()
        {
            var dayStart = new DateTimeOffset(2026, 6, 23, 14, 0, 0, TimeSpan.Zero);

            WindowNavigator.WindowEnd(TrafficPeriodV2.Day, dayStart)
                .Should().Be(new DateTimeOffset(2026, 6, 24, 14, 0, 0, TimeSpan.Zero));

            var weekStart = new DateTimeOffset(2026, 6, 18, 0, 0, 0, TimeSpan.Zero);

            WindowNavigator.WindowEnd(TrafficPeriodV2.Week, weekStart)
                .Should().Be(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

            var monthStart = new DateTimeOffset(2026, 5, 25, 0, 0, 0, TimeSpan.Zero);

            WindowNavigator.WindowEnd(TrafficPeriodV2.Month, monthStart)
                .Should().Be(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

            var yearStart = new DateTimeOffset(2025, 7, 1, 0, 0, 0, TimeSpan.Zero);

            WindowNavigator.WindowEnd(TrafficPeriodV2.Year, yearStart)
                .Should().Be(new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero));
        }

        [Fact]
        public void ShouldFormatWindowLabelAsInclusiveRange()
        {
            // Rolling 24h windows show the exact hour bounds.
            WindowNavigator.Label(TrafficPeriodV2.Day,
                new DateTimeOffset(2026, 6, 23, 14, 0, 0, TimeSpan.Zero))
                .Should().Be("23 Jun 2026 14:00 – 24 Jun 2026 14:00");

            // Longer windows show an inclusive day range.
            WindowNavigator.Label(TrafficPeriodV2.Week,
                new DateTimeOffset(2026, 6, 18, 0, 0, 0, TimeSpan.Zero))
                .Should().Be("18 Jun 2026 – 24 Jun 2026");

            WindowNavigator.Label(TrafficPeriodV2.Month,
                new DateTimeOffset(2026, 5, 25, 0, 0, 0, TimeSpan.Zero))
                .Should().Be("25 May 2026 – 24 Jun 2026");

            WindowNavigator.Label(TrafficPeriodV2.Year,
                new DateTimeOffset(2025, 7, 1, 0, 0, 0, TimeSpan.Zero))
                .Should().Be("01 Jul 2025 – 30 Jun 2026");
        }

        [Fact]
        public void ShouldFormatCustomWindowLabelFromExplicitEnd()
        {
            WindowNavigator.Label(
                TrafficPeriodV2.Custom,
                new DateTimeOffset(2026, 6, 6, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 6, 13, 0, 0, 0, TimeSpan.Zero))
                .Should().Be("06 Jun 2026 – 12 Jun 2026");
        }
    }
}
