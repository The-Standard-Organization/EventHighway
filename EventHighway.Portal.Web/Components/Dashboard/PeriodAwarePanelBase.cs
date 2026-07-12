// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Components.Dashboard
{
    // Base for the period-aware dashboard panels (Traffic, Address, Loop, Duplicate, Retry,
    // Participant). When the global "Sync time periods" toggle is on, every panel follows the
    // shared period/window from the control bar and its own period control is disabled. When sync
    // is off, each panel keeps its own period/window cursor, driven by the control in its header.
    // The window end is only carried for the Custom period (standard periods derive it server-side
    // from the rolling window start). The global auto-refresh (RefreshToken) reloads the panel
    // regardless of sync.
    public abstract class PeriodAwarePanelBase : ComponentBase
    {
        [Parameter] public TrafficPeriodV2 GlobalPeriod { get; set; } = TrafficPeriodV2.Week;
        [Parameter] public DateTimeOffset GlobalWindowStart { get; set; }
        [Parameter] public DateTimeOffset? GlobalWindowEnd { get; set; }
        [Parameter] public bool SyncEnabled { get; set; } = true;
        [Parameter] public int RefreshToken { get; set; }

        protected TrafficPeriodV2 LocalPeriod { get; private set; } = TrafficPeriodV2.Week;
        protected DateTimeOffset LocalWindowStart { get; private set; }
        protected DateTimeOffset? LocalWindowEnd { get; private set; }

        protected TrafficPeriodV2 EffectivePeriod => SyncEnabled ? GlobalPeriod : LocalPeriod;

        protected DateTimeOffset EffectiveWindowStart =>
            SyncEnabled ? GlobalWindowStart : LocalWindowStart;

        protected DateTimeOffset? EffectiveWindowEnd =>
            SyncEnabled ? GlobalWindowEnd : LocalWindowEnd;

        protected bool CanGoNext =>
            WindowNavigator.CanGoNext(EffectivePeriod, EffectiveWindowStart, DateTimeOffset.UtcNow);

        // The per-panel control is disabled while the global sync is on.
        protected bool NavDisabled => SyncEnabled;

        private (TrafficPeriodV2 Period, DateTimeOffset Window, DateTimeOffset? End, int Token, bool Sync)?
            loadedKey;

        protected override void OnInitialized() =>
            LocalWindowStart = WindowNavigator.Current(LocalPeriod, DateTimeOffset.UtcNow);

        protected override async Task OnParametersSetAsync()
        {
            // While synced, keep the panel's own cursor mirroring the global one. This means that
            // when sync is turned off the panel stays exactly where it was (e.g. on Year), instead
            // of snapping back to an independent default — the user can then adjust it from there.
            if (SyncEnabled)
            {
                LocalPeriod = GlobalPeriod;
                LocalWindowStart = GlobalWindowStart;
                LocalWindowEnd = GlobalWindowEnd;
            }

            var key = (EffectivePeriod, EffectiveWindowStart, EffectiveWindowEnd, RefreshToken, SyncEnabled);

            if (loadedKey.HasValue && loadedKey.Value == key)
            {
                return;
            }

            loadedKey = key;
            await LoadAsync(EffectivePeriod, EffectiveWindowStart, EffectiveWindowEnd);
        }

        // Implemented by each panel: pull its data for the given period/window.
        protected abstract Task LoadAsync(
            TrafficPeriodV2 period, DateTimeOffset windowStart, DateTimeOffset? windowEnd);

        protected async Task OnPeriodChanged(TrafficPeriodV2 period)
        {
            LocalPeriod = period;

            if (period == TrafficPeriodV2.Custom)
            {
                // Default a custom range to the past week; the pickers adjust it from there.
                LocalWindowStart = WindowNavigator.Current(TrafficPeriodV2.Week, DateTimeOffset.UtcNow);
                LocalWindowEnd = WindowNavigator.WindowEnd(TrafficPeriodV2.Week, LocalWindowStart);
            }
            else
            {
                LocalWindowStart = WindowNavigator.Current(period, DateTimeOffset.UtcNow);
                LocalWindowEnd = null;
            }

            await ReloadLocalAsync();
        }

        protected async Task OnCustomRangeChanged((DateTimeOffset Start, DateTimeOffset End) range)
        {
            LocalWindowStart = range.Start;
            LocalWindowEnd = range.End;
            await ReloadLocalAsync();
        }

        protected async Task OnPrevious()
        {
            LocalWindowStart = WindowNavigator.Previous(LocalPeriod, LocalWindowStart);
            await ReloadLocalAsync();
        }

        protected async Task OnNext()
        {
            if (WindowNavigator.CanGoNext(LocalPeriod, LocalWindowStart, DateTimeOffset.UtcNow))
            {
                LocalWindowStart = WindowNavigator.Next(LocalPeriod, LocalWindowStart);
                await ReloadLocalAsync();
            }
        }

        protected async Task OnCurrent()
        {
            LocalWindowStart = WindowNavigator.Current(LocalPeriod, DateTimeOffset.UtcNow);
            await ReloadLocalAsync();
        }

        private async Task ReloadLocalAsync()
        {
            loadedKey = (EffectivePeriod, EffectiveWindowStart, EffectiveWindowEnd, RefreshToken, SyncEnabled);
            await LoadAsync(EffectivePeriod, EffectiveWindowStart, EffectiveWindowEnd);
            StateHasChanged();
        }
    }
}
