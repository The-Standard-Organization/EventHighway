// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Models.Services.Views.Foundations.HealthDashboards;
using EventHighway.Portal.Web.Services.Views.Foundations.HealthDashboards;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Views.Components.Foundations.Dashboards
{
    public partial class RagTileRow
    {
        [Inject]
        public IHealthViewService HealthViewService { get; set; } = default!;

        public RagTileRowState State { get; private set; } = RagTileRowState.Loading;

        public List<HealthRagTile> Tiles { get; private set; } = new();

        public string? ErrorMessage { get; private set; }

        [Parameter]
        public int RefreshToken { get; set; }

        private int? loadedToken;

        protected override async Task OnParametersSetAsync()
        {
            if (loadedToken == RefreshToken)
            {
                return;
            }

            loadedToken = RefreshToken;

            try
            {
                // The RAG tiles are whole-system and ignore the window (§0 rule 4); a current-period
                // window is supplied only to satisfy the coordination contract.
                DateTimeOffset windowStart =
                    WindowNavigator.Current(TrafficPeriodV2.Day, DateTimeOffset.UtcNow);

                Tiles = await this.HealthViewService.RetrieveHealthRagTilesAsync(
                    TrafficPeriodV2.Day, windowStart);

                State = RagTileRowState.Content;
            }
            catch (Exception)
            {
                ErrorMessage = "Unable to load health status. Please try again.";
                State = RagTileRowState.Error;
            }
        }
    }
}
